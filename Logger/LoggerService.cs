using Logger.Models;
using MongoDB.Driver;

namespace Logger;

public class LoggerService: ILoggerService
{
    public bool ProcessLog(string logMessage)
    {
        try
        {
            // Establish a connection to the local MongoDB instance
            MongoClient client = new MongoClient("mongodb://host.docker.internal:27017");

            // Access the "LogData" database and retrieve the "logInfo" collection
            var logListCollection = client.GetDatabase("LogData").GetCollection<LogInfo>("logInfo");

            // Insert a new LogInfo document into the logInfo collection
            logListCollection.InsertOne(new LogInfo(logMessage));

            // Define a filter to find LogInfo documents with a specific logMessage
            FilterDefinition<LogInfo> filter = Builders<LogInfo>.Filter.Eq("message", logMessage);

            // Execute the query to find LogInfo documents matching the filter and convert the results to a list
            List<LogInfo> results = logListCollection.Find(filter).ToList();

            // Iterate through the results and print each LogInfo's EntryTime and Message to the console
            foreach (LogInfo result in results)
            {
                // Corrected print format
                Console.WriteLine($"{result.EntryTime.ToString("dd/MM/yyyy hh:mm tt")}: {result.Message}");
            }

            // Indicate that the log processing was successful by returning true
            return true;
        }
        catch (Exception ex)
        {
            // Print the exception message if an error occurs
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false; // Indicate that the log processing was unsuccessful
        }
    }
}

