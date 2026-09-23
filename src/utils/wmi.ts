import { exec } from "child_process";

/**
 * Executes a WMI query and returns the raw result as an array of objects.
 * The function uses the built‑in `wmic` command to avoid extra native dependencies.
 *
 * @param query The WMI query string, e.g. "SELECT * FROM Win32_TemperatureProbe"
 * @returns Promise resolving to an array of result rows.
 */
export function wmiQuery<T = any>(query: string): Promise<T[]> {
  return new Promise((resolve, reject) => {
    // wmic returns CSV when /format:csv is used – we parse it manually.
    const cmd = `wmic ${query} /format:csv`;
    exec(cmd, { windowsHide: true }, (error, stdout, stderr) => {
      if (error) {
        reject(error);
        return;
      }
      if (stderr) {
        reject(new Error(stderr));
        return;
      }

      // First line is header with node name, skip it.
      const lines = stdout.trim().split(/\r?\n/);
      if (lines.length < 2) {
        resolve([]);
        return;
      }

      const header = lines[0].split(",").map((h) => h.trim());
      const data: T[] = [];

      for (let i = 1; i < lines.length; i++) {
        const values = lines[i].split(",").map((v) => v.trim());
        const row: any = {};
        for (let j = 0; j < header.length; j++) {
          row[header[j]] = values[j];
        }
        data.push(row);
      }

      resolve(data);
    });
  });
}

/**
 * Retrieves the current RPM of the AIO pump using the standard sensor name
 * used by most motherboard manufacturers (e.g., "AIO_PUMP").
 *
 * @returns Promise<number> RPM value, or 0 if not found / unparsable.
 */
export async function getPumpRpm(): Promise<number> {
  // The exact WMI class for fan sensors varies; we use Win32_Fan as a generic fallback.
  // Many OEMs expose a "Name" property that contains "Pump" or "AIO".
  const fans = await wmiQuery<any>("path Win32_Fan get Name,DesiredSpeed,CurrentSpeed");
  const pump = fans.find((f) => /pump|aio/i.test(f.Name ?? ""));
  if (!pump) return 0;

  const rpm = Number(pump.CurrentSpeed);
  return Number.isNaN(rpm) ? 0 : rpm;
}
