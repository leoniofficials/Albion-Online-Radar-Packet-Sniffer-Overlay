using AlbionRadar.ViewModels;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace AlbionRadar
{
    public partial class MainWindow : Window
    {
        private TargetWindowTracker _tracker;

        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            SetTransparentWindow();

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

            _tracker = new TargetWindowTracker();
            _tracker.OnTargetWindowChanged += UpdateRadarOverlay;
            _tracker.Start();
        }

        private void SetTransparentWindow()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, -20); // GWL_EXSTYLE
            SetWindowLong(hwnd, -20, extendedStyle | 0x80000 | 0x20); // WS_EX_LAYERED | WS_EX_TRANSPARENT
        }

        private void UpdateRadarOverlay(TargetWindowTracker.RECT rect)
        {
            if (rect.Left == -1)
            {
                this.Hide();
                return;
            }

            this.Left = rect.Left;
            this.Top = rect.Top + 200;
            this.Width = 400;
            this.Height = 400;

            if (!this.IsVisible)
                this.Show();
        }

        private void MyNotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
                this.Show();
                this.Activate();
            }
            else
            {
                this.Hide();
            }
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
