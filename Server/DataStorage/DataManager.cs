using System;
using System.Collections.Generic;
using SignalRDemo.Models;

namespace SignalRDemo.DataStorage
{
    /// <summary>
    /// The data manager.
    /// </summary>
    public class DataManager
    {
        #region Public Methods

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>Random list of numbers from 1 to 40.</returns>
        public static List<ChartModel> GetData()
        {
            Random random = new Random();
            return new List<ChartModel>()
            {
               new ChartModel { Data = new List<int> { random.Next(1, 40) }, Label = "Data1" },
               new ChartModel { Data = new List<int> { random.Next(1, 40) }, Label = "Data2" },
               new ChartModel { Data = new List<int> { random.Next(1, 40) }, Label = "Data3" },
               new ChartModel { Data = new List<int> { random.Next(1, 40) }, Label = "Data4" }
            };
        }

        #endregion
    }
}