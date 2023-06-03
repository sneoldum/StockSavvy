using MongoDB.Bson;
using MongoDB.Driver;
using StockSavvy.Models;

namespace StockSavvy.Services;

public class UserService
{
    private IMongoCollection<UserMongoModel> userCollection;

    public UserService()
    {
        var client =
            new MongoClient(
                //"mongodb+srv://admin:hQSH0VkQDAmU9gT2@mongocluster.uqjdact.mongodb.net/?retryWrites=true&w=majority");
        "mongodb+srv://admin:hQSH0VkQDAmU9gT2@mongocluster.uqjdact.mongodb.net/stocksavvy?connect=replicaSet");

        var database = client.GetDatabase("stocksavvy");
        userCollection = database.GetCollection<UserMongoModel>("User");
    }

    //returns all of the Users from db
    public List<UserMongoModel> Get()
    {
        return userCollection.Find(model => true).ToList();
    }

    //returns user by unique username
    public UserMongoModel GetOneByUsername(String username)
    {
        return userCollection.Find(model => model.Username == username).First();
    }

    //creates a user from scratch, Id field must be null in order to create new one otherwise it will try to update an existing one
    public UserMongoModel Create(UserMongoModel userMongoModel)
    {
        userCollection.InsertOne(userMongoModel);
        return userMongoModel;
    }

    //updates a user, usermongomodel has to contain a valid user id
    public UserMongoModel Update(UserMongoModel userMongoModel)
    {
        userCollection.ReplaceOne(model => model.Id == userMongoModel.Id, userMongoModel);
        return userMongoModel;
    }
    
    //deletes user by its id
    public void Delete(ObjectId id)
    {
        userCollection.DeleteOne(model => model.Id == id);
    }
}