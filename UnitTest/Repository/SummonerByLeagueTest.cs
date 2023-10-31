using Core.Model;
using Core.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;

namespace UnitTest.Repository
{
    public class SummonerByLeagueTest
    {
        private Mock<IMongoDatabase> _mongoDBMock;
        private Mock<ILogger<SummonerByLeagueRepository>> _loggerMock;
        private Mock<MongoCollectionBase<SummonerByLeague>> _summonerByLeagueCollectionMock;
        private Mock<MongoCollectionSettings> _collectionSettingsMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<SummonerByLeagueRepository>>();
            ILogger<SummonerByLeagueRepository> logger = _loggerMock.Object;

            _summonerByLeagueCollectionMock = new Mock<MongoCollectionBase<SummonerByLeague>>();
            _collectionSettingsMock = new Mock<MongoCollectionSettings>();
            _mongoDBMock = new Mock<IMongoDatabase>();

            _mongoDBMock
            .Setup(db =>
                db.GetCollection<SummonerByLeague>("summonerbyleagueschemas", null))
            .Returns(_summonerByLeagueCollectionMock.Object);

            var mockAsyncCursor = new Mock<IAsyncCursor<SummonerByLeague>>();

            SummonerByLeague[] summonerbyleague = new[] { new SummonerByLeague() };

            mockAsyncCursor.Setup(_ => _.Current).Returns(summonerbyleague);
            mockAsyncCursor.SetupSequence
            (_ => _.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            _summonerByLeagueCollectionMock
            .Setup(_ => _.FindAsync(
                It.IsAny<FilterDefinition<SummonerByLeague>>(),
                It.IsAny<FindOptions<SummonerByLeague, SummonerByLeague>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(mockAsyncCursor.Object);
        }

        [Test]
        public async Task ShouldReturnSummonersFromDB()
        {
            var summonerByLeagueRepository = new SummonerByLeagueRepository(_loggerMock.Object, _mongoDBMock.Object);

            var builder = Builders<SummonerByLeague>.Filter.Empty;

            var result = await summonerByLeagueRepository.findSummonerByLeagueWithFilter(builder);

            var list = result.ToList();

            Assert.That(list.Count, Is.EqualTo(1));
        }
    }
}