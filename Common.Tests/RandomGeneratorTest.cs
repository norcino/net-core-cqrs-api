using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests
{
    [TestClass]
    public class RandomGeneratorTest
    {
        [DataRow("    ", 101)]
        [DataRow(" ", 987)]
        [DataRow(null, 33)]
        [DataTestMethod]
        public void String_returns_string_with_no_prefix_when_null_empty_or_space_and_correct_suffix_length(string prefix, int lenght)
        {
            Assert.AreEqual(lenght, AnonymousData.String(prefix, lenght).Length);
        }

        [DataRow("fff", 1)]
        [DataRow("d5f43g", 10)]
        [DataRow("4g2g4", 16)]
        [DataRow("Name", 100)]
        [DataRow("Test", 101)]
        [DataRow("wooo", 987)]
        [DataTestMethod]
        public void String_returns_string_with_the_desired_prefix_suffix_length(string prefix, int length)
        {
            var anonymousString = AnonymousData.String(prefix, length);
            Assert.IsTrue(anonymousString.StartsWith($"{prefix}_"));
            Assert.AreEqual(length, anonymousString.Length - $"{prefix}_".Length);
        }

        [DataRow(0)]
        [DataRow(-10)]
        [DataRow(-1)]
        [DataTestMethod]
        public void String_throws_exception_if_length_is_less_than_one(int length)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => AnonymousData.String("", length), "length must be greater than zero");
        }

        [DataRow(10)]
        [DataRow(16)]
        [DataRow(100)]
        [DataRow(101)]
        [DataRow(987)]
        [DataTestMethod]
        public void String_returns_string_with_no_prefix_and_desired_suffix_length(int length)
        {
            Assert.AreEqual(length, AnonymousData.String("", length).Length);
        }
    }
}