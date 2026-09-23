import express, { Request, Response } from "express";
import { PumpWatchdog } from "./monitor/pumpWatchdog.js";

const app = express();
const port = process.env.PORT ? Number(process.env.PORT) : 3000;

// Create a single watchdog instance for the lifetime of the process.
const watchdog = new PumpWatchdog({
  // Adjust these values if your Fan Control installation differs.
  fanControlExe: `${process.env.ProgramFiles}\\FanControl\\FanControl.exe`,
  resetArgs: ["/apply"]
});

app.use(express.json());

app.get("/status", (_req: Request, res: Response) => {
  res.json({
    running: watchdog.isRunning()
  });
});

app.post("/start", (_req: Request, res: Response) => {
  watchdog.start();
  res.json({ message: "Pump watchdog started." });
});

app.post("/stop", (_req: Request, res: Response) => {
  watchdog.stop();
  res.json({ message: "Pump watchdog stopped." });
});

app.listen(port, () => {
  console.log(`Pump Watchdog API listening at http://localhost:${port}`);
});
