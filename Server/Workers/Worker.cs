using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
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
        #region Constants

        /// <summary>
        /// The broadcast method name.
        /// </summary>
        private const string broadcastMethodName = "broadcastChannel";

        #endregion

        #region Properties

        /// <summary>
        /// The hub.
        /// </summary>
        private IHubContext<SignalRHub> hub;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker"/> class.
        /// </summary>
        /// <param name="hub">The hub.</param>
        public Worker(IHubContext<SignalRHub> hub)
        {
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
                await this.hub.Clients.All.SendAsync(broadcastMethodName, DataManager.GetData());
                await Task.Delay(5000, stoppingToken);
            }
        }

        #endregion
    }
}