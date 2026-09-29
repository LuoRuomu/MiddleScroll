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

Bring the browser's middle-click autoscroll to any window.

Once it is running, MiddleScroll takes the middle button ahead of other applications, so the two do not scroll at the same time.

It is for anyone whose scroll wheel is stiff, or who finds flicking it back and forth tiring.

## Behavior

- Speed follows the Chrome / Edge curve: a 15px dead zone, then `0.008 * distance^2.2` pixels per second.
- Scrolling stays on the control you clicked, even if the pointer later moves away.
- While armed, hardware wheel input is ignored so a nudge of the wheel does not jump the page.
- The middle button is taken in every application. Borderless fullscreen is left alone.
- Horizontal movement scrolls sideways.

## Use

1. Run `MiddleScroll.exe`, or open the Start menu, search for MiddleScroll, and click it to run.
2. Place the pointer over the content you want to scroll, then click the wheel.
3. Move the pointer up or down. Farther from the mark is faster.
4. Click the wheel again, or press Esc.

Right-click the tray icon to turn **Pause in fullscreen** or **Start with Windows** on or off, or choose **Exit**.

## Quick Start

Run the command below in PowerShell. It downloads, extracts, and starts MiddleScroll.

```powershell
curl.exe -fL -H "Accept: application/vnd.github.raw" -o "$pwd\MiddleScroll.zip" "https://api.github.com/repos/LuoRuomu/MiddleScroll/contents/MiddleScroll.zip"; if ($LASTEXITCODE -ne 0) { throw "download failed" }; Unblock-File -LiteralPath "$pwd\MiddleScroll.zip"; Expand-Archive -LiteralPath "$pwd\MiddleScroll.zip" -DestinationPath "$pwd" -Force; Unblock-File -LiteralPath "$pwd\MiddleScroll.exe"; Start-Process -FilePath "$pwd\MiddleScroll.exe"
```

## License

[MIT](LICENSE)
