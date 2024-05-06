using Logger.Models;
using MongoDB.Driver;

namespace Logger;

public class LoggerService: ILoggerService
{
    public bool ProcessLog(string logMessage)
    {
        MongoClient client = new MongoClient("mongodb://localhost:27017");

        var playlistCollection = client.GetDatabase("sample_mflix").GetCollection<Playlist>("playlist");

        List<string> movieList = new List<string>();
        movieList.Add("1234");

        playlistCollection.InsertOne(new Playlist("nraboy", movieList));

        FilterDefinition<Playlist> filter = Builders<Playlist>.Filter.Eq("username", "nraboy");

        List<Playlist> results = playlistCollection.Find(filter).ToList();

        foreach (Playlist result in results)
        {
            Console.WriteLine(string.Join(", ", result.items));
        }

        return true;
    }

    //private readonly ILogger _logger;

    //public LoggerService()
    //{
    //    _logger = new LoggerConfiguration()
    //        .MinimumLevel.Debug()
    //        .WriteTo.MongoDB("mongodb://localhost:27017", "sample_mflix", Serilog.Events.LogEventLevel.Information)
    //        .CreateLogger();
    //}

    //public bool ProcessLog(string logMessage)
    //{
    //    try
    //    {
    //        // Log the message received from the calling application
    //        _logger.Information(logMessage);

    //        // Your existing logging logic goes here
    //        MongoClient client = new MongoClient("mongodb://localhost:27017");
    //        var database = client.GetDatabase("sample_mflix"); // Your MongoDB database name
    //        var collection = database.GetCollection<LogEvent>("LogCollector"); // Your MongoDB collection name

    //        // Serialize log message and insert into MongoDB
    //        var logEvent = new LogEvent(
    //            DateTimeOffset.UtcNow,
    //            LogEventLevel.Information,
    //            null,
    //            new MessageTemplate(logMessage),
    //            new LogEventProperty[0]
    //        );
    //        collection.InsertOne(logEvent);

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log error if an exception occurs during logging
    //        _logger.Error(ex, "Error occurred while processing log.");
    //        return false;
    //    }
    //}
}

