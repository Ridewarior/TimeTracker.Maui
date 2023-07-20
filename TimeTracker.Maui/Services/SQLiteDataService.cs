using System.Diagnostics;
using SQLite;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.Services;

public class SQLiteDataService
{
    private SQLiteConnection _conn;
    private readonly string _dbPath;
    // Eventually create a logger class that will handle logging full exceptions as well as holding status messages for display
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
        SQLitePCL.Batteries.Init();
        _conn = new SQLiteConnection(_dbPath);
        _conn.CreateTable<TimeRecord>();
    }

    /// <summary>
    /// Gets all Time Records from the data source
    /// </summary>
    /// <returns>Returns a list of TimeRecord</returns>
    public List<TimeRecord> GetTimeRecords()
    {
        try
        {
            Init();
            return _conn.Table<TimeRecord>().ToList();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to retrieve Time Records. Inner Exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to retrieve Time Records";
        }

        return new List<TimeRecord>();
    }
    
    /// <summary>
    /// Gets a single Time Record from the data source
    /// </summary>
    /// <param name="recordId"></param>
    /// <returns>Returns a TimeRecord</returns>
    public TimeRecord GetTimeRecord(string recordId)
    {
        try
        {
            Init();
            return _conn.Table<TimeRecord>().FirstOrDefault(x => x.RECORD_ID == recordId);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to retrieve Time Record. Inner exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to retrieve Time Record";
        }
        
        return null;
    }

    /// <summary>
    /// Gets the highest run count for the resumed record
    /// </summary>
    /// <param name="recordId"></param>
    /// <returns>The highest run count for the resumed record</returns>
    public int GetResumedRunCount(string recordId)
    {
        try
        {
            Init();
            var queryResults = _conn.Table<TimeRecord>().Where(x => x.PARENT_ID == recordId).Select(x => x.RUN_COUNT).ToList();

            return !queryResults.Any() ? 1 : queryResults.Max();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to retrieve count of Time Records. Inner exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to retrieve count of Time Records";
        }

        return 0;
    }

    /// <summary>
    /// Adds a new Time Record to the data source
    /// </summary>
    /// <param name="record"></param>
    /// <returns>Status code</returns>
    public int AddRecord(TimeRecord record)
    {
        try
        {
            Init();
            if (record == null)
            {
                throw new InvalidDataException("Invalid Time Record");
            }
            
            var result = _conn.Insert(record);
            if (result != 0)
            {
                StatusMessage = "Insert Successful";
                return _conn.Table<TimeRecord>().Count();
            }
            else
            {
                StatusMessage = "Insert Failed";
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to insert record. Inner Exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to insert record";
        }

        return 0;
    }

    /// <summary>
    /// Updates the TimeRecord
    /// </summary>
    /// <param name="record"></param>
    /// <returns>Status code</returns>
    public int UpdateRecord(TimeRecord record)
    {
        try
        {
            Init();
            return _conn.Update(record);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to update record. Inner exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to update record";
        }

        return 0;
    }

    /// <summary>
    /// Deletes a single Time Record from the data source
    /// </summary>
    /// <param name="recordId"></param>
    /// <returns>Status code</returns>
    public int DeleteRecord(string recordId)
    {
        try
        {
            Init();
            return _conn.Table<TimeRecord>().Delete(x => x.RECORD_ID == recordId);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to delete record. Inner exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to delete record";
        }
        
        return 0;
    }
}