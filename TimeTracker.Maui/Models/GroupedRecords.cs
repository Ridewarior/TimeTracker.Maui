namespace TimeTracker.Maui.Models;

public class GroupedRecords : List<TimeRecord>
{
    public string GroupDate { get; set; }

    public GroupedRecords(string groupDate, List<TimeRecord> timeRecords) : base(timeRecords)
    {
        GroupDate = groupDate;
    }
}