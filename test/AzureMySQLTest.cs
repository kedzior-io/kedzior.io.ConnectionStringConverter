using System;
using Xunit;

namespace kedzior.io.ConnectionStringConverter.Tests
{
    public class AzureMySQLTest
    {
        [Fact]
        public void ToMySQLStandard_NonMySQLStandard_MySQLStandard()
        {
            var inputCS = "Database=localdb;Data Source=127.0.0.1:54068;User Id=azure;Password=kedzior";
            var expectedCS = "Database=localdb;Port=54068;Pwd=kedzior;Server=127.0.0.1;Uid=azure";

            var actualCS = inputCS.ToMySQLStandard();

            Assert.Equal(expectedCS, actualCS);
        }

        [Fact]
        public void ToMySQLStandard_RandomOrder_AlphabeticalOrder()
        {
            var inputCS = "Data Source=127.0.0.1:54068;Database=localdb;Password=kedzior;User Id=azure";
            var expectedCS = "Database=localdb;Port=54068;Pwd=kedzior;Server=127.0.0.1;Uid=azure";

            var actualCS = inputCS.ToMySQLStandard();

            Assert.Equal(expectedCS, actualCS);
        }

        [Fact]
        public void ToMySQLStandard_PortNotIncluded_AddDefaultPort()
        {
            var inputCS = "Data Source=127.0.0.1;Database=localdb;Password=kedzior;User Id=azure";
            var expectedCS = "Database=localdb;Port=3306;Pwd=kedzior;Server=127.0.0.1;Uid=azure";

            var actualCS = inputCS.ToMySQLStandard();

            Assert.Equal(expectedCS, actualCS);
        }

        [Fact]
        public void ToMySQLStandard_PortIncluded_PortNotDuplicate()
        {
            var inputCS = "Data Source=127.0.0.1;Port=3306;Database=localdb;Password=kedzior;User Id=azure";
            var expectedCS = "Database=localdb;Port=3306;Pwd=kedzior;Server=127.0.0.1;Uid=azure";

            var actualCS = inputCS.ToMySQLStandard();

            Assert.Equal(expectedCS, actualCS);
        }


        [Fact]
        public void ToMySQLStandard_ConnStringEmpty_ThrowsArgumentException()
        {
            var inputCS = "";
            var expectedMessage = "Connection String is empty.";

            var exception = Assert.Throws<ArgumentException>(() => inputCS.ToMySQLStandard());
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void ToMySQLStandard_ConnStringWhiteSpace_ThrowsArgumentException()
        {
            string inputCS = " ";
            var expectedMessage = "Connection String is empty.";

            var exception = Assert.Throws<ArgumentException>(() => inputCS.ToMySQLStandard());
            Assert.Equal(expectedMessage, exception.Message);
        }
    }
}
