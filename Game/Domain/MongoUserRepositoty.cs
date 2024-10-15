using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        private readonly object lockObject = new();
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            var createIndexModel = new CreateIndexModel<UserEntity>(
                Builders<UserEntity>.IndexKeys.Ascending(x => x.Login),
                new CreateIndexOptions
                {
                    Unique = true
                });
            userCollection.Indexes.CreateOne(createIndexModel);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(x => x.Id == id).SingleOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            lock (lockObject)
            {
                var found = userCollection.Find(x => x.Login == login).FirstOrDefault();
                if (found != null)
                    return found;
                var newUser = new UserEntity(Guid.NewGuid()) { Login = login };
                userCollection.InsertOne(newUser);
                return newUser;
            }
        }

        public void Update(UserEntity user)
        {
            userCollection.UpdateOne(x => x.Id == user.Id, new BsonDocumentUpdateDefinition<UserEntity>(new BsonDocument("$set", user.ToBsonDocument())));
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(x => x.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var skippedCount = (pageNumber - 1) * pageSize;
            var items = userCollection.Find(x => true)
                                      .SortBy(x => x.Login)
                                      .Skip(skippedCount)
                                      .Limit(pageSize)
                                      .ToList();
            var totalCount = userCollection.CountDocuments(x => true);
            var currentPage = items.Count > 0 ? pageNumber : (int)totalCount / pageSize;
            return new PageList<UserEntity>(items, totalCount, currentPage, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}