using Amazon.Runtime.Internal.Util;
using Core.Repository;
using Dataminer.Services;
using Microsoft.Extensions.Logging;
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
        private Mock<ILogger<SummonerByLeagueService>> _loggerMock;
        private Mock<SummonerByLeagueRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<SummonerByLeagueService>>();
            ILogger<SummonerByLeagueService> logger = _loggerMock.Object;

            _repositoryMock = new Mock<SummonerByLeagueRepository>();
        }

        [Test]
        public void shouldBeUpdateable()
        {
            var summonerByLeagueService = new SummonerByLeagueService(_loggerMock.Object, _repositoryMock.Object);

            long Date_26_2023 = 1690368231392;
            long Interval_12H = 12 * 60 * 60 * 1000;

            var result = summonerByLeagueService.isUpdateable(Date_26_2023, Interval_12H);

            Assert.IsTrue(result);
        }

        [Test]
        public void shouldNotBeUpdateable()
        {
            var summonerByLeagueService = new SummonerByLeagueService(_loggerMock.Object, _repositoryMock.Object);

            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long currentDate = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond) * 1000;
            long Interval_12H = 12 * 60 * 60 * 1000;

            var result = summonerByLeagueService.isUpdateable(currentDate, Interval_12H);

            Assert.IsFalse(result);
        }
    }
}