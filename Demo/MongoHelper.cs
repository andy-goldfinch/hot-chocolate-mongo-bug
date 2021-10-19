using MongoDB.Driver;

namespace Demo
{
    public class MongoHelper
    {
        public IMongoCollection<Book> GetCollection()
        {
            MongoClient dbClient = new MongoClient("mongodb://root:password@localhost:27037");

            var collection = dbClient.GetDatabase("admin").GetCollection<Book>("books");

            if (collection.CountDocuments(x => true) == 0)
            {
                collection.InsertMany(new[]
                {
                    new Book()
                    {
                        Title = "C# For Beginners",
                        Author = new Author()
                        {
                            Name = "Jon Skeet"
                        }
                    },
                    new Book()
                    {
                        Title = "Java For Experts",
                        Author = new Author()
                        {
                            Name = "Rita Skeeter"
                        }
                    }
                });
            }

            return collection;
        }
    }
}