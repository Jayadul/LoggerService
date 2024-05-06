using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Logger.Models;

public class LogInfo
{
    public ObjectId _id { get; set; }

    [BsonElement("message")]
    public string Message { get; set; }
    public DateTime EntryTime { get; set; }

    public LogInfo(string message)
    {
        this.Message = message;
        this.EntryTime = DateTime.Now;
    }
}
