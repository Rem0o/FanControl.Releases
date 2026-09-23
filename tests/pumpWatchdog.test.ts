import { PumpWatchdog } from "../src/monitor/pumpWatchdog";
import * as wmi from "../src/utils/wmi";

jest.mock("../src/utils/wmi");

describe("PumpWatchdog", () => {
  const mockGetPumpRpm = wmi.getPumpRpm as jest.MockedFunction<typeof wmi.getPumpRpm>;

  afterEach(() => {
    jest.clearAllMocks();
  });

  it("does not trigger reset when RPM is non‑zero", async () => {
    mockGetPumpRpm.mockResolvedValueOnce(1500);
    const watchdog = new PumpWatchdog({ intervalMs: 10, fanControlExe: "echo", resetArgs: [] });
    const spy = jest.spyOn(watchdog as any, "resetFanControl");
    watchdog.start();

    // Wait a couple of intervals.
    await new Promise((r) => setTimeout(r, 30));
    watchdog.stop();

    expect(spy).not.toHaveBeenCalled();
  });

  it("triggers reset when RPM drops to zero", async () => {
    mockGetPumpRpm.mockResolvedValueOnce(0);
    const watchdog = new PumpWatchdog({ intervalMs: 10, fanControlExe: "echo", resetArgs: [] });
    const spy = jest.spyOn(watchdog as any, "resetFanControl");
    watchdog.start();

    await new Promise((r) => setTimeout(r, 30));
    watchdog.stop();

    expect(spy).toHaveBeenCalled();
  });
});
