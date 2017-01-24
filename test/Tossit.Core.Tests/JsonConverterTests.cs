using Xunit;

namespace Tossit.Core.Tests
{
    public class JsonConverterTests
    {
        [Fact]
        public void SerializeWithObjectShouldReturnString() 
        {
            // Arrange
            var jsonConverter = new JsonConverter();

            // Act
            var result = jsonConverter.Serialize(new FooClass());

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void DeserializeWithValidJsonStringShouldReturnObject() 
        {
            // Arrange
            var jsonConverter = new JsonConverter();

            // Act
            var result = jsonConverter.Deserialize<FooClass>("{Id:1}");

            // Assert
            Assert.True(result != null);
        }

        public class FooClass
        {
            public int Id { get; set; }
        }
    }
}
