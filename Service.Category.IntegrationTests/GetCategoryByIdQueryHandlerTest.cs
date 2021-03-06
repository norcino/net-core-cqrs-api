﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Common.Tests.FluentAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Category.Query;

namespace Service.Category.IntegrationTests
{
    [TestClass]
    public class CategoryValidatorTest : BaseServiceIntegrationTest
    {
        [TestMethod]
        public async Task Handler_get_category_by_id_with_the_correct_properties()
        {
            var category = new Data.Entity.Category
            {
                Name = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                Description = "Test description",
                Active = true
            };

            await Context.Categories.AddAsync(category);
            await Context.SaveChangesAsync();

            var query = new GetCategoryByIdQuery(category.Id);
            var dbCategory = await mediator.ProcessQueryAsync(query);

            Assert.IsNotNull(dbCategory);

            Assert.That.This(dbCategory).HasSameProperties(category);
        }
    }
}
