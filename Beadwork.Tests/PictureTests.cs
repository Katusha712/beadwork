using System;
using Xunit;

namespace Beadwork.Tests
{
    public class PictureTests
    {
        [Fact]
        public void IsItemNumber_WithNull_ReturnFalse()
        {
            bool actual = Picture.IsItemNumber(null);

            Assert.False(actual);
        }

        [Fact]
        public void IsItemNumber_WithBlankString_ReturnFalse()
        {
            bool actual = Picture.IsItemNumber("    ");

            Assert.False(actual);
        }

        [Fact]
        public void IsItemNumber_WithInvalidItemNumberl_ReturnFalse()
        {
            bool actual = Picture.IsItemNumber("AAA");

            Assert.False(actual);
        }

        [Fact]
        public void IsItemNumber_WithValidItemNumber_ReturnTrue()
        {
            bool actual = Picture.IsItemNumber("IN-25496321");

            Assert.True(actual);
        }

        [Fact]
        public void IsItemNumber_WithTrashItemNumberl_ReturnFalse()
        {
            bool actual = Picture.IsItemNumber("aaa In-25496321 aaa");

            Assert.False(actual);
        }
    }
}
