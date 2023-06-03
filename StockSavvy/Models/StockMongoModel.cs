using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockSavvy.Models;

public class StockMongoModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("StockCode")]
    //tsla, amzn etc.
    public string StockCode { get; set; }
    
    [BsonElement("AverageCost")]
    //average cost must recalculated for every add or delete operation on amont
    public double AverageCost { get; set; }
    
    [BsonElement("Amount")]
    public double Amount { get; set; }

    [BsonElement("Status")]
    [BsonRepresentation(BsonType.Boolean)]
    public bool Status { get; set; }
}