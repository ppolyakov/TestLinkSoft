using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using SocialNetworkAnalyzerApi.Data;
using SocialNetworkAnalyzerApi.Entities;
using SocialNetworkAnalyzerApi.Services.ImportService;
using System.Text;

namespace SocialNetworkAnalayzerApiTests.ImportServiceTests
{
    [TestClass]
    public class ImportServiceTests
    {
        private DbContextOptions<DataContext> _options = null!;

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique name of DB for each test
            .Options;

        }

        [TestMethod]
        public async Task Import_ValidData_ImportsSuccessfully()
        {
            // Arrange
            var mockSet = new Mock<DbSet<ImportData>>();

            var service = new ImportService(GetContextWithData(_options, mockSet.Object));

            var lines = new List<string>
            {
                "0 1",
                "0 2",
                "1 3"
            };

            var fileContent = string.Join("\n", lines);
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)));

            // Act
            await service.Import("TestImport", formFileMock.Object, CancellationToken.None);

            // Assert
            mockSet.Verify(m => m.Add(It.IsAny<ImportData>()), Times.Once);
            mockSet.Verify(m => m.AddRange(It.IsAny<IEnumerable<ImportData>>()), Times.Once);
            mockSet.Verify(m => m.Remove(It.IsAny<ImportData>()), Times.Never);
            mockSet.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<ImportData>>()), Times.Never);
        }

        [TestMethod]
        public async Task Import_FileIsNull_ThrowsException()
        {
            // Arrange
            var mockSet = new Mock<DbSet<ImportData>>();

            var service = new ImportService(GetContextWithData(_options, mockSet.Object));

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await service.Import("FileNullImport", null, CancellationToken.None);
            });

            Assert.AreEqual("Error importing data: File is null", exception.Message);
        }

        [TestMethod]
        public async Task Import_InvalidData_FormatError_ThrowsException()
        {
            // Arrange
            var mockSet = new Mock<DbSet<ImportData>>();

            var service = new ImportService(GetContextWithData(_options, mockSet.Object));

            var invalidData = "invalid_line";
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes(invalidData)));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await service.Import("InvalidDataImport", formFileMock.Object, CancellationToken.None);
            });

            mockSet.Verify(m => m.Add(It.IsAny<ImportData>()), Times.Never);
            mockSet.Verify(m => m.Remove(It.IsAny<ImportData>()), Times.Never);
            mockSet.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<ImportData>>()), Times.Never);
        }

        private DataContext GetContextWithData(DbContextOptions<DataContext> options, DbSet<ImportData> mockSet)
        {
            var context = new DataContext(options);

            context.Imports = mockSet;

            var importDataList = new List<ImportData>
            {
                new() { Id = 1, Name = "Import1", Data = "[{ 'UserId': 0, 'FriendId': 1 }]" },
                new() { Id = 2, Name = "Import2", Data = "[{ 'UserId': 1, 'FriendId': 2 }]" }
            };

            mockSet.AddRange(importDataList);
            context.SaveChanges();

            return context;
        }
    }
}