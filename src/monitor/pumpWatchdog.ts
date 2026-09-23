import { getPumpRpm } from "../utils/wmi.js";
import { exec } from "child_process";

/**
 * Configuration for the watchdog.
 */
export interface WatchdogConfig {
  /** Interval in milliseconds between RPM checks. */
  intervalMs: number;
  /** Path to the Fan Control executable. */
  fanControlExe: string;
  /** Arguments to force Fan Control to re‑apply its curve. */
  resetArgs: string[];
}

/**
 * Simple logger that prefixes messages with a timestamp.
 */
function log(message: string) {
  console.log(`[${new Date().toISOString()}] ${message}`);
}

/**
 * Executes Fan Control with the provided arguments.
 */
function resetFanControl(config: WatchdogConfig): Promise<void> {
  return new Promise((resolve, reject) => {
    const args = config.resetArgs.map((a) => `"${a}"`).join(" ");
    const cmd = `"${config.fanControlExe}" ${args}`;
    exec(cmd, { windowsHide: true }, (error, stdout, stderr) => {
      if (error) {
        log(`Fan Control reset failed: ${error.message}`);
        reject(error);
        return;
      }
      log(`Fan Control reset executed successfully.`);
      resolve();
    });
  });
}

/**
 * Core watchdog class.
 */
export class PumpWatchdog {
  private timer: NodeJS.Timeout | null = null;
  private readonly config: WatchdogConfig;

  constructor(config?: Partial<WatchdogConfig>) {
    // Default configuration – adjust paths if Fan Control is installed elsewhere.
    this.config = {
      intervalMs: 5000,
      fanControlExe: `${process.env.ProgramFiles}\\FanControl\\FanControl.exe`,
      resetArgs: ["/apply"], // Fan Control supports a silent apply switch.
      ...config,
    };
  }

  /** Starts the periodic monitoring loop. */
  start() {
    if (this.timer) {
      log("Watchdog already running.");
      return;
    }
    log("Starting Pump Watchdog.");
    this.timer = setInterval(() => this.checkAndRecover(), this.config.intervalMs);
    // Immediate first check.
    this.checkAndRecover();
  }

  /** Stops the monitoring loop. */
  stop() {
    if (!this.timer) {
      log("Watchdog is not running.");
      return;
    }
    clearInterval(this.timer);
    this.timer = null;
    log("Pump Watchdog stopped.");
  }

  /** Reads the RPM and triggers a reset if it is zero. */
  private async checkAndRecover() {
    try {
      const rpm = await getPumpRpm();
      log(`Current pump RPM: ${rpm}`);
      if (rpm === 0) {
        log("Pump RPM is 0 – attempting to restore Fan Control curve.");
        await resetFanControl(this.config);
      }
    } catch (err) {
      log(`Error during watchdog check: ${(err as Error).message}`);
    }
  }

  /** Returns true if the watchdog is active. */
  isRunning(): boolean {
    return this.timer !== null;
  }
}
