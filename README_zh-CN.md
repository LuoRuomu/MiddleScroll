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

右键托盘图标可以开关「全屏时暂停」和「开机自动启动」。

适用于 64 位 Windows 10 或 11，系统自带 .NET Framework 4.x。把 exe 复制到另一台电脑即可运行；开机启动是按用户保存的，不会随文件一起复制。

## 构建

```powershell
powershell -ExecutionPolicy Bypass -File build.ps1
```

可执行文件输出到 `bin/MiddleScroll.exe`。构建使用 .NET Framework 自带的 C# 编译器（`csc.exe`），目标平台为 x64。

## 许可

[MIT](LICENSE)
