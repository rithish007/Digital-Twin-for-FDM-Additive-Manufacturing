using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;

public class MongoDBManager : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> collection;

    void Start()
    {
        ConnectToMongoDB();
    }

    void ConnectToMongoDB()
    {
        // Replace with your MongoDB Atlas connection string
        string connectionString = "mongodb+srv://Rithish:<DB2024>@printerdatacluster.wwmr4.mongodb.net/?retryWrites=true&w=majority&appName=PrinterDataCluster";
        client = new MongoClient(connectionString);
        database = client.GetDatabase("Printer DT");
        collection = database.GetCollection<BsonDocument>("PrinterRPiDB");
        Debug.Log("Connected to MongoDB Atlas");
    }

    public List<BsonDocument> GetLatestData(int limit = 10)
    {
        var sort = Builders<BsonDocument>.Sort.Descending("_id");
        var documents = collection.Find(new BsonDocument())
            .Sort(sort)
            .Limit(limit)
            .ToList();
        return documents;
    }
}