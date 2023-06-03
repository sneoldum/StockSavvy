using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace StockSavvy.Models;

public class UserMongoModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    [BsonElement("FirstName")]
    public string FirstName { get; set; }
    [BsonElement("LastName")]
    public string LastName { get; set; }
    [BsonElement("Username")]
    public string Username { get; set; }
    [BsonElement("PasswordSalt")]
    public string PasswordSalt { get; set; }
    [BsonElement("PasswordHash")]
    public string PasswordHash { get; set; }
    [BsonElement("PortfolioId")]
    [BsonRepresentation(BsonType.ObjectId)]
    //this part should filled when user creates a portfolio. this should be equal to Id field in @PortfolioMongoModel class
    public ObjectId PortfolioId { get; set; }
    [BsonElement("Status")]
    [BsonRepresentation(BsonType.Boolean)]
    public bool Status { get; set; }
    
    public void testConnection()
    {
        
        //using MongoDB.Driver;
        //using MongoDB.Bson;

        const string connectionUri = "mongodb+srv://admin:hQSH0VkQDAmU9gT2@mongocluster.uqjdact.mongodb.net/?retryWrites=true&w=majority";

        var settings = MongoClientSettings.FromConnectionString(connectionUri);

        // Set the ServerApi field of the settings object to Stable API version 1
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        // Create a new client and connect to the server
        var client = new MongoClient(settings);

        // Send a ping to confirm a successful connection
        try {
            var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
        } catch (Exception ex) {
            Console.WriteLine(ex);
        }

    }
}