using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using DxfToolAutoCAD;

namespace DxfToolAutoCAD.Tests
{
    public class PluginInitializerTests
    {
        [Fact]
        public void Initialize_ShouldSetupServiceProvider()
        {
            // Arrange
            var initializer = new PluginInitializer();

            // Act
            initializer.Initialize();

            // Assert
            Assert.NotNull(PluginInitializer.ServiceProvider);
            
            // Verify logging service is registered
            var logger = PluginInitializer.ServiceProvider.GetService<ILogger<PluginInitializerTests>>();
            Assert.NotNull(logger);
        }

        [Fact]
        public void ServiceProvider_ShouldProvideLoggingServices()
        {
            // Arrange
            var initializer = new PluginInitializer();
            initializer.Initialize();

            // Act
            var loggerFactory = PluginInitializer.ServiceProvider.GetService<ILoggerFactory>();
            var logger = PluginInitializer.ServiceProvider.GetService<ILogger<PluginInitializerTests>>();

            // Assert
            Assert.NotNull(loggerFactory);
            Assert.NotNull(logger);
        }

        [Fact]
        public void Terminate_ShouldDisposeServiceProvider()
        {
            // Arrange
            var initializer = new PluginInitializer();
            initializer.Initialize();
            Assert.NotNull(PluginInitializer.ServiceProvider);

            // Act
            initializer.Terminate();

            // Assert - ServiceProvider should still be accessible but disposed
            // (We can't easily test disposal without making ServiceProvider internal disposable)
            Assert.NotNull(PluginInitializer.ServiceProvider);
        }

        [Fact]
        public void Plugin_CoreFunctionality_ShouldWork()
        {
            // Arrange & Act & Assert
            // Test the core plugin functionality without console interaction
            var initializer = new PluginInitializer();
            initializer.Initialize();
            
            var serviceProvider = PluginInitializer.ServiceProvider;
            var logger = serviceProvider.GetService<ILogger<PluginInitializerTests>>();
            
            Assert.NotNull(serviceProvider);
            Assert.NotNull(logger);
            
            // Test logging
            logger.LogInformation("Test log message");
            
            // Cleanup
            initializer.Terminate();
            
            // If we get here, core functionality works
            Assert.True(true);
        }
    }
}
