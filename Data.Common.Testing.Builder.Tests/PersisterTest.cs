using System.Linq;
using Common.IntegrationTests;
using Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Tests.FluentAssertion;

namespace Data.Common.Testing.Builder.Tests
{
    [TestClass]
    public class PersisterTest : BaseIdempotentIntegrationTest
    {
        [TestMethod]
        public void Persist_should_save_entity_to_the_database_with_default_values()
        {
            var category = Persister<Category>.New().Persist();
            var dbCategory = Context.Categories.FirstOrDefault(c => c.Id == category.Id);
            Assert.That.This(category).HasSameProperties(dbCategory);
        }

        [TestMethod]
        public void Persist_passing_10_should_save_10_entities_to_the_database_with_default_values()
        {
            var categories = Persister<Category>.New().Persist(10);
            Assert.That.All(categories).HaveCount(10);
            Assert.That.All(categories).Are(c => c != null);

            categories.ForEach(c =>
                Assert.That.This(Context.Categories.FirstOrDefault(current => current.Id == c.Id)).HasSameProperties(c)
            );
        }

        //[TestMethod]
        //public void TestMethod1()
        //{
        //    var category = Builder<Category>.New().Build();
        //    var category2 = Builder<Category>.New().BuildMany(2);
        //    var category3 = Builder<Category>.New().BuildMany(3, (c, i) =>
        //    {
        //        c.Name = $"Name_{i}";
        //    });

        //    var categoryF = Builder<Category>.New().Build((c) =>
        //    {
        //        c.Name = "Name_asdasdad";
        //        c.Active = false;
        //    });

        //    Assert.AreEqual(2, category2.Count);
        //    Assert.AreEqual(3, category3.Count);
            
        //     var context = ContextProvider.GetContext();

        //    Assert.AreEqual(0, context.Categories.Count());

        //    var persister = new Persister<Category>(context);

        //    var ct = persister.Persist(c =>
        //        {
        //            c.Name = "Manuel";
        //            c.Description = "Bello";
        //        });

        //    Assert.IsNotNull(ct);

        //    Assert.AreEqual(1, context.Categories.Count());

        //    var loaded = context.Categories.Find(ct.Id);
            
        //    Assert.AreEqual("Manuel", loaded.Name);
        //    Assert.AreEqual("Bello", loaded.Description);

        //    persister.Persist(100, (c, i) =>
        //    {
        //        c.Name = $"Manuel_{i}";
        //        c.Description = "Bello";
        //        c.Active = i % 2 == 0;
        //    });

        //    var tmp = context.Categories.ToList();
        //    Assert.AreEqual(101, context.Categories.Count());
        //}
    }
}
