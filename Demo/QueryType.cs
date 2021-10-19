using HotChocolate.Data.Filters;
using HotChocolate.Types;

namespace Demo
{
    public class QueryType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor
                .Field(f => f.GetBooks())
                .UseFiltering<BookFilterType>();
        }
    }
    
    public class BookFilterType : FilterInputType<Book>
    {
        protected override void Configure(IFilterInputTypeDescriptor<Book> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor
                .Field(f => f.Author.Name)
                .Name("authorName")
                .Type<StringOperationFilterInputType>();
        }
    }
}