using System;
using System.Runtime.InteropServices;

namespace LedClockCOM
{
    /// <summary>
    /// Main COM interface for the LED Clock Control.
    /// This defines all properties and methods that Clarion can call.
    /// </summary>
    [ComVisible(true)]
    [Guid("782FE39F-D366-4E8C-84D1-4C9C7BF254EE")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ILedClock
    {
        /// <summary>
        /// Gets or sets the LED foreground color as an integer (RGB).
        /// </summary>
        [DispId(1)]
        int LedColor { get; set; }

        /// <summary>
        /// Gets or sets the background color as an integer (RGB).
        /// </summary>
        [DispId(2)]
        int BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets whether to use 24-hour format (true) or 12-hour format (false).
        /// </summary>
        [DispId(3)]
        bool Use24HourFormat { get; set; }

        /// <summary>
        /// Gets or sets whether to show seconds.
        /// </summary>
        [DispId(4)]
        bool ShowSeconds { get; set; }

        /// <summary>
        /// Gets or sets whether the clock is running.
        /// </summary>
        [DispId(5)]
        bool IsRunning { get; set; }

        /// <summary>
        /// Gets the current time as a formatted string.
        /// </summary>
        [DispId(6)]
        string CurrentTime { get; }

        /// <summary>
        /// Starts the clock.
        /// </summary>
        [DispId(7)]
        void Start();

        /// <summary>
        /// Stops the clock.
        /// </summary>
        [DispId(8)]
        void Stop();

        /// <summary>
        /// Sets the LED color using RGB values.
        /// </summary>
        [DispId(9)]
        void SetLedColorRGB(int red, int green, int blue);

        /// <summary>
        /// Sets the background color using RGB values.
        /// </summary>
        [DispId(10)]
        void SetBackgroundColorRGB(int red, int green, int blue);

        /// <summary>
        /// Displays control name and version information in a MessageBox.
        /// </summary>
        [DispId(11)]
        void About();
    }
}
