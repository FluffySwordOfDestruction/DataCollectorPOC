using DAL;
using DataCollector.Api.DataManager;
using System;

namespace DataCollector.Api
{
	public class DataCollectorApi
    {
		#region Public API Methods

		public static void ProcessTurbineData()
		{
            DateTime endDate = new DateTime(2013, 10, 08, 12, 00, 00);

			var turbinaDataCollector = new VestasTurbinesData();
			var turbineList = turbinaDataCollector.GetTurbineList();

			DataProcessor.ProcessAllTurbineData(turbineList, turbinaDataCollector, endDate);
		}

		#endregion
	}
}
