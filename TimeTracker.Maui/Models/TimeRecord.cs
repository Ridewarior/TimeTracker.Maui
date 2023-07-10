using SQLite;

namespace TimeTracker.Maui.Models;

[Table("TimeRecords")]
public class TimeRecord
{
    /// <summary>
    /// Unique ID of the Time Record
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int TIMERECORD_ID { get; set; }

    /// <summary>
    /// Title of the Time Record
    /// </summary>
    public string RECORD_TITLE { get; set; }

    /// <summary>
    /// Start date and time of the Time Record
    /// </summary>
    public string START_TIMESTAMP { get; set; }

    /// <summary>
    /// Stop date and time of the Time record
    /// </summary>
    public string STOP_TIMESTAMP { get; set; }

    /// <summary>
    /// Total time accumulated for the Time Record
    /// </summary>
    public string TIME_ELAPSED { get; set; }

    /// <summary>
    /// Title of the Work Item for this Time Record
    /// </summary>
    public string WORKITEM_TITLE { get; set; }

    /// <summary>
    /// Name of the Client the Time Record is logged under
    /// </summary>
    public string CLIENT_NAME { get; set; }

    /// <summary>
    /// Specific ID the Time Record can be logged under.
    /// This is commonly used to identify which ID the time should be logged under in a corporate system.
    /// </summary>
    public string LOG_ID { get; set; }

    /// <summary>
    /// True if the selected record has a running timer
    /// </summary>
    [Ignore]
    public bool REC_TIMER_RUNNING { get; set; }
}