# FanControl.Releases

[<img src="https://www.paypalobjects.com/webstatic/mktg/logo/pp_cc_mark_37x23.jpg">](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=N4JPSTUQHRJM8&currency_code=USD&source=url&item_name=Fan+Control)
##
This is the release repository for Fan Control, a highly customizable fan controlling software for Windows.


![Fan Control](Images/MainUI.png)

## New

* <b>Move graph points horizontally</b>
* <b>Reduced CPU usage by 50%</b>
* <b> -c or --config [json config file] command line arg </b>
* Increased max temp. to 120 C
* Replaced numerical comboboxes with text boxes
* Time average custom sensor ( Plus sign menu )
* File custom sensor interface ( Plus sign menu )
* Plugin system, see [Plugins wiki](https://github.com/Rem0o/FanControl.Releases/wiki/Plugins) and
  * https://github.com/Rem0o/FanControl.HWInfo
  * https://github.com/Rem0o/FanControl.DellPlugin

## Installation

1. [Download the latest archive](/FanControl.zip?raw=true)
2. Extract to the desired installation folder
3. Start FanControl.exe

## Issues and hardware compatibility

* I am not the main developer for the driver/backend portion of this software. Fan Control is basically a UI on top of existing hardware libraries. Any issue regarding hardware compatibility entirely depends on:
  * https://github.com/LibreHardwareMonitor/LibreHardwareMonitor
  * https://github.com/falahati/NvAPIWrapper
  
* Please only open issues for the software itself, UI, feature request and so on.
* If you do have a hardware compatibility request and you can provide a <b>working</b> sample of code that can be used in .NET, like with a [Plugin](https://github.com/Rem0o/FanControl.Releases/wiki/Plugins), then feel free to submit that.

## FAQ
* <b>Q</b>: There is no control cards / control cards are missing / control cards are not changing my fan speeds, what's the issue?
<br><b>A</b>: See section above.

* <b>Q</b>: How does <b>[FAN CURVE TYPE]</b> works and what does its parameters do?
<br><b>A</b>: Click on its card's icon at the top left, a dialog will tell you.
* <b>Q</b>: Does it run on my OS?
<br><b>A</b>: If your OS is Windows 10, yes.

## Features

* Save, edit and load multiple profiles
* Multiple temperature sources ( CPU, GPU, motherboard, hard drives, ".sensor" file )
* <b>Mix different curves and sensors together</b>
* Custom fan curves
* Manual control
* Fine tune the fan control response with steps, start %, stop %, response time and hysteresis
* Modern, dashboard-style UI
* Works as a background application with a customizable tray icon
* And more!

## Fan curve types

* Linear : Temperature based linear function
* Graph : Temperature based custom curve
* Target: Temperature based that holds speed until target temperature is reached
* Mix : Use two different curves and apply a mix function (Min, Max, Sum, Average)
* Sync : Sync to an existing control
* Flat: Set a fixed %

## Graph fan curve editor

* Add, remove and drag points arround the graph
* Copy and paste points from a graph to another
* Fine-tune the response with the hysteresis and response time parameters

![Fan Control](Images/GraphDialog.png)

## Theme editor

* Dark mode available

![Fan Control](Images/ColorsDialog.png)

## Libraries used:
* https://github.com/LibreHardwareMonitor/LibreHardwareMonitor
* https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit
* https://github.com/falahati/NvAPIWrapper
