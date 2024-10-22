using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain;

public class MongoGameTurnRepository : IGameTurnRepository
{
    public const string CollectionName = "gameTurns";
    private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;

    public MongoGameTurnRepository(IMongoDatabase database)
    {
        gameTurnCollection = database.GetCollection<GameTurnEntity>(CollectionName);
        var createIndexModel = new CreateIndexModel<GameTurnEntity>(
            Builders<GameTurnEntity>.IndexKeys.Descending(x => x.TurnIndex));
        gameTurnCollection.Indexes.CreateOne(createIndexModel);
    }

    public void Insert(GameTurnEntity gameTurnEntity)
    {
        gameTurnCollection.InsertOne(gameTurnEntity);
    }

    public IEnumerable<GameTurnEntity> GetLastTurns(Guid gameId, int count)
    {
        return gameTurnCollection.Find(x => x.GameId == gameId)
                                 .SortByDescending(x => x.TurnIndex)
                                 .Limit(count)
                                 .ToList();
    }
}