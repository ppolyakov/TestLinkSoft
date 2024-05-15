using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using SocialNetworkAnalyzerApi.Data;
using SocialNetworkAnalyzerApi.Entities;
using SocialNetworkAnalyzerApi.Services.StatisticService;
using System.Runtime.Intrinsics.X86;

namespace SocialNetworkAnalyzerApiTests
{
    public class ImportsTests
    {
        [Fact]
        public async Task GetAllUsersCount_WithValidImport_ReturnsCorrectCount()
        {
            // Arrange
            var importData = new List<ImportData>
            {
                new ImportData()
                { 
                    Id = 1, 
                    Name = "ValidImport",
                    Data = "{ \"User1\" = 0, \"User2\" = 1 },{ \"User1\" = 0, \"User2\" = 2 },{ \"User1\" = 0, \"User2\" = 3 },{ \"User1\" = 1, \"User2\" = 2 },{ \"User1\" = 1, \"User2\" = 3 }"
                }
            };
            var importJson = JsonConvert.SerializeObject(importData);

            var mockDbContext = new Mock<DataContext>();
            var mockDbSet = GetMockDbSet(importData);

            mockDbContext.Setup(c => c.Imports).Returns(mockDbSet.Object);

            var service = new StatisticService(mockDbContext.Object);

            // Act
            int count = await service.GetAllUsersCount("ValidImport", CancellationToken.None);

            // Assert
            Assert.Equal(4, count); // Ожидаемый результат: 4 уникальных пользователей
        }

        //[Fact]
        //public async Task GetAverageFriends_WithValidImport_ReturnsCorrectAverage()
        //{
        //    // Arrange
        //    var import = new Import
        //    {
        //        Id = 1,
        //        Name = "ValidImport",
        //        Data = JsonConvert.SerializeObject(new List<Friendship>
        //    {
        //        new Friendship { User1 = 0, User2 = 1 },
        //        new Friendship { User1 = 0, User2 = 2 },
        //        new Friendship { User1 = 0, User2 = 3 },
        //        new Friendship { User1 = 1, User2 = 2 },
        //        new Friendship { User1 = 1, User2 = 3 }
        //    })
        //    };

        //    var dbContextMock = new Mock<YourDbContext>();
        //    var mockImports = new List<Import> { import };
        //    var mockDbSet = GetQueryableMockDbSet(mockImports);
        //    dbContextMock.Setup(c => c.Imports).Returns(mockDbSet.Object);

        //    var service = new YourService(dbContextMock.Object);

        //    // Act
        //    double averageFriends = await service.GetAverageFriends("ValidImport", CancellationToken.None);

        //    // Assert
        //    Assert.Equal(2.5, averageFriends); // Ожидаемый результат: 2.5 среднее количество друзей
        //}

        private Mock<DbSet<T>> GetMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockDbSet = new Mock<DbSet<T>>();

            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return mockDbSet;
        }

        private Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var data = sourceList.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }
    }
}