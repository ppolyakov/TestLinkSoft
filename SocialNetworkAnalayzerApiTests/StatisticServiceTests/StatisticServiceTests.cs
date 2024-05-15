using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SocialNetworkAnalyzerApi.Data;
using SocialNetworkAnalyzerApi.Entities;
using SocialNetworkAnalyzerApi.Models;
using SocialNetworkAnalyzerApi.Services.StatisticService;

namespace SocialNetworkAnalayzerApiTests.StatisticServiceTests
{
    [TestClass]
    public class StatisticServiceTests
    {
        private DbContextOptions<DataContext> _options = null!;
        private List<ImportData> _importData = [];

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique name of DB for each test
            .Options;

            _importData = new List<ImportData>
            {
                new ImportData { Id = 1, Name = "ValidImport", Data = JsonConvert.SerializeObject(new List<Friendship>
                {
                    new Friendship(0, 1),
                    new Friendship(0, 2),
                    new Friendship(0, 3),
                    new Friendship(1, 2),
                    new Friendship(1, 3)
                }) }
            };
        }

        [TestMethod]
        public async Task GetAllUsersCount_WithValidImport_ReturnsCorrectCount()
        {
            // Arrange
            using (var context = new DataContext(_options))
            {
                foreach (var item in _importData)
                {
                    context.Imports.Add(item);
                }
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(_options))
            {
                var service = new StatisticService(context);
                // Act
                int count = await service.GetAllUsersCount("ValidImport", CancellationToken.None);

                // Assert
                Assert.AreEqual(4, count); // Expected result: 4 users
            }
        }

        [TestMethod]
        public async Task GetAverageFriends_WithValidImport_ReturnsCorrectAverage()
        {
            // Arrange
            using (var context = new DataContext(_options))
            {
                foreach (var item in _importData)
                {
                    context.Imports.Add(item);
                }
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(_options))
            {
                var service = new StatisticService(context);
                double averageFriends = await service.GetAverageFriends("ValidImport", CancellationToken.None);

                // Assert
                Assert.AreEqual(2.5, averageFriends); // Expected result: 2.5 average count friend
            }

        }

        [TestMethod]
        public async Task GetAllUsersCount_WithInvalidImportName_ReturnsZero()
        {
            // Arrange
            using (var context = new DataContext(_options))
            {
                foreach (var item in _importData)
                {
                    context.Imports.Add(item);
                }
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(_options))
            {
                var service = new StatisticService(context);

                // Act & Assert
                var exception = await Assert.ThrowsExceptionAsync<Exception>(async () =>
                {
                    await service.GetAllUsersCount("InvalidImport", CancellationToken.None);
                });

                // Assert
                Assert.AreEqual("Error in GetAllUsersCount: Import with name 'InvalidImport' not found.", exception.Message); // Ожидаемый текст исключения
            }
        }

        [TestMethod]
        public async Task GetAverageFriends_WithEmptyImportData_ThrowsException()
        {
            // Arrange
            using (var context = new DataContext(_options))
            {
                foreach (var item in _importData)
                {
                    context.Imports.Add(item);
                }
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(_options))
            {
                var service = new StatisticService(context);
                // Act & Assert
                var exception = await Assert.ThrowsExceptionAsync<Exception>(async () =>
                {
                    await service.GetAverageFriends("EmptyImport", CancellationToken.None);
                });

                // Assert
                Assert.AreEqual("Error in GetAverageFriends: Import with name 'EmptyImport' not found.", exception.Message);
            }
        }
    }
}