using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Core.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;

namespace UnitTest.Repository
{
    public class SummonerByLeagueTest
    {
        private Mock<IMongoDatabase> mongoDBMock;
        private Mock<ILogger<SummonerByLeagueRepository>> loggerMock;
        private Mock<MongoCollectionBase<SummonerByLeague>> summonerByLeagueCollectionMock;
        private Mock<MongoCollectionSettings> collectionSettingsMock;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<SummonerByLeagueRepository>>();
            ILogger<SummonerByLeagueRepository> logger = loggerMock.Object;

            summonerByLeagueCollectionMock = new Mock<MongoCollectionBase<SummonerByLeague>>();
            collectionSettingsMock = new Mock<MongoCollectionSettings>();
            mongoDBMock = new Mock<IMongoDatabase>();

            mongoDBMock
            .Setup(db =>
                db.GetCollection<SummonerByLeague>("summonerbyleagueschemas", null))
            .Returns(summonerByLeagueCollectionMock.Object);
        }

        [Test]
        public async Task ShouldReturnSummonersFromDB()
        {
            SummonerByLeague[] summonerbyleague = new[] { new SummonerByLeague() };

            var mockAsyncCursor = new Mock<IAsyncCursor<SummonerByLeague>>();

            mockAsyncCursor.Setup(_ => _.Current).Returns(summonerbyleague);
            mockAsyncCursor.SetupSequence
            (_ => _.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            summonerByLeagueCollectionMock
            .Setup(_ => _.FindAsync(
                It.IsAny<FilterDefinition<SummonerByLeague>>(),
                It.IsAny<FindOptions<SummonerByLeague, SummonerByLeague>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(mockAsyncCursor.Object);

            var summonerByLeagueRepository = new SummonerByLeagueRepository(loggerMock.Object, mongoDBMock.Object);

            var builder = Builders<SummonerByLeague>.Filter.Empty;

            var result = await summonerByLeagueRepository.findSummonerByLeagueWithFilter(builder);

            var list = result.ToList();

            Assert.That(list.Count, Is.EqualTo(1));
        }
    }
}