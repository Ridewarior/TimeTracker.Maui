using System.Timers;
using Timer = System.Timers.Timer;

namespace TimeTracker.Maui.Services;

public class TimerService
{
    private readonly Timer _timer = new();
    private const int TimerInterval = 1000;
    private int _hours, _minutes, _seconds;

    public TimeSpan ElapsedTime { get; private set; } = TimeSpan.Zero;
    public bool Running { get; set; }

    public event EventHandler TimerElapsed; 

    public TimerService()
    {
        _timer.Interval = TimerInterval;
        _timer.Elapsed += CountEvent;
    }

    /// <summary>
    /// Starts the timer
    /// </summary>
    public void StartTimer()
    {
        if (ElapsedTime != TimeSpan.Zero || _hours > 0 || _minutes > 0 || _seconds > 0)
        {
            ResetTimer();
        }

        _timer.Start();
        Running = true;
    }

    /// <summary>
    /// Stops the timer
    /// </summary>
    public void StopTimer()
    {
        _timer.Stop();
        Running = false;
    }

    /// <summary>
    /// Gets the elapsed time between a set start and end time
    /// </summary>
    /// <param name="startTime">Start date</param>
    /// <param name="endTime">End date</param>
    /// <returns>Returns the time between the two dates as a TimeSpan</returns>
    public TimeSpan GetPresetTime(DateTime startTime, DateTime endTime)
    {
        var interval = endTime - startTime;
        return interval;
    }

    /// <summary>
    /// Resets Timer values
    /// </summary>
    private void ResetTimer()
    {
        _hours = 0;
        _minutes = 0;
        _seconds = 0;
        ElapsedTime = TimeSpan.Zero;
    }
    
    /// <summary>
    /// Timer Count Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CountEvent(object sender, ElapsedEventArgs e)
    {
        _seconds += 1;

        if (_seconds == 60)
        {
            _seconds = 0;
            _minutes += 1;
        }

        if (_minutes == 60)
        {
            _minutes = 0;
            _hours += 1;
        }

        ElapsedTime = new TimeSpan(_hours, _minutes, _seconds);
        TimerElapsed?.Invoke(this, EventArgs.Empty);
    }
}