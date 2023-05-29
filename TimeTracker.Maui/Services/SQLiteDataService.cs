using System.Diagnostics;
using SQLite;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.Services;

public class SQLiteDataService
{
    private SQLiteConnection _conn;
    private readonly string _dbPath;
    // TODO Eventually create a logger class that will handle logging full exceptions as well as holding status messages for display
    public string StatusMessage;

    public SQLiteDataService(string dbPath)
    {
        _dbPath = dbPath;
    }

    /// <summary>
    /// Initializes database connection if it's not already existing
    /// </summary>
    private void Init()
    {
        if (_conn != null)
        {
            return;
        }

        _conn = new SQLiteConnection(_dbPath);
        _conn.CreateTable<TimeRecord>();
    }

    /// <summary>
    /// Gets Time Records from the data source
    /// </summary>
    /// <returns>Returns a list of TimeRecord</returns>
    public List<TimeRecord> GetTimeRecords()
    {
        //try
        //{
        //    Init();
        //    return _conn.Table<TimeRecord>().ToList();
        //}
        //catch (Exception e)
        //{
        //    StatusMessage = "Failed to retrieve Time Records";
        //    Console.WriteLine(e);
        //    throw;
        //}
        //return new List<TimeRecord>();

        //Mock Data
        return new List<TimeRecord>()
        {
            new TimeRecord
            {
                TimeRecordID = 1, RecordTitle = "Test1", StartTime = "05/20/2023 12:00:00PM", StopTime = "05/20/2023 03:00:00PM", TimeElapsed = "03:00", WorkItemTitle = "TestWI", ClientName = "Empower", LogID = "B1006512"
            },
            new TimeRecord
            {
                TimeRecordID = 2, RecordTitle = "Test2", StartTime = "05/21/2023 12:00:00PM", StopTime = "05/21/2023 03:00:00PM", TimeElapsed = "03:00", WorkItemTitle = "TestWI", ClientName = "Empower", LogID = "B1006512"
            },
            new TimeRecord
            {
                TimeRecordID = 3, RecordTitle = "Test3", StartTime = "05/22/2023 12:00:00PM", StopTime = "05/22/2023 03:00:00PM", TimeElapsed = "03:00", WorkItemTitle = "TestWI", ClientName = "Empower", LogID = "B1006512"
            },
            new TimeRecord
            {
                TimeRecordID = 4, RecordTitle = "Test4", StartTime = "05/23/2023 12:00:00PM", StopTime = "05/23/2023 03:00:00PM", TimeElapsed = "03:00", WorkItemTitle = "TestWI", ClientName = "Empower", LogID = "B1006512"
            },
            new TimeRecord
            {
                TimeRecordID = 5, RecordTitle = "Test5", StartTime = "05/24/2023 12:00:00PM", StopTime = "05/25/2023 03:00:00PM", TimeElapsed = "03:00", WorkItemTitle = "TestWI", ClientName = "Empower", LogID = "B1006512"
            },
        };
    }
    
    public TimeRecord GetTimeRecord(int recordId)
    {
        try
        {
            Init();
            return _conn.Table<TimeRecord>().FirstOrDefault(x => x.TimeRecordID == recordId);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to retrieve Time Record. Inner exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to retrieve Time Record";
        }

        return null;
    }

    public int DeleteRecord(int recordId)
    {
        try
        {
            Init();
            return _conn.Table<TimeRecord>().Delete(x => x.TimeRecordID == recordId);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to delete record. Inner exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to delete record";
        }

        return 0;
    }
}