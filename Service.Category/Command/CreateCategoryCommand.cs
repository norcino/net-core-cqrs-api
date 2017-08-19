using Common.Log;
using Service.Common;

namespace Service.Category.Command
{
    public class CreateCategoryCommand : ICommand
    {
        public Data.Entity.Category Category { get; set; }

        public CreateCategoryCommand(Data.Entity.Category category)
        {
            Category = category;
        }

        public LogInfo ToLog()
        {
            const string template = "Category: {Name}";
            return new LogInfo(template, Category.Name);
        }
    }
}
