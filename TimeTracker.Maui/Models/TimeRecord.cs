using SQLite;

namespace TimeTracker.Maui.Models;

[Table("TimeRecords")]
public class TimeRecord
{
    /// <summary>
    /// Unique ID of the Time Record
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int TimeRecordID { get; set; }

    /// <summary>
    /// Title of the Time Record
    /// </summary>
    public string RecordTitle { get; set; }

    /// <summary>
    /// Start date and time of the Time Record
    /// </summary>
    [NotNull]
    public string StartTime { get; set; }

    /// <summary>
    /// Stop date and time of the Time record
    /// </summary>
    [NotNull]
    public string StopTime { get; set; }

    /// <summary>
    /// Total time accumulated for the Time Record
    /// </summary>
    public string TimeElapsed { get; set; }

    /// <summary>
    /// Title of the Work Item for this Time Record
    /// </summary>
    public string WorkItemTitle { get; set; }

    /// <summary>
    /// Name of the Client the Time Record is logged under
    /// </summary>
    public string ClientName { get; set; }

    /// <summary>
    /// Specific ID the Time Record can be logged under.
    /// This is commonly used to identify which ID the time should be logged under in a corporate system.
    /// </summary>
    public string LogID { get; set; }

    /// <summary>
    /// Parent ID of the original Time Record.
    /// This is used for records that have multiple iterations.
    /// </summary>
    public int ParentRecordID { get; set; }
}