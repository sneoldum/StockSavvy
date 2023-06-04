using MongoDB.Bson;
using MongoDB.Driver;
using StockSavvy.Models;

namespace StockSavvy.Services;

public class PortfolioService
{
    private IMongoCollection<PortfolioMongoModel> PortfolioCollection;
    public PortfolioService()
    {
        var client = new MongoClient("mongodb+srv://admin:hQSH0VkQDAmU9gT2@mongocluster.uqjdact.mongodb.net/?retryWrites=true&w=majority");
        var database = client.GetDatabase("stocksavvy");
        PortfolioCollection = database.GetCollection<PortfolioMongoModel>("Portfolio");
    }

    //returns all of the portfoilos from db
    public List<PortfolioMongoModel> Get()
    {
        return PortfolioCollection.Find(model => true).ToList();
    }

    //returns portfolio by unique id
    public PortfolioMongoModel GetOneByUserId(ObjectId userId)
    {
        return PortfolioCollection.Find(model => model.UserId == userId).FirstOrDefault();
    }

    //creates a portfolio from scratch, Id field must be null in order to create new one otherwise it will try to update an existing one
    public string Create(PortfolioMongoModel portfolioMongoModel)
    {
        PortfolioCollection.InsertOne(portfolioMongoModel);
        return portfolioMongoModel.Id.ToString();
    }

    //updates a portfolio, portfoliomongomodel has to contain a valid portfolio id
    public string Update(PortfolioMongoModel portfolioMongoModel)
    {
        PortfolioCollection.ReplaceOne(model => model.Id == portfolioMongoModel.Id, portfolioMongoModel);
        return portfolioMongoModel.Id.ToString();
    }

    //deletes by id
    public void Delete(ObjectId objectId)
    {
        PortfolioCollection.DeleteOne(model => model.Id == objectId);
    }
}