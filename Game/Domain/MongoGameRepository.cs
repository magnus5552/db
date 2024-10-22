using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gameCollection;
        
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db)
        {
            gameCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            return gameCollection.Find(x => x.Id == gameId).SingleOrDefault();
        }

        public void Update(GameEntity game)
        {
            gameCollection.UpdateOne(x => x.Id == game.Id, new BsonDocumentUpdateDefinition<GameEntity>(new BsonDocument("$set", game.ToBsonDocument())));
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameCollection.Find(x => x.Status == GameStatus.WaitingToStart).Limit(limit).ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var result = gameCollection.UpdateOne(x => x.Status == GameStatus.WaitingToStart && x.Id == game.Id, new BsonDocumentUpdateDefinition<GameEntity>(new BsonDocument("$set", game.ToBsonDocument())));
            return result.ModifiedCount > 0;
        }
    }
}