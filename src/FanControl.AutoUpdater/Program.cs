using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FanControl.AutoUpdater;

internal static class Program
{
    private const string VersionUrl = "https://raw.githubusercontent.com/Rem0o/FanControl.Releases/master/version.json";
    private const string LatestReleaseUrl = "https://api.github.com/repos/Rem0o/FanControl.Releases/releases/latest";
    private const string UpdaterUrl = "https://raw.githubusercontent.com/Rem0o/FanControl.Releases/master/Updater.exe";
    private const string TaskName = "FanControl";

    private static readonly string ApplicationDirectory = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
    private static readonly string SettingsPath = Path.Combine(ApplicationDirectory, "FanControl.AutoUpdater.json");
    private static readonly string LogPath = Path.Combine(ApplicationDirectory, "FanControl.AutoUpdater.log");

    [STAThread]
    private static async Task<int> Main(string[] args)
    {
        string command = args.FirstOrDefault()?.ToLowerInvariant() ?? "--configure";

        return command switch
        {
            "--startup" => await RunAtStartupAsync(RunMode.Startup),
            "--check-only" => await RunAtStartupAsync(RunMode.CheckOnly),
            "--prepare-only" => await RunAtStartupAsync(RunMode.PrepareOnly),
            "--enable" => Configure(enabled: true, interactive: false),
            "--disable" => Configure(enabled: false, interactive: false),
            _ => ConfigureInteractively()
        };
    }

    private static int ConfigureInteractively()
    {
        int answer = MessageBoxW(
            IntPtr.Zero,
            "Automatically install Fan Control updates at logon before Fan Control starts?\n\n" +
            "Yes: enable automatic updates\nNo: disable and restore normal startup\nCancel: leave unchanged",
            "Fan Control automatic updates",
            0x00000003 | 0x00000020 | 0x00001000);

        return answer switch
        {
            6 => Configure(enabled: true, interactive: true),
            7 => Configure(enabled: false, interactive: true),
            _ => 0
        };
    }

    private static int Configure(bool enabled, bool interactive)
    {
        try
        {
            SaveSettings(new AutoUpdateSettings(enabled, 20));
            bool taskUpdated = SetStartupTask(enabled);
            string message = taskUpdated
                ? enabled
                    ? "Automatic updates at startup are enabled."
                    : "Automatic updates are disabled. Normal Fan Control startup is restored."
                : "The FanControl startup task could not be updated. See the log for details.";

            Log(message);
            if (interactive)
            {
                MessageBoxW(IntPtr.Zero, message, "Fan Control automatic updates", taskUpdated ? 0x40u : 0x10u);
            }
            return taskUpdated ? 0 : 1;
        }
        catch (Exception exception)
        {
            Log(exception.ToString());
            if (interactive)
            {
                MessageBoxW(IntPtr.Zero, exception.Message, "Fan Control automatic updates", 0x10);
            }
            return 1;
        }
    }

    private static async Task<int> RunAtStartupAsync(RunMode mode)
    {
        using Mutex mutex = new(initiallyOwned: true, "Global\\FanControl.AutoUpdater", out bool createdNew);
        if (!createdNew)
        {
            return 0;
        }

        AutoUpdateSettings settings = LoadSettings();
        if (!settings.Enabled)
        {
            if (mode == RunMode.Startup) StartFanControl();
            return 0;
        }

        try
        {
            int currentVersion = GetCurrentVersion();
            using HttpClient client = CreateHttpClient(settings.CheckTimeoutSeconds);

            using JsonDocument versionDocument = await GetJsonAsync(client, GetEndpoint("FANCONTROL_AUTOUPDATE_VERSION_URL", VersionUrl));
            int remoteVersion = versionDocument.RootElement.GetProperty("Number").GetInt32();
            string expectedUpdaterHash = versionDocument.RootElement
                .GetProperty("Checksums")
                .GetProperty("Updater")
                .GetString() ?? throw new InvalidDataException("Updater checksum is missing.");

            if (remoteVersion <= currentVersion)
            {
                Log($"V{currentVersion} is current; remote is V{remoteVersion}.");
                if (mode == RunMode.Startup) StartFanControl();
                return 0;
            }

            using JsonDocument releaseDocument = await GetJsonAsync(client, GetEndpoint("FANCONTROL_AUTOUPDATE_RELEASE_URL", LatestReleaseUrl));
            string tag = releaseDocument.RootElement.GetProperty("tag_name").GetString() ?? string.Empty;
            if (!int.TryParse(tag.Trim().TrimStart('v', 'V'), NumberStyles.None, CultureInfo.InvariantCulture, out int releaseVersion) ||
                releaseVersion != remoteVersion)
            {
                throw new InvalidDataException($"version.json reports V{remoteVersion}, but latest release tag is '{tag}'.");
            }

            Log($"Update V{currentVersion} -> V{remoteVersion} is available.");
            if (mode == RunMode.CheckOnly)
            {
                return 10;
            }

            string updaterPath = Path.Combine(ApplicationDirectory, "Updater.exe");
            await EnsureUpdaterAsync(client, updaterPath, expectedUpdaterHash);
            if (mode == RunMode.PrepareOnly)
            {
                return 10;
            }

            StartOfficialUpdater(updaterPath);
            return 10;
        }
        catch (Exception exception)
        {
            Log(exception.ToString());
            if (mode == RunMode.Startup) StartFanControl();
            return 1;
        }
    }

    private static HttpClient CreateHttpClient(int timeoutSeconds)
    {
        HttpClient client = new()
        {
            Timeout = TimeSpan.FromSeconds(Math.Clamp(timeoutSeconds, 5, 120))
        };
        client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("FanControl.AutoUpdater/1.0");
        return client;
    }

    private static async Task<JsonDocument> GetJsonAsync(HttpClient client, string endpoint)
    {
        if (TryGetLocalPath(endpoint, out string? localPath))
        {
            await using FileStream stream = File.OpenRead(localPath!);
            return await JsonDocument.ParseAsync(stream);
        }

        await using Stream remoteStream = await client.GetStreamAsync(endpoint);
        return await JsonDocument.ParseAsync(remoteStream);
    }

    private static async Task EnsureUpdaterAsync(HttpClient client, string updaterPath, string expectedHash)
    {
        if (File.Exists(updaterPath) && HashMatches(updaterPath, expectedHash))
        {
            return;
        }

        string temporaryPath = updaterPath + ".download";
        try
        {
            string endpoint = GetEndpoint("FANCONTROL_AUTOUPDATE_UPDATER_URL", UpdaterUrl);
            if (TryGetLocalPath(endpoint, out string? localPath))
            {
                File.Copy(localPath!, temporaryPath, overwrite: true);
            }
            else
            {
                await using Stream source = await client.GetStreamAsync(endpoint);
                await using FileStream destination = File.Create(temporaryPath);
                await source.CopyToAsync(destination);
            }

            if (!HashMatches(temporaryPath, expectedHash))
            {
                throw new InvalidDataException("Downloaded Updater.exe checksum does not match version.json.");
            }

            File.Move(temporaryPath, updaterPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
    }

    private static bool HashMatches(string path, string expectedHash)
    {
        using FileStream stream = File.OpenRead(path);
        string actualHash = Convert.ToHexString(MD5.HashData(stream));
        return actualHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
    }

    private static int GetCurrentVersion()
    {
        string fanControlPath = Path.Combine(ApplicationDirectory, "FanControl.exe");
        if (!File.Exists(fanControlPath))
        {
            throw new FileNotFoundException("FanControl.exe was not found next to the auto updater.", fanControlPath);
        }

        return FileVersionInfo.GetVersionInfo(fanControlPath).FileMajorPart;
    }

    private static void StartOfficialUpdater(string updaterPath)
    {
        _ = Process.Start(new ProcessStartInfo
        {
            FileName = updaterPath,
            WorkingDirectory = ApplicationDirectory,
            Arguments = "-v 10",
            UseShellExecute = true,
            WindowStyle = ProcessWindowStyle.Hidden
        }) ?? throw new InvalidOperationException("Updater.exe did not start.");

        Log("Official Updater.exe started in hidden mode.");
    }

    private static void StartFanControl()
    {
        string fanControlPath = Path.Combine(ApplicationDirectory, "FanControl.exe");
        Process.Start(new ProcessStartInfo
        {
            FileName = fanControlPath,
            WorkingDirectory = ApplicationDirectory,
            UseShellExecute = true
        });
    }

    private static bool SetStartupTask(bool enabled)
    {
        string executable = Environment.ProcessPath ?? throw new InvalidOperationException("Cannot determine executable path.");
        string fanControlPath = Path.Combine(ApplicationDirectory, "FanControl.exe");
        string actionExecutable = enabled ? executable : fanControlPath;
        string actionArguments = enabled ? "--startup" : string.Empty;

        const string script = """
            $ErrorActionPreference = 'Stop'
            $actionParameters = @{
                Execute = $env:FANCONTROL_TASK_EXECUTABLE
                WorkingDirectory = $env:FANCONTROL_TASK_WORKING_DIRECTORY
            }
            if ($env:FANCONTROL_TASK_ARGUMENTS) {
                $actionParameters.Argument = $env:FANCONTROL_TASK_ARGUMENTS
            }
            $action = New-ScheduledTaskAction @actionParameters
            $task = Get-ScheduledTask -TaskName $env:FANCONTROL_TASK_NAME -ErrorAction SilentlyContinue
            if ($null -ne $task) {
                Set-ScheduledTask -TaskName $env:FANCONTROL_TASK_NAME -Action $action | Out-Null
            } else {
                $identity = [Security.Principal.WindowsIdentity]::GetCurrent().Name
                $trigger = New-ScheduledTaskTrigger -AtLogOn -User $identity
                $principal = New-ScheduledTaskPrincipal -UserId $identity -LogonType Interactive -RunLevel Highest
                Register-ScheduledTask -TaskName $env:FANCONTROL_TASK_NAME -Action $action -Trigger $trigger -Principal $principal -Description 'Start FanControl at startup' | Out-Null
            }
            """;

        return RunPowerShell(
            script,
            new Dictionary<string, string>
            {
                ["FANCONTROL_TASK_NAME"] = TaskName,
                ["FANCONTROL_TASK_EXECUTABLE"] = actionExecutable,
                ["FANCONTROL_TASK_ARGUMENTS"] = actionArguments,
                ["FANCONTROL_TASK_WORKING_DIRECTORY"] = ApplicationDirectory
            }) == 0;
    }

    private static int RunPowerShell(string script, IReadOnlyDictionary<string, string> environment)
    {
        string encodedCommand = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        ProcessStartInfo startInfo = new("powershell.exe")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-EncodedCommand");
        startInfo.ArgumentList.Add(encodedCommand);
        foreach ((string name, string value) in environment) startInfo.Environment[name] = value;

        using Process process = Process.Start(startInfo) ?? throw new InvalidOperationException("Could not start powershell.exe.");
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(30_000))
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
            Log("Task Scheduler configuration timed out after 30 seconds.");
            return 1;
        }

        Task.WaitAll(outputTask, errorTask);
        string output = outputTask.Result.Trim();
        string error = errorTask.Result.Trim();
        if (process.ExitCode != 0) Log($"Task Scheduler configuration exited with {process.ExitCode}: {error}");
        else if (output.Length > 0) Log($"Task Scheduler configuration: {output}");
        return process.ExitCode;
    }

    private static AutoUpdateSettings LoadSettings()
    {
        try
        {
            if (!File.Exists(SettingsPath)) return new AutoUpdateSettings(true, 20);
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(SettingsPath));
            bool enabled = document.RootElement.TryGetProperty("Enabled", out JsonElement enabledElement) && enabledElement.GetBoolean();
            int timeout = document.RootElement.TryGetProperty("CheckTimeoutSeconds", out JsonElement timeoutElement)
                ? timeoutElement.GetInt32()
                : 20;
            return new AutoUpdateSettings(enabled, timeout);
        }
        catch (Exception exception)
        {
            Log($"Settings error: {exception.Message}");
            return new AutoUpdateSettings(true, 20);
        }
    }

    private static void SaveSettings(AutoUpdateSettings settings)
    {
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsPath, json);
    }

    private static string GetEndpoint(string environmentVariable, string fallback) =>
        Environment.GetEnvironmentVariable(environmentVariable) is { Length: > 0 } value ? value : fallback;

    private static bool TryGetLocalPath(string endpoint, out string? localPath)
    {
        if (Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri) && uri.IsFile)
        {
            localPath = uri.LocalPath;
            return true;
        }

        if (Path.IsPathFullyQualified(endpoint))
        {
            localPath = endpoint;
            return true;
        }

        localPath = null;
        return false;
    }

    private static void Log(string message)
    {
        try
        {
            File.AppendAllText(LogPath, $"{DateTimeOffset.Now:O} {message}{Environment.NewLine}");
        }
        catch
        {
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int MessageBoxW(IntPtr hWnd, string text, string caption, uint type);

    private enum RunMode
    {
        Startup,
        CheckOnly,
        PrepareOnly
    }

    private sealed record AutoUpdateSettings(bool Enabled, int CheckTimeoutSeconds);
}
