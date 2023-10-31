using Core.Model;
using Core.Model.Database;
using Core.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Repository
{
    public class SummonerTest
    {
        private Mock<IMongoDatabase> _mongoDBMock;
        private Mock<ILogger<Summoner>> _loggerMock;
        private Mock<MongoCollectionBase<Summoner>> _summonerCollectionMock;
        private Mock<MongoCollectionSettings> _collectionSettingsMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<Summoner>>();
            ILogger<Summoner> logger = _loggerMock.Object;

            _summonerCollectionMock = new Mock<MongoCollectionBase<Summoner>>();
            _collectionSettingsMock = new Mock<MongoCollectionSettings>();
            _mongoDBMock = new Mock<IMongoDatabase>();

            _mongoDBMock
            .Setup(db =>
                db.GetCollection<Summoner>("summoner", null))
            .Returns(_summonerCollectionMock.Object);

            var mockAsyncCursor = new Mock<IAsyncCursor<Summoner>>();

            Summoner summoner = new Summoner();

            //mockAsyncCursor.Setup(_ => _.Current).Returns(summoner);
            //mockAsyncCursor.SetupSequence
            //(_ => _.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            //_summonerCollectionMock
            //.Setup(_ => _.FindAsync(
            //    It.IsAny<FilterDefinition<SummonerByLeague>>(),
            //    It.IsAny<FindOptions<SummonerByLeague, SummonerByLeague>>(),
            //    It.IsAny<CancellationToken>()
            //))
            //.ReturnsAsync(mockAsyncCursor.Object);
        }
    }
}