using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockSavvy.Models;

public class PortfolioMongoModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("UserId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId UserId { get; set; }
    
    [BsonElement("Status")]
    [BsonRepresentation(BsonType.Boolean)]
    public bool Status { get; set; }
    
    [BsonElement("Stocks")]
    [BsonRepresentation(BsonType.Array)]
    //this list contaions id of stocks that part of this particular portfolio
    public List<string> Stocks { get; set; }
}