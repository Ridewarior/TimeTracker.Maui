namespace TimeTracker.Maui.Models;

public class GroupedRecords : List<TimeRecord>
{
    public string GroupDate { get; set; }

    public string AccumulatedTime { get; set; }

    public GroupedRecords(string groupDate, string accumulatedTime, IEnumerable<TimeRecord> timeRecords) : base(timeRecords)
    {
        GroupDate = groupDate;
        AccumulatedTime = accumulatedTime;
    }
}