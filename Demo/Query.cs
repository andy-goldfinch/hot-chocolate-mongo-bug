using HotChocolate;
using HotChocolate.Data;

namespace Demo
{
    
    public class Query
        {
            public IExecutable<Book> GetBooks()
            {
                var helper = new MongoHelper();

                return helper.GetCollection().AsExecutable();
            }
        }
}