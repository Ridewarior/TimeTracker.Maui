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
            return _conn.Table<TimeRecord>().FirstOrDefault(x => x.TIMERECORD_ID == recordId);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to retrieve Time Record. Inner exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to retrieve Time Record";
        }
        
        return null;
    }

    /// <summary>
    /// Gets the Record Title of the given Record Id
    /// </summary>
    /// <param name="recordId"></param>
    /// <returns></returns>
    public string GetParentRecordTitle(string recordId)
    {
        try
        {
            Init();
            return _conn.Table<TimeRecord>().Where(x => x.TIMERECORD_ID == recordId).Select(x => x.RECORD_TITLE).FirstOrDefault();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to retrieve Time Record Title. Inner exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to retrieve Time Record";
        }

        return null;
    }

    /// <summary>
    /// Gets the count of Time Records with the same Parent Record Id
    /// </summary>
    /// <param name="recordId"></param>
    /// <returns></returns>
    public int GetResumedRecordCount(string recordId)
    {
        try
        {
            Init();
            return _conn.Table<TimeRecord>().Count(x => x.PARENT_RECORD_ID == recordId);
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
    /// <exception cref="NotImplementedException"></exception>
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
    /// Deletes a single Time Record from the data source
    /// </summary>
    /// <param name="recordId"></param>
    /// <returns>Returns that Delete result</returns>
    public int DeleteRecord(string recordId)
    {
        try
        {
            Init();
            return _conn.Table<TimeRecord>().Delete(x => x.TIMERECORD_ID == recordId);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to delete record. Inner exception: \n{e.Message} \n{e.InnerException}");
            StatusMessage = "Failed to delete record";
        }
        
        return 0;
    }
}