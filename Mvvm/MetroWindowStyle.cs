using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using ReportingServer1.DB;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace IVSAnalysisResultPlayer
{
    partial class MetroWindowStyle
    {
        bool restoreIfMove = false;
        private double fullScreenHeight;
        private double maxScreenHeight;
        private bool bMaxMized = true;
        private void PART_TITLEBAR_MouseMove(object sender, MouseEventArgs e)
        {
            if (restoreIfMove && e.LeftButton == MouseButtonState.Pressed)
            {
                restoreIfMove = false;

                Border border = sender as Border;
                Window window = border.GetVisualParent<Window>();

                var mouseX = e.GetPosition(window).X;
                var width = window.RestoreBounds.Width;
                var x = mouseX - width / 2;
                IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(mainWindowPtr);

                var screen = currentMonitor.WorkingArea;
                if (x < 0)
                {
                    x = 0;
                }
                else
                    if (x + width > screen.Width)
                    {
                        x = screen.Width - width;
                    }
                window.WindowState = WindowState.Normal;
                window.Show();
                window.Left = screen.X + x;
                window.Top = 0;
                window.DragMove();
            }
        }

        void PART_TITLEBAR_Loaded(object sender, RoutedEventArgs e)
        {
            Border border = sender as Border;
            Window window = border.GetVisualParent<Window>();
            if (window != null)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                    System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(mainWindowPtr);
                    window.MaxHeight = currentMonitor.WorkingArea.Height;// currentMonitor.WorkingArea.Height + window.Height - currentMonitor.Bounds.Height;
                    //window.WindowState = WindowState.Maximized;
                }
            }
        }
        private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Border border = sender as Border;
                Window window = border.GetVisualParent<Window>();
                if (window != null)
                {
                    if (window.WindowState == WindowState.Maximized)
                    {
                        window.WindowState = WindowState.Normal;
                        bMaxMized = false;
                    }
                    else
                    {
                        IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                        System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(mainWindowPtr);
                        window.MaxHeight = currentMonitor.WorkingArea.Height;
                        window.WindowState = WindowState.Maximized;
                        bMaxMized = true;
                    }
                }
            }
            else
            {
                Border button = sender as Border;
                Window window = button.GetVisualParent<Window>();
                if (window != null)
                {
                    if (window.WindowState == WindowState.Maximized)
                    {
                        restoreIfMove = true;
                    }
                    window.DragMove();
                }
            }
        }

        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.MainWindow.Close();
            Button button = sender as Button;
            Window window = button.GetVisualParent<Window>();
            if (window != null)
            {
                window.Close();
            }
        }

        private void PART_MAXIMIZE_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Window window = button.GetVisualParent<Window>();
            //maxScreenHeight = MonitorHelper.GetCurrentMonitor(mainWindowPtr).WorkingArea.Height * Dpi.HeightFactor;
            //maxScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            //System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
            /*
            if (bMaxMized)
            {
                bMaxMized = false;
                window.WindowState = WindowState.Normal;
            }
            else {
                bMaxMized = true;
                IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(mainWindowPtr);
                window.WindowState = WindowState.Maximized;
            }
            */

            if(IsMaximize(window) == false)
            {
                IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(mainWindowPtr);
                window.MaxHeight = currentMonitor.WorkingArea.Height;
                window.WindowState = WindowState.Maximized;
            }
            else 
            {
                window.WindowState = WindowState.Normal;
                
            }

        }
        private bool IsMaximize(Window window)
        {
            IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
            System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(mainWindowPtr);
            return window.MaxHeight == currentMonitor.WorkingArea.Height && window.Width == currentMonitor.Bounds.Width;
        }
        private bool IsFullScreen(Window window)
        {
            IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
            System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(mainWindowPtr);
            return window.Height == currentMonitor.Bounds.Height && window.Width == currentMonitor.Bounds.Width;
        }
        private void FillFullScreen(Window window)
        {
            IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
            System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(mainWindowPtr);

            window.MaxWidth = double.PositiveInfinity;
            window.MaxHeight = currentMonitor.Bounds.Height;
            //window.Hide();
            window.WindowState = WindowState.Maximized;
            //window.Show();

            Win32Helper.MoveWindow(mainWindowPtr, 0, 0, (int)window.Width, currentMonitor.Bounds.Height, true);
            Win32Helper.SetForegroundWindow(mainWindowPtr);
        }

        private void FillWorkArea(Window window)
        {
            IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
            System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(mainWindowPtr);

            window.MaxHeight = currentMonitor.WorkingArea.Height;
            window.MaxWidth = currentMonitor.WorkingArea.Width;
            window.WindowState = WindowState.Maximized;

            Win32Helper.MoveWindow(mainWindowPtr, 0, 0, currentMonitor.Bounds.Width, currentMonitor.WorkingArea.Height, true);
            Win32Helper.SetForegroundWindow(mainWindowPtr);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void PART_FULLSCREEN_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Window window = button.GetVisualParent<Window>();

            IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
            //fullScreenHeight = MonitorHelper.GetCurrentMonitor(mainWindowPtr).Bounds.Height * Dpi.HeightFactor;
            System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(mainWindowPtr);

            if (IsFullScreen(window) == false)
            {
                if (IsMaximize(window) == false)
                {

                    FillFullScreen(window);
                }
                else
                {
                    FillFullScreen(window);

                    //Win32Helper.MoveWindow(mainWindowPtr, 0, 0, (int)window.Width, (int)window.Height, true);
                    //Win32Helper.SetForegroundWindow(mainWindowPtr);
                }
            }
            else
            {
                window.WindowState = WindowState.Normal;
            }
        }

        private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Window window = button.GetVisualParent<Window>();

            window.WindowState = WindowState.Minimized;
        }

        private void THUMB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb button = sender as Thumb;
            Window window = button.GetVisualParent<Window>();
            if (IsMaximize(window))
            {
                return;
            }
            var str = (string)(sender as Thumb).Tag;

            if (str.Contains("T"))
            {
                var diff = window.ActualHeight - e.VerticalChange;
                if (diff > window.MinHeight && diff < window.MaxHeight)
                {
                    window.Height = diff;
                    window.Top = window.Top + e.VerticalChange;
                }
            }
            if (str.Contains("L"))
            {
                var diff = window.ActualWidth - e.HorizontalChange;
                if (diff > window.MinWidth && diff < window.MaxWidth)
                {
                    window.Width = diff;
                    window.Left = window.Left + e.HorizontalChange;
                }
            }
            if (str.Contains("B"))
            {
                window.Height = Math.Min(Math.Max(window.MinHeight, window.ActualHeight + e.VerticalChange), window.MaxHeight);
            }
            if (str.Contains("R"))
            {
                window.Width = Math.Min(Math.Max(window.MinWidth, window.ActualWidth + e.HorizontalChange), window.MaxWidth);
            }

            e.Handled = true;
        }
    }

    public static class Win32Helper
    {
        public enum ShowWindowCommands
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window 
            /// for the first time.
            /// </summary>
            Normal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>       
            ShowMaximized = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value 
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except 
            /// the window is not activated.
            /// </summary>
            ShowNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position. 
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level 
            /// window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is 
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position. 
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
            /// that owns the window is not responding. This flag should only be 
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);
    }

    public static class Dpi
    {
        public static double WidthFactor;
        public static double HeightFactor;
        private const double DEFAULT_DPI = 96;

        static Dpi()
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = (int)dpiXProperty.GetValue(null, null);
            var dpiY = (int)dpiYProperty.GetValue(null, null);

            WidthFactor =  DEFAULT_DPI / dpiX;
            HeightFactor = DEFAULT_DPI / dpiY;
        }
    }
}
