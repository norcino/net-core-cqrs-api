using System.Linq;
using Common.IntegrationTests;
using Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Common.Testing.Builder.Tests
{
    [TestClass]
    public class CategoryBuilderTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var category = CategoryBuilder.New().Build();
            var category2 = CategoryBuilder.New().Build(2);
            var category3 = CategoryBuilder.New().Build(3, (c, i) =>
            {
                c.Name = $"Name_{i}";
                return c;
            });

            var categoryF = CategoryBuilder.New().Build((c) =>
            {
                c.Name = "Name_asdasdad";
                c.Active = false;
            });

            Assert.AreEqual(2, category2.Count);
            Assert.AreEqual(3, category3.Count);


             var context = TestDataConfiguration.GetContext();

            Assert.AreEqual(0, context.Categories.Count());

            var persister = new Persister<Category>(context);

            var ct = persister.Persist(c =>
                {
                    c.Name = "Manuel";
                    c.Description = "Bello";
                });

            Assert.IsNotNull(ct);

            Assert.AreEqual(1, context.Categories.Count());

            var loaded = context.Categories.Find(ct.Id);
            
            Assert.AreEqual("Manuel", loaded.Name);
            Assert.AreEqual("Bello", loaded.Description);

            persister.Persist(100, (c, i) =>
            {
                c.Name = $"Manuel_{i}";
                c.Description = "Bello";
                c.Active = i % 2 == 0;
            });

            var tmp = context.Categories.ToList();
            Assert.AreEqual(101, context.Categories.Count());
        }
    }
}
