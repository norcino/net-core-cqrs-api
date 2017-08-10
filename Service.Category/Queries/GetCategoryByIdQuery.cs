using Service.Common;

namespace Service.Category.Queries
{
    public class GetCategoryByIdQuery : IQuery<Data.Entity.Category>
    {
        public int CategoryId { get; }

        public GetCategoryByIdQuery(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
