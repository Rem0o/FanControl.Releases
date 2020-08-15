# FanControl.Releases

[<img src="https://www.paypalobjects.com/webstatic/mktg/logo/pp_cc_mark_37x23.jpg">](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=N4JPSTUQHRJM8&currency_code=USD&source=url&item_name=Fan+Control)
##
This is the release repository for Fan Control, a highly customizable fan controlling software for Windows.

Libraries used:
* https://github.com/LibreHardwareMonitor/LibreHardwareMonitor
* https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit
* https://github.com/falahati/NvAPIWrapper

## New

* Support for newer Nvidia RTX cards with NvAPIWrapper
* Support for external sensors with .sensor files (see Example.sensor in your application folder)

## Installation

1. [Download the latest archive](/FanControl.zip?raw=true)
2. Extract to the desired installation folder
3. [Optional] Run the Register_Startup_Task.cmd script
4. Start FanControl.exe

## Issues

* I am not the main developer for the driver portion of this software. Any issue regarding hardware compatibility should be submitted to LibreHardwareMonitor's repository.
* Please only open issues for the software itself, UI, feature request and so on.

![Fan Control](Images/MainUI.png)

## Features

* Save, edit and load multiple profiles
* Multiple temperature sources ( CPU, GPU, motherboard, hard drives, ".sensor" file )
* Custom fan curves
* Fine tune the fan control response with steps, activation %, response time and hysteresis
* Mix different curves and sensors together
* Modern, dashboard-style UI
* Works as a background application with a customizable tray icon
* Create custom external temperature sensors with *.sensor files.
* And more!

## Fan curve types

* Linear : Temperature based linear function
* Graph : Temperature based custom curve
* Mix : Use two different curves and apply a mix function (Max, Sum)
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
