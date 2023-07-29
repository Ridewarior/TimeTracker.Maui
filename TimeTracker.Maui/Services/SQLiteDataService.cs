using Microsoft.Extensions.Logging;
using SQLite;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.Services;

public class SQLiteDataService
{
    private SQLiteConnection _conn;
    private readonly string _dbPath;
    private readonly ILogger<SQLiteDataService> _logger;

    public SQLiteDataService(string dbPath, ILogger<SQLiteDataService> logger)
    {
        _dbPath = dbPath;
        _logger = logger;
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
        _logger.LogInformation("SQLite connection initialized");
    }

    /// <summary>
    /// Gets all Time Records from the data source
    /// </summary>
    /// <returns>List of TimeRecords</returns>
    public List<TimeRecord> GetTimeRecords()
    {
        try
        {
            Init();

            var results = _conn.Table<TimeRecord>().ToList();
            _logger.LogDebug("Returned {resultsCount} records from the source", results.Count);

            return results;
        }
        catch (Exception e)
        {
            _logger.LogError(e.InnerException, "Failed to retrieve the Time Records from the source. Exception: {Message}", e.Message);
        }

        return null;
    }
    
    /// <summary>
    /// Gets a single Time Record from the data source
    /// </summary>
    /// <param name="recordId"></param>
    /// <returns>TimeRecord</returns>
    public TimeRecord GetTimeRecord(string recordId)
    {
        try
        {
            Init();

            var result = _conn.Table<TimeRecord>().FirstOrDefault(x => x.RECORD_ID == recordId);
            _logger.LogDebug("Record retrieved from source: {result}", result.RECORD_ID);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e.InnerException, "Failed to retrieve the Time Record from the source. Exception: {Message}", e.Message);
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
            var result = !queryResults.Any() ? 1 : queryResults.Max();
            _logger.LogDebug("Number of run counts retrieved: {queryResultsCount}. Highest run count is {result}", queryResults.Count, result);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e.InnerException, "Failed to retrieve the resumed run count. Exception: {Message}", e.Message);
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
                _logger.LogDebug("Record {record} was added successfully.", record.RECORD_ID);
                return _conn.Table<TimeRecord>().Count();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.InnerException, "Failed to add the Time Record. Exception: {Message}", e.Message);
        }

        return 0;
    }

    /// <summary>
    /// Updates a single TimeRecord
    /// </summary>
    /// <param name="record"></param>
    /// <returns>Status code</returns>
    public int UpdateRecord(TimeRecord record)
    {
        try
        {
            Init();

            var result = _conn.Update(record);

            if (result != 0)
            {
                _logger.LogDebug("Updated record {record} successfully", record.RECORD_ID);
                return result;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.InnerException, "Failed to update the Time Record. Exception {Message}", e.Message);
        }

        return 0;
    }

    public void UpdateAllRuns(string recordId, Dictionary<string, string>updatedValues)
    {
        if (!updatedValues.Any())
        {
            return;
        }

        try
        {
            Init();
            
            var recordsList = _conn.Table<TimeRecord>().Where(x => x.RECORD_ID == recordId || x.PARENT_ID == recordId).ToList();

            foreach (var record in recordsList)
            {
                foreach (var newValue in updatedValues)
                {
                    _logger.LogDebug("Property {newValueKey} is being updated to {newValue}", newValue.Key, newValue.Value);
                    switch (newValue.Key)
                    {
                        case nameof(TimeRecord.RECORD_TITLE):
                        {
                            record.RECORD_TITLE = newValue.Value;
                            break;
                        }
                        case nameof(TimeRecord.WORKITEM_TITLE):
                        {
                            record.WORKITEM_TITLE = newValue.Value;
                            break;
                        }
                        case nameof(TimeRecord.CLIENT_NAME):
                        {
                            record.CLIENT_NAME = newValue.Value;
                            break;
                        }
                        case nameof(TimeRecord.LOG_ID):
                        {
                            record.LOG_ID = newValue.Value;
                            break;
                        }
                    }
                }

                UpdateRecord(record);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.InnerException, "Failed to update the Time Records. Exception: {Message}", e.Message);
        }
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

#if DEBUG
            var recordToDelete = GetTimeRecord(recordId);
            _logger.LogDebug("Record to delete: {recordToDelete}", recordToDelete);
#endif

            var result = _conn.Table<TimeRecord>().Delete(x => x.RECORD_ID == recordId);

            if (result != 0)
            {
                return result;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.InnerException, "Failed to delete the Time Record. Exception: {Message}", e.Message);
        }
        
        return 0;
    }
}