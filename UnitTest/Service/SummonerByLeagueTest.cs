using Amazon.Runtime.Internal.Util;
using AutoFixture;
using AutoMapper;
using Core.Interfaces;
using Core.Model;
using Core.Model.Riot_Games;
using Core.Repository;
using Dataminer.Services;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Service
{
    public class SummonerByLeagueTest
    {
        private Mock<ILogger<MiningService>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISummonerByLeagueRepository> _repositoryMock;
        private Mock<ISummonerRepository> _repositoryMockSummoner;
        private Mock<IRiotGamesApi> _riotGamesApiMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<MiningService>>();
            ILogger<MiningService> logger = _loggerMock.Object;
            _mapperMock = new Mock<IMapper>();

            _repositoryMock = new Mock<ISummonerByLeagueRepository>();
            _repositoryMockSummoner = new Mock<ISummonerRepository>();
            _riotGamesApiMock = new Mock<IRiotGamesApi>();

            var fixture = new Fixture();

            var entries = fixture.CreateMany<Entry>(300).ToList();
            var entriesRG = fixture.CreateMany<RGApiEntry>(300).ToList();

            var sblChallenger = fixture.Build<SummonerByLeague>()
                .Without(summoner => summoner._id)
                .With(sbl => sbl.tier, "MASTER")
                .With(sbl => sbl.entries, entries)
                .CreateMany(3);

            var sblRiotChallenger = fixture.Build<RGApiSummonerByLeague>()
                .With(sbl => sbl.tier, "CHALLENGER")
                .With(sbl => sbl.entries, entriesRG)
                .Create();

            _repositoryMock.Setup(x => x.findSummonerByLeagueWithFilter(It.IsAny<FilterDefinition<SummonerByLeague>>())).Returns(Task.FromResult<IEnumerable<SummonerByLeague>>(sblChallenger));

            _riotGamesApiMock.Setup(api =>
                api.GetSummonerByLeague(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )).Returns(Task.FromResult(sblRiotChallenger));
        }

        [Test]
        public async Task shouldUpdateSummoners()
        {
            var summonerByLeagueService = new MiningService(_loggerMock.Object, _mapperMock.Object, _repositoryMock.Object, _repositoryMockSummoner.Object, _riotGamesApiMock.Object);

            await summonerByLeagueService.refreshSummonerByLeague();

            _riotGamesApiMock.Verify(d =>
                d.GetSummonerByLeague(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                    Times.Exactly(3)
                );
        }

        [Test]
        public void shouldBeUpdateable()
        {
            var summonerByLeagueService = new MiningService(_loggerMock.Object, _mapperMock.Object, _repositoryMock.Object, _repositoryMockSummoner.Object, _riotGamesApiMock.Object);

            long Date_26_2023 = 1690368231392;
            long Interval_12H = 12 * 60 * 60 * 1000;

            var result = summonerByLeagueService.isUpdateable(Date_26_2023, Interval_12H);

            Assert.IsTrue(result);
        }

        [Test]
        public void shouldNotBeUpdateable()
        {
            var summonerByLeagueService = new MiningService(_loggerMock.Object, _mapperMock.Object, _repositoryMock.Object, _repositoryMockSummoner.Object, _riotGamesApiMock.Object);

            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long currentDate = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond) * 1000;
            long Interval_12H = 12 * 60 * 60 * 1000;

            var result = summonerByLeagueService.isUpdateable(currentDate, Interval_12H);

            Assert.IsFalse(result);
        }
    }
}