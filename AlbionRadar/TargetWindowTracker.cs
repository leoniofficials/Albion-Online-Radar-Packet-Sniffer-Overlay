using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace AlbionRadar
{
    public class TargetWindowTracker
    {
        private const string TARGET_WINDOW_TITLE = "Albion Online Client";
        private DispatcherTimer _timer;

        public event Action<RECT> OnTargetWindowChanged;

        public TargetWindowTracker()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _timer.Tick += PollWindow;
        }

        public void Start() => _timer.Start();

        private void PollWindow(object? sender, EventArgs e)
        {
            IntPtr hwnd = FindWindow(null, TARGET_WINDOW_TITLE);

            if (hwnd == IntPtr.Zero || !IsWindowVisible(hwnd) || IsIconic(hwnd) || GetForegroundWindow() != hwnd)
            {
                OnTargetWindowChanged?.Invoke(new RECT { Left = -1 });
                return;
            }

            if (GetWindowRect(hwnd, out RECT rect))
            {
                OnTargetWindowChanged?.Invoke(rect);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }
    }
}
