using System.Timers;
using Timer = System.Timers.Timer;

namespace TimeTracker.Maui.Services;

public class TimerService
{
    private readonly Timer _perpetualTimer = new();
    private readonly Timer _timer = new();
    private const int TimerInterval = 1000;
    private int _perpetualHours, _perpetualMinutes, _perpetualSeconds;
    private int _hours, _minutes, _seconds;

    private TimeSpan _perpetualElapsedTime = TimeSpan.Zero;
    public TimeSpan ElapsedTime { get; private set; } = TimeSpan.Zero;

    public bool Running { get; set; }

    public event EventHandler TimerElapsed; 

    public TimerService()
    {
        _perpetualTimer.Interval = TimerInterval;
        _perpetualTimer.Elapsed += PerpetualCountEvent;
        _timer.Interval = TimerInterval;
        _timer.Elapsed += CountEvent;
    }

    /// <summary>
    /// Starts the timer
    /// </summary>
    /// <param name="existingTime"></param>
    public void StartTimer(TimeSpan existingTime)
    {
        if (ElapsedTime != TimeSpan.Zero || _hours > 0 || _minutes > 0 || _seconds > 0)
        {
            ResetTimer();
        }

        _perpetualElapsedTime = existingTime;
        _perpetualHours = existingTime.Hours;
        _perpetualMinutes = existingTime.Minutes;

        ElapsedTime = existingTime;
        _hours = existingTime.Hours;
        _minutes = existingTime.Minutes;

        _perpetualTimer.Start();
        _timer.Start();
        Running = true;
    }

    /// <summary>
    /// Adjusts the Elapsed time forwards or backwards
    /// </summary>
    /// <param name="newStartTime"></param>
    /// <param name="pullBack"></param>
    public void AdjustTimer(TimeSpan newStartTime, bool pullBack)
    {
        ElapsedTime = pullBack ? ElapsedTime.Add(newStartTime) : ElapsedTime.Subtract(newStartTime);

        _hours = newStartTime.Hours;
        _minutes = newStartTime.Minutes;
    }

    /// <summary>
    /// Stops the timer
    /// </summary>
    public void StopTimer()
    {
        _perpetualTimer.Stop();
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
    /// Resyncs the timer with the unchanging timer
    /// </summary>
    public void ResyncTimers()
    {
        ElapsedTime = _perpetualElapsedTime;

        _hours = _perpetualHours;
        _minutes = _perpetualMinutes;
    }

    /// <summary>
    /// Resets Timer values
    /// </summary>
    private void ResetTimer()
    {
        _perpetualHours = 0;
        _perpetualMinutes = 0;
        _perpetualSeconds = 0;
        _perpetualElapsedTime = TimeSpan.Zero;

        _hours = 0;
        _minutes = 0;
        _seconds = 0;
        ElapsedTime = TimeSpan.Zero;
    }

    /// <summary>
    /// Perpetual timer count event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PerpetualCountEvent(object sender, ElapsedEventArgs e)
    {
        _perpetualSeconds += 1;

        if (_perpetualSeconds == 60)
        {
            _seconds = 0;
            _minutes += 1;
        }

        if (_perpetualMinutes == 60)
        {
            _minutes = 0;
            _hours += 1;
        }

        _perpetualElapsedTime = new TimeSpan(_perpetualHours, _perpetualMinutes, _perpetualSeconds);
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