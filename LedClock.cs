using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LedClockCOM
{
    /// <summary>
    /// LED Clock Control - A simple digital clock with LED-style display.
    /// Features configurable colors and 12/24 hour format.
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("0E91E242-4BE2-4988-9C45-1C614C7B22D7")]
    [ComSourceInterfaces(typeof(ILedClockEvents))]
    [ProgId("LedClockCOM.LedClock")]
    public partial class LedClock : UserControl, ILedClock
    {
        #region Fields

        private Timer _timer;
        private Color _ledColor;
        private Color _backgroundColor;
        private bool _use24HourFormat;
        private bool _showSeconds;
        private bool _isRunning;
        private bool _controlsInitialized;
        private string _currentTime;

        #endregion

        #region COM Event Delegates

        public delegate void ClockTickDelegate(string currentTime);
        public delegate void ClockStartedDelegate();
        public delegate void ClockStoppedDelegate();

        #endregion

        #region COM Events

        public event ClockTickDelegate ClockTick;
        public event ClockStartedDelegate ClockStarted;
        public event ClockStoppedDelegate ClockStopped;

        #endregion

        #region Constructor

        public LedClock()
        {
            // Field initialization ONLY - NO child controls!
            _ledColor = Color.Lime;
            _backgroundColor = Color.Black;
            _use24HourFormat = false;
            _showSeconds = true;
            _isRunning = false;
            _controlsInitialized = false;
            _currentTime = string.Empty;

            // Size and DoubleBuffered are safe in constructor
            Size = new Size(280, 80);
            DoubleBuffered = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (_controlsInitialized)
                return;

            try
            {
                // NOW it's safe to set BackColor
                BackColor = _backgroundColor;

                // Create timer
                _timer = new Timer();
                _timer.Interval = 1000;
                _timer.Tick += Timer_Tick;

                // Update display immediately
                UpdateTime();

                // Auto-start the clock
                _timer.Start();
                _isRunning = true;

                _controlsInitialized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing LED Clock: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Timer Event

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
            Invalidate();
            RaiseClockTick(_currentTime);
        }

        private void UpdateTime()
        {
            DateTime now = DateTime.Now;

            if (_use24HourFormat)
            {
                _currentTime = _showSeconds
                    ? now.ToString("HH:mm:ss")
                    : now.ToString("HH:mm");
            }
            else
            {
                _currentTime = _showSeconds
                    ? now.ToString("hh:mm:ss tt")
                    : now.ToString("hh:mm tt");
            }
        }

        #endregion

        #region ILedClock Properties

        [ComVisible(true)]
        [Description("Gets or sets the LED foreground color as an integer (RGB).")]
        public int LedColor
        {
            get { return ColorToInt(_ledColor); }
            set
            {
                _ledColor = IntToColor(value);
                Invalidate();
            }
        }

        [ComVisible(true)]
        [Description("Gets or sets the background color as an integer (RGB).")]
        public int BackgroundColor
        {
            get { return ColorToInt(_backgroundColor); }
            set
            {
                _backgroundColor = IntToColor(value);
                BackColor = _backgroundColor;
                Invalidate();
            }
        }

        [ComVisible(true)]
        [Description("Gets or sets whether to use 24-hour format.")]
        public bool Use24HourFormat
        {
            get { return _use24HourFormat; }
            set
            {
                _use24HourFormat = value;
                UpdateTime();
                Invalidate();
            }
        }

        [ComVisible(true)]
        [Description("Gets or sets whether to show seconds.")]
        public bool ShowSeconds
        {
            get { return _showSeconds; }
            set
            {
                _showSeconds = value;
                UpdateTime();
                Invalidate();
            }
        }

        [ComVisible(true)]
        [Description("Gets or sets whether the clock is running.")]
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                if (value)
                    Start();
                else
                    Stop();
            }
        }

        [ComVisible(true)]
        [Description("Gets the current time as a formatted string.")]
        public string CurrentTime
        {
            get { return _currentTime ?? string.Empty; }
        }

        #endregion

        #region ILedClock Methods

        [ComVisible(true)]
        [Description("Starts the clock.")]
        public void Start()
        {
            try
            {
                if (_timer != null && !_isRunning)
                {
                    _timer.Start();
                    _isRunning = true;
                    UpdateTime();
                    Invalidate();
                    RaiseClockStarted();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting clock: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [ComVisible(true)]
        [Description("Stops the clock.")]
        public void Stop()
        {
            try
            {
                if (_timer != null && _isRunning)
                {
                    _timer.Stop();
                    _isRunning = false;
                    RaiseClockStopped();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping clock: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [ComVisible(true)]
        [Description("Sets the LED color using RGB values.")]
        public void SetLedColorRGB(int red, int green, int blue)
        {
            try
            {
                red = Math.Max(0, Math.Min(255, red));
                green = Math.Max(0, Math.Min(255, green));
                blue = Math.Max(0, Math.Min(255, blue));

                _ledColor = Color.FromArgb(red, green, blue);
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting LED color: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [ComVisible(true)]
        [Description("Sets the background color using RGB values.")]
        public void SetBackgroundColorRGB(int red, int green, int blue)
        {
            try
            {
                red = Math.Max(0, Math.Min(255, red));
                green = Math.Max(0, Math.Min(255, green));
                blue = Math.Max(0, Math.Min(255, blue));

                _backgroundColor = Color.FromArgb(red, green, blue);
                BackColor = _backgroundColor;
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting background color: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [ComVisible(true)]
        [Description("Shows control name and version information")]
        public void About()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var name = assembly.GetName().Name;
                var version = assembly.GetName().Version;
                var versionStr = $"{version.Major}.{version.Minor}.{version.Build}";

                MessageBox.Show(
                    $"{name}\nVersion: {versionStr}\n\nLED-style digital clock control for Clarion.",
                    "About",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch { }
        }

        #endregion

        #region Event Raising Methods

        private void RaiseClockTick(string currentTime)
        {
            if (ClockTick != null)
            {
                try
                {
                    ClockTick(currentTime);
                }
                catch { }
            }
        }

        private void RaiseClockStarted()
        {
            if (ClockStarted != null)
            {
                try
                {
                    ClockStarted();
                }
                catch { }
            }
        }

        private void RaiseClockStopped()
        {
            if (ClockStopped != null)
            {
                try
                {
                    ClockStopped();
                }
                catch { }
            }
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Draw background
            using (SolidBrush bgBrush = new SolidBrush(_backgroundColor))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            // Draw border (LED display style)
            using (Pen borderPen = new Pen(Color.FromArgb(60, 60, 60), 3))
            {
                g.DrawRectangle(borderPen, 1, 1, Width - 3, Height - 3);
            }

            // Inner border for depth effect
            using (Pen innerPen = new Pen(Color.FromArgb(40, 40, 40), 1))
            {
                g.DrawRectangle(innerPen, 4, 4, Width - 9, Height - 9);
            }

            // Draw LED-style time text
            string displayTime = _currentTime ?? DateTime.Now.ToString("hh:mm:ss tt");

            // Calculate font size based on control size
            float fontSize = Math.Min(Height * 0.5f, Width / (displayTime.Length * 0.7f));
            fontSize = Math.Max(12, fontSize);

            using (Font ledFont = new Font("Consolas", fontSize, FontStyle.Bold))
            using (SolidBrush ledBrush = new SolidBrush(_ledColor))
            {
                // Draw dim "8" segments for LED effect (background segments)
                using (SolidBrush dimBrush = new SolidBrush(Color.FromArgb(30, _ledColor)))
                {
                    string dimText = new string('8', displayTime.Replace(":", " ").Replace(" ", " ").Length);
                    // Skip dim effect for simplicity - just draw the time
                }

                // Measure and center the text
                SizeF textSize = g.MeasureString(displayTime, ledFont);
                float x = (Width - textSize.Width) / 2;
                float y = (Height - textSize.Height) / 2;

                // Draw glow effect
                using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(50, _ledColor)))
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        g.DrawString(displayTime, ledFont, glowBrush, x - i, y);
                        g.DrawString(displayTime, ledFont, glowBrush, x + i, y);
                        g.DrawString(displayTime, ledFont, glowBrush, x, y - i);
                        g.DrawString(displayTime, ledFont, glowBrush, x, y + i);
                    }
                }

                // Draw the main text
                g.DrawString(displayTime, ledFont, ledBrush, x, y);
            }
        }

        #endregion

        #region Helper Methods

        private int ColorToInt(Color color)
        {
            // Return as BGR for Clarion compatibility (Windows color format)
            return color.R | (color.G << 8) | (color.B << 16);
        }

        private Color IntToColor(int colorValue)
        {
            // Parse BGR format (Windows color format)
            int r = colorValue & 0xFF;
            int g = (colorValue >> 8) & 0xFF;
            int b = (colorValue >> 16) & 0xFF;
            return Color.FromArgb(r, g, b);
        }

        #endregion

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
