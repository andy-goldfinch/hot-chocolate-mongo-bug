using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Demo
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }

        public Author Author { get; set; }

    }
}