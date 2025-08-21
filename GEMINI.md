# hypr-wellbeing

## Project Overview

`hypr-wellbeing` is a command-line tool for tracking application usage time on Linux, with a specific focus on the Hyprland and niri window managers. It helps users monitor their screen time and promote digital wellbeing by providing detailed statistics on application usage.

## Features

- **Time Tracking:** Monitors the active window and logs the time spent on each application.
- **Daily Reports:** Displays a summary of daily application usage, including total screen time and a breakdown of time spent on each application.
- **Data Storage:** Stores usage data in CSV files in the `~/.cache/hyprwatch/daily_data/` directory.
- **Cross-Desktop Support (Limited):** While primarily designed for Hyprland and niri, it may work on other Wayland-based desktops.
- **Analysis:** Includes logic for generating weekly reports and sorting data.

## Technology Stack

- **Language:** C#
- **Framework:** .NET 8
- **Platform:** Linux
- **Compilation:** Published as a self-contained AOT (Ahead-Of-Time) compiled application.

## File Structure

```
/home/swarn/dox/code/hypr-wellbeing/
├───.gitignore
├───hypr-wellbeing.csproj
├───LICENSE
├───Program.cs
├───README.md
├───.assets/
│   └───show.png
├───.git/...
├───bin/
│   ├───Debug/...
│   └───Release/...
├───obj/
│   ├───Debug/...
│   └───Release/...
└───src/
    ├───Analysis.cs
    ├───GetWindows.cs
    ├───GetWindowsv2.cs
    ├───TimeOperations.cs
    └───WatchLog.cs
```

## Usage

### Monitor Mode

To run the application in the background and monitor application usage, use the following command:

```sh
hypr-wellbeing -d &> /dev/null &
```

### Show Stats

To display the statistics for the current day, use the following command:

```sh
hypr-wellbeing --show
```

## Future Improvements

The following features are planned for future releases:

- Weekly view of application usage
- Monthly view of application usage
