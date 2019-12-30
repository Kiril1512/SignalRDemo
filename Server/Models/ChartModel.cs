using System.Collections.Generic;

namespace SignalRDemo.Models
{
    /// <summary>
    /// Chart model.
    /// </summary>
    public class ChartModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartModel"/> class.
        /// </summary>
        public ChartModel()
        {
            Data = new List<int>();
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public List<int> Data { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label { get; set; }
    }
}