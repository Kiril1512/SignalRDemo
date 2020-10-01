using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using SignalRDemo;
using SignalRDemo.Hub.Interfaces;
using SignalRDemo.Hubs;
using SignalRDemo.Models;

using Xunit;

namespace SignalRDemoTests.Tests
{
    public class WorkerTests
    {
        #region Mocks

        /// <summary>
        /// The hub context mock.
        /// </summary>
        public static readonly Mock<IHubContext<SignalRHub, ISignalRHub>> hubContextMock = new Mock<IHubContext<SignalRHub, ISignalRHub>>();

        #endregion

        #region Tests

        /// <summary>
        /// Should execute worker and broadcast chart data.
        /// </summary>
        [Fact]
        public async Task ShouldExecuteWorkerAndBroadcastChartData()
        {
            using (Worker worker = BuildWorker())
            {
                await worker.StartAsync(CancellationToken.None).ConfigureAwait(false);

                await Task.Delay(5000);

                await worker.StopAsync(CancellationToken.None).ConfigureAwait(false);

                // Should execute 2 times, at the start and after 5 seconds

                hubContextMock.Verify(mock => mock.Clients.All.BroadcastChartData(It.IsAny<List<ChartModel>>()), Times.Exactly(2));
            };
        }

        #endregion

        #region Private Methods

        private Worker BuildWorker()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging();

            IServiceProvider provider = services.BuildServiceProvider();

            // Create the logger to the worker

            ILoggerFactory factory = provider.GetService<ILoggerFactory>();

            ILogger<Worker> logger = factory.CreateLogger<Worker>();

            // Add dependency

            services.AddSingleton(logger);

            // Setup hub mock

            hubContextMock.Setup(mock => mock.Clients.All.BroadcastChartData(It.IsAny<List<ChartModel>>())).Returns(Task.CompletedTask);

            // Return worker

            return new Worker(services.BuildServiceProvider(), hubContextMock.Object);
        }

        #endregion
    }
}