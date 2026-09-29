<p>
  <a href="README.md"><img alt="English" src="https://img.shields.io/badge/English-2563EB?style=for-the-badge"></a>
  <a href="README_zh-CN.md"><img alt="简体中文" src="https://img.shields.io/badge/%E7%AE%80%E4%BD%93%E4%B8%AD%E6%96%87-9CA3AF?style=for-the-badge"></a>
</p>

<table>
  <tr>
    <td width="72"><img src="assets/crosshair.png" width="64" alt="MiddleScroll"></td>
    <td><h1>MiddleScroll</h1></td>
  </tr>
</table>

Global middle-click autoscroll for Windows. Click the wheel once, move the pointer, and the window under that click scrolls. Click again or press Esc to stop.

Once it is running, MiddleScroll takes the middle button ahead of other applications, so the two do not scroll at the same time.

It is for anyone whose scroll wheel is stiff, or who finds flicking it back and forth tiring.

## Behavior

- Speed follows the Chrome / Edge curve: a 15px dead zone, then `0.008 * distance^2.2` pixels per second.
- Scrolling stays on the control you clicked, even if the pointer later moves away.
- While armed, hardware wheel input is ignored so a nudge of the wheel does not jump the page.
- The middle button is taken in every application. Borderless fullscreen is left alone.
- Horizontal movement scrolls sideways.

## Use

1. Run `MiddleScroll.exe`. It stays in the notification area.
2. Click the wheel over the content you want to scroll.
3. Move up or down. Farther from the mark is faster.
4. Click the wheel again, or press Esc.

Right-click the tray icon to pause in fullscreen, toggle startup, or quit.

Windows 10 or 11, 64-bit. .NET Framework 4.x is already part of the system. Copy the exe to another PC and run it there; startup is per user and is not copied with the file.

## Download

Run this in PowerShell to download `MiddleScroll.zip` and extract `MiddleScroll.exe`.

```powershell
curl.exe -L -o MiddleScroll.zip https://github.com/LuoRuomu/MiddleScroll/raw/main/MiddleScroll.zip; Expand-Archive .\MiddleScroll.zip -DestinationPath . -Force
```

## License

[MIT](LICENSE)
