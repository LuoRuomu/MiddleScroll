# MiddleScroll

<p align="center">
  <img src="assets/crosshair.png" width="72" alt="Middle-click scroll mark">
</p>

Global middle-click autoscroll for Windows. Click the wheel once, move the pointer, and the window under that click scrolls. Click again or press Esc to stop.

Once it is running, it takes the middle button ahead of other applications, so their own scroll gestures do not fight with it.

把浏览器里的中键自动滚动带到任意窗口。按一下滚轮出现准星，移动鼠标即可滚动；再按一次滚轮或按 Esc 退出。

开启后，本工具优先接管中键，优先级高于其他软件，避免两边同时滚动造成冲突。

## Behavior

- Speed follows the Chrome / Edge curve: a 15px dead zone, then `0.008 * distance^2.2` pixels per second.
- Scrolling stays on the control you clicked, even if the pointer later moves away.
- While armed, hardware wheel input is ignored so a nudge of the wheel does not jump the page.
- The middle button is taken in every application. Borderless fullscreen is left alone.
- Horizontal movement scrolls sideways.

速度曲线与 Chrome / Edge 一致：准星周围 15 像素不滚动，超出后按距离的 2.2 次方加速。滚动锁定在按下滚轮时的窗口上。滚动过程中物理滚轮不再叠加。中键在所有软件中优先由本工具处理；无边框全屏（游戏、视频）除外。

## Use

1. Run `MiddleScroll.exe`. It stays in the notification area.
2. Click the wheel over the content you want to scroll.
3. Move up or down. Farther from the mark is faster.
4. Click the wheel again, or press Esc.

Right-click the tray icon to pause in fullscreen, toggle startup, or quit.

双击 `MiddleScroll.exe` 即可。程序停在通知区域，不需要安装。托盘菜单可以开关「全屏时暂停」和「开机自动启动」。

Windows 10 or 11, 64-bit. .NET Framework 4.x is already part of the system. Copy the exe to another PC and run it there; startup is per user and is not copied with the file.

## Build

```powershell
powershell -ExecutionPolicy Bypass -File build.ps1
```

The executable is written to `bin/MiddleScroll.exe`. The build uses the .NET Framework C# compiler (`csc.exe`) and targets x64.

## License

[MIT](LICENSE)
