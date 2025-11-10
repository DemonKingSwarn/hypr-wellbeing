# hypr-wellbeing

![](./.assets/show.png)

## Installation

### AUR

```sh
paru -S hypr-wellbeing-bin
```

### Windows

Make sure [Scoop](https://scoop.sh/) is installed.

```sh
scoop bucket add demon-apps https://github.com/DemonKingSwarn/flix-cli-bucket.git
scoop install hypr-wellbeing
```

### Github Releases

You can download it from [Releases](https://github.com/DemonKingSwarn/hypr-wellbeing/releases)

## Usage

### Monitor Mode

**Linux**:

```sh
hypr-wellbeing -d &> /dev/null &
```

**Windows**:

```sh
hypr-wellbeing.exe -d > $null 2>&1
```

### Show Stats

```sh
hypr-wellbeing --show
```

### Show Weekly Stats

```sh
hypr-wellbeing --weekly
```

## To-Do 

- [] Monthly view

## Special Thanks

- [Watcher](https://github.com/Waishnav/Watcher) (Similar to hyprwatch but only for X11) - for showing how to calculate the timings of the app
