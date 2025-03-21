using Vanguard.Common.Base;

namespace Vanguard.UnitTests.Common
{
    public class ValueObjectTests
    {
        [Fact]
        public void Equal_ValueObjects_Should_Be_Equal()
        {
            // Arrange
            var address1 = new Address("123 Test St", "TestCity", "12345");
            var address2 = new Address("123 Test St", "TestCity", "12345");

            // Act & Assert
            Assert.Equal(address1, address2);
            Assert.True(address1 == address2);
            Assert.False(address1 != address2);
        }

        [Fact]
        public void Different_ValueObjects_Should_Not_Be_Equal()
        {
            // Arrange
            var address1 = new Address("123 Test St", "TestCity", "12345");
            var address2 = new Address("456 Test St", "TestCity", "12345");

            // Act & Assert
            Assert.NotEqual(address1, address2);
            Assert.False(address1 == address2);
            Assert.True(address1 != address2);
        }

        [Fact]
        public void GetHashCode_Should_Return_Same_Value_For_Equal_Objects()
        {
            // Arrange
            var address1 = new Address("123 Test St", "TestCity", "12345");
            var address2 = new Address("123 Test St", "TestCity", "12345");

            // Act & Assert
            Assert.Equal(address1.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_Should_Return_Different_Values_For_Different_Objects()
        {
            // Arrange
            var address1 = new Address("123 Test St", "TestCity", "12345");
            var address2 = new Address("456 Test St", "TestCity", "12345");

            // Act & Assert
            Assert.NotEqual(address1.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void Equality_Operators_Should_Handle_Null_Values()
        {
            // Arrange
            var address = new Address("123 Test St", "TestCity", "12345");
            Address nullAddress = null;

            // Act & Assert
            Assert.False(address == null);
            Assert.False(null == address);
            Assert.True(address != null);
            Assert.True(null != address);
            Assert.True(nullAddress == null);
            Assert.True(null == nullAddress);
            Assert.False(nullAddress != null);
            Assert.False(null != nullAddress);
        }

        [Fact]
        public void Equal_References_Should_Be_Equal()
        {
            // Arrange
            var address1 = new Address("123 Test St", "TestCity", "12345");
            var address2 = address1;

            // Act & Assert
            Assert.Equal(address1, address2);
            Assert.True(address1 == address2);
            Assert.False(address1 != address2);
        }

        // Test class that implements ValueObject for testing
        private class Address : ValueObject
        {
            public string Street { get; }
            public string City { get; }
            public string ZipCode { get; }

            public Address(string street, string city, string zipCode)
            {
                Street = street;
                City = city;
                ZipCode = zipCode;
            }

            protected override IEnumerable<object> GetAtomicValues()
            {
                yield return Street;
                yield return City;
                yield return ZipCode;
            }
        }
    }
}