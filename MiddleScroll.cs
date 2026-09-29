// MiddleScroll — global middle-click autoscroll for Windows.
// Click the wheel to arm, move the pointer to scroll, click again or Esc to stop.

using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MiddleScroll
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Native.SetProcessDPIAware();
            bool created;
            using (new Mutex(true, @"Local\MiddleScroll.Global.v1", out created))
            {
                if (!created)
                    return;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TrayApp());
            }
        }
    }

    internal sealed class TrayApp : ApplicationContext
    {
        // Chrome / Edge middle-click curve, in device-independent pixels:
        // no motion inside 15px, then 0.008 * distance^2.2 pixels per second.
        private const double DeadZoneDip = 15.0;
        private const double SpeedMult = 0.008;
        private const double SpeedExp = 2.2;
        private const double MaxPxPerSec = 30000.0;
        private const double PixelsPerNotch = 55.0;
        private const int WheelNotch = 120;
        private const int TickMs = 8;

        private readonly NotifyIcon tray;
        private readonly Crosshair crosshair;
        private readonly System.Windows.Forms.Timer timer;
        private readonly MenuItem pauseFullscreenItem;
        private readonly MenuItem autostartItem;

        private Native.LowLevelProc mouseProc;
        private Native.LowLevelProc keyProc;
        private IntPtr mouseHook;
        private IntPtr keyHook;

        private bool active;
        private Point anchor;
        private IntPtr targetWindow;
        private double wheelRemainder;
        private double hWheelRemainder;
        private bool pauseInFullscreen = true;
        private readonly double dpiScale;

        public TrayApp()
        {
            dpiScale = Native.GetDpiScale();
            crosshair = new Crosshair();
            timer = new System.Windows.Forms.Timer { Interval = TickMs };
            timer.Tick += OnTick;

            pauseFullscreenItem = new MenuItem("全屏时暂停", (s, e) =>
            {
                pauseInFullscreen = !pauseInFullscreen;
                pauseFullscreenItem.Checked = pauseInFullscreen;
            })
            { Checked = true };

            autostartItem = new MenuItem("开机自动启动", (s, e) =>
            {
                bool enable = !IsAutostartEnabled();
                SetAutostart(enable);
                autostartItem.Checked = enable;
            })
            { Checked = IsAutostartEnabled() };

            var menu = new ContextMenu();
            menu.MenuItems.Add(pauseFullscreenItem);
            menu.MenuItems.Add(autostartItem);
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(new MenuItem("退出", (s, e) => ExitThread()));

            tray = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                Visible = true,
                Text = "中键自动滚动：按一下滚轮，再移动鼠标",
                ContextMenu = menu
            };

            var handle = crosshair.Handle;
            if (handle == IntPtr.Zero)
                return;
            InstallHooks();
        }

        private void InstallHooks()
        {
            mouseProc = MouseHook;
            keyProc = KeyHook;
            IntPtr module = Native.GetModuleHandle(null);
            mouseHook = Native.SetWindowsHookEx(Native.WH_MOUSE_LL, mouseProc, module, 0);
            keyHook = Native.SetWindowsHookEx(Native.WH_KEYBOARD_LL, keyProc, module, 0);
        }

        private IntPtr MouseHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int msg = wParam.ToInt32();

                // Ignore the physical wheel while armed. A bump of the wheel
                // would otherwise stack on the gesture and jump the page.
                // Injected events are kept so the SendInput fallback still lands.
                if (active && (msg == Native.WM_MOUSEWHEEL || msg == Native.WM_MOUSEHWHEEL) && !Native.IsInjected(lParam))
                    return (IntPtr)1;

                if (msg == Native.WM_MBUTTONDOWN || msg == Native.WM_MBUTTONUP)
                {
                    if (pauseInFullscreen && !active && Native.IsForegroundFullscreen())
                        return Native.CallNextHookEx(mouseHook, nCode, wParam, lParam);

                    if (msg == Native.WM_MBUTTONUP)
                        crosshair.BeginInvoke(new Action(Toggle));
                    return (IntPtr)1;
                }
            }
            return Native.CallNextHookEx(mouseHook, nCode, wParam, lParam);
        }

        private IntPtr KeyHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && active && wParam.ToInt32() == Native.WM_KEYDOWN)
            {
                int vk = Marshal.ReadInt32(lParam);
                if (vk == Native.VK_ESCAPE)
                {
                    crosshair.BeginInvoke(new Action(Deactivate));
                    return (IntPtr)1;
                }
            }
            return Native.CallNextHookEx(keyHook, nCode, wParam, lParam);
        }

        private void Toggle()
        {
            if (active)
                Deactivate();
            else
                Activate();
        }

        private void Activate()
        {
            Native.POINT pt;
            Native.GetCursorPos(out pt);
            anchor = new Point(pt.X, pt.Y);
            // Latch the control under the click. Later pointer motion must not retarget.
            targetWindow = Native.DeepestWindowFromPoint(pt);
            wheelRemainder = 0;
            hWheelRemainder = 0;
            active = true;
            crosshair.Place(anchor);
            timer.Start();
        }

        private void Deactivate()
        {
            active = false;
            timer.Stop();
            crosshair.Hide();
            targetWindow = IntPtr.Zero;
            wheelRemainder = 0;
            hWheelRemainder = 0;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!active)
                return;

            Native.POINT pt;
            Native.GetCursorPos(out pt);
            wheelRemainder += AxisWheelPerTick(pt.Y - anchor.Y, true);
            hWheelRemainder += AxisWheelPerTick(pt.X - anchor.X, false);
            FlushWheel(ref wheelRemainder, false);
            FlushWheel(ref hWheelRemainder, true);
        }

        // Emit the integer part each tick. Holding for a full notch makes motion stutter.
        private void FlushWheel(ref double remainder, bool horizontal)
        {
            int step = (int)remainder;
            if (step == 0)
                return;

            remainder -= step;
            if (!Native.PostWheel(targetWindow, step, horizontal, anchor))
                Native.SendWheel(step, horizontal);
        }

        private double AxisWheelPerTick(int physicalDelta, bool vertical)
        {
            double distance = Math.Abs(physicalDelta / dpiScale);
            if (distance <= DeadZoneDip)
                return 0;

            double pxPerSec = SpeedMult * Math.Pow(distance, SpeedExp);
            if (pxPerSec > MaxPxPerSec)
                pxPerSec = MaxPxPerSec;

            // Down and right match the browser: content follows the pointer.
            double wheelPerTick = pxPerSec / PixelsPerNotch * WheelNotch * (TickMs / 1000.0);
            bool negative = vertical ? physicalDelta > 0 : physicalDelta < 0;
            return negative ? -wheelPerTick : wheelPerTick;
        }

        private static bool IsAutostartEnabled()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
            {
                return key != null && key.GetValue("MiddleScroll") != null;
            }
        }

        private static void SetAutostart(bool enable)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (key == null)
                    return;
                if (enable)
                    key.SetValue("MiddleScroll", "\"" + Application.ExecutablePath + "\"");
                else
                    key.DeleteValue("MiddleScroll", false);
            }
        }

        protected override void ExitThreadCore()
        {
            timer.Stop();
            if (mouseHook != IntPtr.Zero)
                Native.UnhookWindowsHookEx(mouseHook);
            if (keyHook != IntPtr.Zero)
                Native.UnhookWindowsHookEx(keyHook);
            tray.Visible = false;
            tray.Dispose();
            crosshair.Dispose();
            base.ExitThreadCore();
        }
    }

    internal sealed class Crosshair : Form
    {
        private static readonly Image Mark = LoadMark();

        private static Image LoadMark()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MiddleScroll.crosshair.png");
            return stream == null ? null : new Bitmap(stream);
        }

        public Crosshair()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            Size = new Size(46, 46);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Tool window, click-through, no activation, topmost.
                cp.ExStyle |= 0x00000080 | 0x00000020 | 0x08000000 | 0x00000008;
                return cp;
            }
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public void Place(Point anchor)
        {
            Location = new Point(anchor.X - Width / 2, anchor.Y - Height / 2);
            if (!Visible)
                Show();
            else
                Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Magenta);
            if (Mark == null)
                return;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImage(Mark, new Rectangle(0, 0, Width, Height));
        }
    }

    internal static class Native
    {
        public const int WH_MOUSE_LL = 14;
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_MOUSEHWHEEL = 0x020E;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int VK_ESCAPE = 0x1B;

        public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        // x64 INPUT places the mouse union at offset 8.
        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(8)]
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        private static extern uint GetDpiForSystem();

        public static double GetDpiScale()
        {
            uint dpi = GetDpiForSystem();
            if (dpi < 96)
                return 1.0;
            return dpi / 96.0;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll")]
        private static extern IntPtr ChildWindowFromPointEx(IntPtr hwndParent, POINT pt, uint flags);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public static IntPtr DeepestWindowFromPoint(POINT screen)
        {
            IntPtr hwnd = WindowFromPoint(screen);
            for (int i = 0; hwnd != IntPtr.Zero && i < 16; i++)
            {
                POINT client = screen;
                if (!ScreenToClient(hwnd, ref client))
                    break;
                // Skip invisible and transparent windows.
                IntPtr child = ChildWindowFromPointEx(hwnd, client, 0x0001 | 0x0004);
                if (child == IntPtr.Zero || child == hwnd)
                    break;
                hwnd = child;
            }
            return hwnd;
        }

        public static bool PostWheel(IntPtr hwnd, int delta, bool horizontal, Point screen)
        {
            if (hwnd == IntPtr.Zero)
                return false;
            uint msg = horizontal ? (uint)WM_MOUSEHWHEEL : (uint)WM_MOUSEWHEEL;
            IntPtr wParam = (IntPtr)(delta << 16);
            IntPtr lParam = (IntPtr)((screen.Y << 16) | (screen.X & 0xFFFF));
            return PostMessage(hwnd, msg, wParam, lParam);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // LLMHF_INJECTED is MSLLHOOKSTRUCT.flags.
        public static bool IsInjected(IntPtr hookStruct)
        {
            int flags = Marshal.ReadInt32(hookStruct, 12);
            return (flags & 0x1) != 0;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        public static void SendWheel(int delta, bool horizontal)
        {
            var input = new INPUT
            {
                type = 0,
                mi = new MOUSEINPUT
                {
                    mouseData = delta,
                    dwFlags = horizontal ? 0x01000u : 0x0800u
                }
            };
            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        // Borderless fullscreen is left alone. A maximized window is not fullscreen.
        public static bool IsForegroundFullscreen()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
                return false;

            RECT rect;
            if (!GetWindowRect(hwnd, out rect))
                return false;

            IntPtr monitor = MonitorFromWindow(hwnd, 2);
            var info = new MONITORINFO { cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO)) };
            if (!GetMonitorInfo(monitor, ref info))
                return false;

            bool covers =
                rect.Left <= info.rcMonitor.Left &&
                rect.Top <= info.rcMonitor.Top &&
                rect.Right >= info.rcMonitor.Right &&
                rect.Bottom >= info.rcMonitor.Bottom;
            if (!covers)
                return false;

            long style = IntPtr.Size == 8
                ? GetWindowLongPtr64(hwnd, -16).ToInt64()
                : GetWindowLong32(hwnd, -16);
            bool maximized = (style & 0x01000000) != 0;
            return !maximized;
        }
    }
}
