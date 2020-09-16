using System.Collections.Generic;
using System.Threading.Tasks;

using SignalRDemo.Models;

namespace SignalRDemo.Hubs
{
    public interface ISignalRHub
    {
        /// <summary>
        /// Broadcasts the message.
        /// </summary>
        /// <param name="message">The message.</param>
        Task BroadcastMessage(string message);

        /// <summary>
        /// Broadcasts the message.
        /// </summary>
        /// <param name="message">The chat message.</param>
        Task BroadcastMessage(ChatMessage message);

        /// <summary>
        /// Broadcasts the chart data.
        /// </summary>
        /// <param name="chartData">The chart data.</param>
        Task BroadcastChartData(List<ChartModel> chartData);
    }
}
