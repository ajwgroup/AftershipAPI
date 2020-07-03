using AftershipAPI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AftershipAPITests
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void QueryString_SingleQuery()
        {
            var queryString = new Querystring();

            queryString.Add("name", new List<string> { "value" });

            var result = queryString.ToString();

            result.Should().Be("?name=value");
        }

        [TestMethod]
        public void QueryString_MultiQuery()
        {
            var queryString = new Querystring();

            queryString.Add("name", new List<string> { "value1", "value2" });

            var result = queryString.ToString();

            result.Should().Be("?name=value1%2Cvalue2");
        }

        [TestMethod]
        public void AddRequireField_AddAField_IsAdded()
        {
            var courier = new Courier("url", "slug", "name", "phone", "otherName");

            const string RequierField = "required";
            courier.AddRequireField(RequierField);

            courier.RequireFields.Should().Contain(RequierField);
        }

        [TestMethod]
        public void AddRequireField_AddTwoFields_BothAdded()
        {
            var courier = new Courier("url", "slug", "name", "phone", "otherName");

            const string RequierField = "required";
            courier.AddRequireField(RequierField);
            courier.AddRequireField($"{RequierField}1");

            courier.RequireFields.Should().Contain(RequierField).And.Contain(RequierField + "1");
        }

        [TestMethod]
        public void AddRequireField_DeleteRequireField_FieldIsDeleted()
        {
            var courier = new Courier("url", "slug", "name", "phone", "otherName");

            const string RequierField = "required";
            courier.AddRequireField(RequierField);
            courier.AddRequireField($"{RequierField}1");

            courier.DeleteRequireField(RequierField);

            courier.RequireFields.Should().OnlyContain(x => x == $"{RequierField}1");
        }

        [TestMethod]
        public void AddRequireField_DeleteRequireFields_NoRequireFields()
        {
            var courier = new Courier("url", "slug", "name", "phone", "otherName");

            const string RequierField = "required";
            courier.AddRequireField(RequierField);
            courier.AddRequireField($"{RequierField}1");

            courier.DeleteRequireFields();

            courier.RequireFields.Should().BeEmpty();
        }
    }    
}
