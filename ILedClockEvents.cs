using System;
using System.Runtime.InteropServices;

namespace LedClockCOM
{
    /// <summary>
    /// COM event interface for the LED Clock Control.
    /// This defines all events that can be fired to Clarion.
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("5722DA49-B315-4313-9122-6DE3C816A60D")]
    public interface ILedClockEvents
    {
        /// <summary>
        /// Fired every time the clock ticks (once per second).
        /// </summary>
        /// <param name="currentTime">The current time as a formatted string</param>
        [DispId(1)]
        void ClockTick(string currentTime);

        /// <summary>
        /// Fired when the clock is started.
        /// </summary>
        [DispId(2)]
        void ClockStarted();

        /// <summary>
        /// Fired when the clock is stopped.
        /// </summary>
        [DispId(3)]
        void ClockStopped();
    }
}
