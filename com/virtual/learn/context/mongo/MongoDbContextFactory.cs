using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace lug.Context.Mongo
{
    /// <summary>
    /// Factory de création de context mongoDb
    /// </summary>
    public class MongoDbContextFactory<T>
    {
        /// <summary>
        /// Méthode de création d'un MongoDbContext par defaut
        /// </summary>
        public static MongoDbContext<T> InitializeContext(ILogger logger, string connexionString, string databaseName, string collectionName)
        {
            MongoDbContext<T> context = new MongoDbContext<T>();
            context.InitializeContextASync(logger, connexionString, databaseName, collectionName);
            return context;
        }
    }
}