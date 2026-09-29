<p>
  <a href="README.md"><img alt="English" src="https://img.shields.io/badge/English-9CA3AF?style=for-the-badge"></a>
  <a href="README_zh-CN.md"><img alt="简体中文" src="https://img.shields.io/badge/%E7%AE%80%E4%BD%93%E4%B8%AD%E6%96%87-2563EB?style=for-the-badge"></a>
</p>

<table>
  <tr>
    <td width="72"><img src="assets/crosshair.png" width="64" alt="MiddleScroll"></td>
    <td><h1>MiddleScroll</h1></td>
  </tr>
</table>

把浏览器里的中键自动滚动带到任意窗口。按一下滚轮出现准星，移动鼠标即可滚动；再按一次滚轮或按 Esc 退出。

开启后，本工具优先接管中键，优先级高于其他软件，避免两边同时滚动造成冲突。

尤其适合滚轮不灵活，或觉得来回拨动滚轮比较费劲的朋友。

## 行为

- 速度曲线与 Chrome / Edge 一致：准星周围 15 像素不滚动，超出后按距离的 2.2 次方加速。
- 滚动锁定在按下滚轮时的窗口上，之后鼠标移开也不会换目标。
- 滚动过程中忽略物理滚轮，避免手指带动滚轮时页面突然跳一下。
- 中键在所有软件中由本工具优先处理。无边框全屏（游戏、视频）除外。
- 左右移动可以横向滚动。

## 使用

1. 运行 `MiddleScroll.exe`。程序停在通知区域，不需要安装。
2. 把指针放在要滚动的内容上，按一下滚轮。
3. 上下移动鼠标。离准星越远，滚得越快。
4. 再按一次滚轮，或按 Esc 退出。

右键托盘图标可开关「全屏时暂停」和「开机自动启动」，也可选择「退出」。

未开启开机自启时，打开开始菜单，搜索 MiddleScroll，点击即可运行。

## Quick Start

在 PowerShell 中运行下面的命令，将自动下载、解压并启动 MiddleScroll。启动后，按一下滚轮即可激活。

```powershell
curl.exe -fL -H "Accept: application/vnd.github.raw" -o "$pwd\MiddleScroll.zip" "https://api.github.com/repos/LuoRuomu/MiddleScroll/contents/MiddleScroll.zip"; if ($LASTEXITCODE -ne 0) { throw "download failed" }; Unblock-File -LiteralPath "$pwd\MiddleScroll.zip"; Expand-Archive -LiteralPath "$pwd\MiddleScroll.zip" -DestinationPath "$pwd" -Force; Unblock-File -LiteralPath "$pwd\MiddleScroll.exe"; $sc = (New-Object -ComObject WScript.Shell).CreateShortcut("$env:APPDATA\Microsoft\Windows\Start Menu\Programs\MiddleScroll.lnk"); $sc.TargetPath = "$pwd\MiddleScroll.exe"; $sc.WorkingDirectory = "$pwd"; $sc.Save(); Start-Process -FilePath "$pwd\MiddleScroll.exe"
```

## 许可

[MIT](LICENSE)
