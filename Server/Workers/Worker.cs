using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SignalRDemo.DataStorage;
using SignalRDemo.Hubs;

namespace SignalRDemo
{
    /// <summary>
    /// The worker.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService"/>
    public class Worker : BackgroundService
    {
        #region Properties

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// The hub.
        /// </summary>
        private readonly IHubContext<SignalRHub, ISignalRHub> hub;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        private ILogger<Worker> Logger
        {
            get
            {
                return this.serviceProvider.GetRequiredService<ILogger<Worker>>();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker"/> class.
        /// </summary>
        /// <param name="hub">The hub.</param>
        public Worker(IServiceProvider serviceProvider, IHubContext<SignalRHub, ISignalRHub> hub)
        {
            this.serviceProvider = serviceProvider;
            this.hub = hub;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This method is called when the <see
        /// cref="T:Microsoft.Extensions.Hosting.IHostedService"/> starts. The implementation should
        /// return a task that represents the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">
        /// Triggered when <see
        /// cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)"/>
        /// is called.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task"/> that represents the long running operations.
        /// </returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await this.hub.Clients.All.BroadcastChartData(DataManager.GetData());

                this.Logger.LogDebug("Sent data to all users at {0}", DateTime.UtcNow);

                await Task.Delay(5000, stoppingToken);
            }
        }

        #endregion
    }
}