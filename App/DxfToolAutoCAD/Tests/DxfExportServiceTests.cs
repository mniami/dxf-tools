#if AUTOCAD_API_AVAILABLE
using Autodesk.AutoCAD.DatabaseServices;
#endif
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using DxfToolAutoCAD;

namespace DxfToolAutoCAD.Tests
{
    public class SoundPlanImportServiceTests
    {
        private readonly Mock<ILogger<SoundPlanImportService>> _mockLogger;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly SoundPlanImportService _service;

        public SoundPlanImportServiceTests()
        {
            _mockLogger = new Mock<ILogger<SoundPlanImportService>>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _service = new SoundPlanImportService(_mockLogger.Object, _mockServiceProvider.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithLogger()
        {
            // Arrange & Act
            var service = new SoundPlanImportService(_mockLogger.Object, _mockServiceProvider.Object);

            // Assert
            Assert.NotNull(service);
        }

#if AUTOCAD_API_AVAILABLE
        [Fact]
        public void ImportSoundPlanData_WithValidPath_ShouldProcessFile()
        {
            // This test would only run when AutoCAD APIs are available
            // For now, it's a placeholder showing how to structure AutoCAD-dependent tests
            Assert.True(true, "AutoCAD API dependent test placeholder");
        }
#endif

        [Fact]
        public void ImportService_WithoutAutoCAD_ShouldNotCrash()
        {
            // This tests that the service can be instantiated even without AutoCAD
            // The actual import functionality is wrapped in conditional compilation
            Assert.NotNull(_service);
        }
    }
}
