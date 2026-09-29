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

Right-click the tray icon to turn **Pause in fullscreen** or **Start with Windows** on or off, or choose **Exit**.

If it does not start with Windows, open the Start menu, search for MiddleScroll, and click it to run.

## Quick Start

Run the command below in PowerShell. It downloads, extracts, and starts MiddleScroll. Click the wheel to activate it.

```powershell
curl.exe -fL -H "Accept: application/vnd.github.raw" -o "$pwd\MiddleScroll.zip" "https://api.github.com/repos/LuoRuomu/MiddleScroll/contents/MiddleScroll.zip"; if ($LASTEXITCODE -ne 0) { throw "download failed" }; Unblock-File -LiteralPath "$pwd\MiddleScroll.zip"; Expand-Archive -LiteralPath "$pwd\MiddleScroll.zip" -DestinationPath "$pwd" -Force; Unblock-File -LiteralPath "$pwd\MiddleScroll.exe"; $sc = (New-Object -ComObject WScript.Shell).CreateShortcut("$env:APPDATA\Microsoft\Windows\Start Menu\Programs\MiddleScroll.lnk"); $sc.TargetPath = "$pwd\MiddleScroll.exe"; $sc.WorkingDirectory = "$pwd"; $sc.Save(); Start-Process -FilePath "$pwd\MiddleScroll.exe"
```

## License

[MIT](LICENSE)
