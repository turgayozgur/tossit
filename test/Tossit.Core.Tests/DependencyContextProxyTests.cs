using System.Linq;
using Xunit;

namespace Tossit.Core.Tests
{
    public class DependencyContextProxyTests
    {
        [Fact]
        public void GetDefaultAssemblyNamesShouldReturnAssemblyNames()
        {
            // Arrange
            var dependencyContextProxy = new DependencyContextProxy();

            // Act
            var result = dependencyContextProxy.GetDefaultAssemblyNames();

            // Assert
            Assert.True(result.Count() > 0);
        }
    }
}