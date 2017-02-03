using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tossit.Core.Tests
{
    public class ReflectionHelperTests
    {
        [Fact]
        public void GetTypesThatImplementedByInterfaceShouldReturnTypes()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();

            // Act
            var result = reflectionHelper.GetTypesThatImplementedByInterface(typeof(IBarInterface<>));

            // Assert
            Assert.True(result.All(x => x is IBarInterface<string>));
        }

        [Fact]
        public void GetTypesThatImplementedByInterfaceByNonExistingInterfaceTypeShouldReturnEmptyList()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();

            // Act
            var result = reflectionHelper.GetTypesThatImplementedByInterface(typeof(IFooInterface<>));

            // Assert
            Assert.True(result.ToList().Count == 0);
        }

        [Fact]
        public void InvokeGenericMethodShouldCallGivenMethod()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();
            var barClass = new BarClass();
            var parameter = new FooClass { Data = "test" };

            // Act
            reflectionHelper.InvokeGenericMethod("BarMethod", barClass, parameter, typeof(IBarInterface<>));

            // Assert
            Assert.True(barClass.Data == "test");
        }

        [Fact]
        public void InvokeGenericMethodByNotExistingMethodShouldThrowInvalidOperationException()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();
            var barClass = new BarClass();
            var parameter = new FooClass { Data = "test" };

            // Assert
            Assert.Throws(typeof(InvalidOperationException),
                () => reflectionHelper.InvokeGenericMethod("FailMethod", barClass, parameter, typeof(IBarInterface<>)));
        }

        [Fact]
        public void InvokeGenericMethodByInvalidInterfaceTypeShouldThrowInvalidOperationException()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();
            var barClass = new BarClass();
            var parameter = new FooClass { Data = "test" };

            // Assert
            Assert.Throws(typeof(InvalidOperationException),
                () => reflectionHelper.InvokeGenericMethod("BarMethod", barClass, parameter, typeof(IBarInterface<string>)));
        }

        [Fact]
        public void InvokeGenericMethodByNullOrWhitespaceNameShouldThrowArgumentNullException()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();
            var barClass = new BarClass();
            var parameter = new FooClass { Data = "test" };

            // Assert
            Assert.Throws(typeof(ArgumentNullException),
                () => reflectionHelper.InvokeGenericMethod(string.Empty, barClass, parameter, typeof(IBarInterface<>)));
        }

        [Fact]
        public void InvokeGenericMethodByNullObjShouldThrowArgumentNullException()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();
            var parameter = new FooClass { Data = "test" };

            // Assert
            Assert.Throws(typeof(ArgumentNullException),
                () => reflectionHelper.InvokeGenericMethod("BarMethod", null, parameter, typeof(IBarInterface<>)));
        }

        [Fact]
        public void InvokeGenericMethodByNullParameterShouldThrowArgumentNullException()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();
            var barClass = new BarClass();

            // Assert
            Assert.Throws(typeof(ArgumentNullException),
                () => reflectionHelper.InvokeGenericMethod("BarMethod", barClass, null, typeof(IBarInterface<>)));
        }

        [Fact]
        public void InvokeGenericMethodByNullGenericInterfaceTypeShouldThrowArgumentNullException()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();
            var barClass = new BarClass();
            var parameter = new FooClass { Data = "test" };

            // Assert
            Assert.Throws(typeof(ArgumentNullException),
                () => reflectionHelper.InvokeGenericMethod("BarMethod", barClass, parameter, null));
        }

        [Fact]
        public void FilterObjectsByInterfaceShouldReturnFilteredObjects()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();

            // Act
            var result = reflectionHelper.FilterObjectsByInterface(new List<object>
            {
                new FooClass(),
                new BarClass()
            }, typeof(IBarInterface<string>));

            // Assert
            Assert.True(result.Count() == 1);
        }

        [Fact]
        public void FilterObjectsByInterfaceWithNonExistingObjectsShouldReturnEmptyList()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();

            // Act
            var result = reflectionHelper.FilterObjectsByInterface(new List<object>
            {
                new BarClass()
            }, typeof(IBarInterface<string>));

            // Assert
            Assert.True(result.Count() == 0);
        }

        [Fact]
        public void FilterObjectsByInterfaceWithNullObjectsShouldThrowArgumentNullException()
        {
            // Arrange
            var reflectionHelper = new ReflectionHelper();

            // Assert
            Assert.Throws(typeof(ArgumentNullException),
                () => reflectionHelper.FilterObjectsByInterface<string>(null, typeof(void)));
        }

        public class FooClass : IBarInterface<string>
        {
            public string Data { get; set; }
        }

        public class BarClass
        {
            public string Data { get; set; }

            public void BarMethod<T>(IBarInterface<T> param)
            {
                this.Data = param.Data;
            }
        }

        public interface IBarInterface<T>
        {
            string Data { get; set; }
        }

        public interface IFooInterface<T>
        {
        }

        public class FooWorker : IBarInterface<string>
        {
            public string Data { get; set; }
        }
    }
}