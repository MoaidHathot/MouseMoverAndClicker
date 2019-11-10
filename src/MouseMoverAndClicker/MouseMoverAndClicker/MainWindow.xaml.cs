using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using FormsCursor = System.Windows.Forms.Cursor;

namespace MouseMoverAndClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer _timer;
        private bool _started;

        private int _delta = 50;
        private bool _allowClicks;
        private bool _randomRange;

        private readonly Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ToggleStartButton.Content = "Start";



            ToggleStartButton.Click += ToggleStartButtonOnClick;

        }

        private void ToggleStartButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (_started)
            {
                _timer?.Dispose();
                _timer = null;

                ToggleStartButton.Content = "Start";

                _started = false;
                return;
            }

            _timer = new Timer(delegate { Dispatcher.Invoke(Move); }, null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(10));
            ToggleStartButton.Content = "Stop";

            _started = true;
        }

        private void Move()
        {
            _delta *= -1;

            var (width, height) = (SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

            var (x, y) = !_randomRange ? (FormsCursor.Position.X + _delta, FormsCursor.Position.Y + _delta) : (_random.Next((int)width), _random.Next((int)height));

            FormsCursor.Position = new System.Drawing.Point(x, y);

            if (_allowClicks)
            {
                Click(FormsCursor.Position.X, FormsCursor.Position.Y);
            }
        }

        private void AllowClickToggle(object sender, RoutedEventArgs e)
        {
            _allowClicks = !_allowClicks;
        }

        private void RandomRangeToggle(object sender, RoutedEventArgs e)
        {
            _randomRange = !_randomRange;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public static void Click(int xpos, int ypos)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        public static void MoveMouse(int x, int y)
        {
            SetCursorPos(x, y);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
    }
}
