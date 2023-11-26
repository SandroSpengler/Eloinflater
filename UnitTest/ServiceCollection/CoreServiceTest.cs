using Amazon.Runtime.Internal.Util;
using Core.Interfaces;
using Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UnitTest.ServiceCollection
{
    public class CoreServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Ignore("not sure how to unit test DI")]
        public void ShouldRegisterServices()
        {
            var _serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            ILoggerFactory loggerFactory = new Mock<ILoggerFactory>().Object;

            // Core.Extensions.CoreServiceCollection.SetupServiceCollection(_serviceCollection);

            var build = _serviceCollection.BuildServiceProvider(new ServiceProviderOptions()
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });

            var repository = build.GetService<ISummonerByLeagueRepository>();

            Assert.IsNotNull(repository);
        }
    }
}