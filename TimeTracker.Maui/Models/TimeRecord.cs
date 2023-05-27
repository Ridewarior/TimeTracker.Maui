namespace TimeTracker.Maui.Models;

public class TimeRecord
{
    public int TimeRecordID { get; set; }
    public string EntryTitle { get; set; }
    public string StartTime { get; set; }
    public string StopTime { get; set; }
    public string TimeElapsed { get; set; }
    public string WorkItemTitle { get; set; }
    public string ClientName { get; set; }
    public string BucketID { get; set; }
    public int ParentRecordID { get; set; }
}