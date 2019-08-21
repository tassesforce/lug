using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace lug.Context.Mongo
{
    public class MongoDbContext<T>
    {
        private IMongoClient client;
        public IMongoDatabase Database {get; private set;}
        public IMongoCollection<T> Collection {get; private set;}
        private ILogger logger;

        /// <summary>Initialisation of a context, with creation of a collection if unknown</summary>
        public async void InitializeContextASync(ILogger logger, string connectionString, string databaseName, string collectionName)
        {
            this.logger = logger;

            var mongoConnectionUrl = new MongoUrl(connectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);
                mongoClientSettings.ClusterConfigurator = cb => {
                    cb.Subscribe<CommandStartedEvent>(e => {
                        logger.LogInformation($"{e.CommandName} - {e.Command.ToJson()}");
                    });
                };

            client = new MongoClient(mongoClientSettings);
            Database = client.GetDatabase(databaseName);
            if (!(await CollectionExistsAsync(collectionName))) {
                logger.LogInformation("La collection {collection} est inconnue, creation en cours", collectionName);
                Database.CreateCollection(collectionName);
            } else {
                logger.LogInformation("La collection {collection} existe", collectionName);
            }
            Collection = Database.GetCollection<T>(collectionName);
        }

        /// <summary>Check if a collection exists</summary>
        public async Task<bool> CollectionExistsAsync(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            //filter by collection name
            var collections = await Database.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            //check for existence
            return await collections.AnyAsync();
        }
     
    }
}