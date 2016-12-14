using DAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DataCollector.Api.DataManager
{
	public class DataProcessor
	{
		internal static void ProcessAllTurbineData(IEnumerable<IEnumerable<TurbineInfo>> turbineList, VestasTurbinesData turbinaDataCollector, DateTime endDate)
		{
			foreach (var turbineTables in turbineList)
				ProcessTablesDataForTurbine(turbineTables, endDate, turbinaDataCollector);

			turbinaDataCollector.CloseConnection();
		}

		internal static void ProcessTablesDataForTurbine(IEnumerable<TurbineInfo> turbineTables, DateTime endDate, VestasTurbinesData turbinaDataCollector)
		{
			foreach (var turbineGroupedByTimeStamp in turbineTables.GroupBy(p => p.TimeStamp))
				ProccessTablesDataGroupedByTimeStamp(turbineGroupedByTimeStamp, endDate, turbinaDataCollector);
		}

		internal static void ProccessTablesDataGroupedByTimeStamp(IGrouping<DateTime, TurbineInfo> turbineGroupedByTimeStamp, DateTime endDate, VestasTurbinesData turbinaDataCollector)
		{
			long counterPackage = 0;

			for (DateTime i = turbineGroupedByTimeStamp.Key; i < endDate; i = i.AddMinutes(10))
			{
				turbineGroupedByTimeStamp = ChangeCurrentTimeStamp(turbineGroupedByTimeStamp, i);

				if (counterPackage % 1000 == 0)
				{
					turbinaDataCollector.CloseConnection();
					turbinaDataCollector.OpenConnection();
				}
				//pobierz dane
				Console.WriteLine("//////////////////////");
				Stopwatch st = new Stopwatch();
				st.Start();

				var turbineData = turbinaDataCollector.GetDataFromTurbines(turbineGroupedByTimeStamp);

				if (!turbineData.Any())
				{
					Console.WriteLine("No data available for: " + i.ToString());
					continue;
				}

				st.Stop();

				StringBuilder sb = new StringBuilder();
				foreach (var turbineInfo in turbineGroupedByTimeStamp)
					sb.Append(turbineInfo.TableName + ", ");

				Console.WriteLine("Obtaining Data for " + sb.ToString());
				Console.WriteLine("Timestam: " + i.ToString() + ", Time Elapsed: = " + st.ElapsedMilliseconds);

				//wyslij do serializatora
				var serializedData = DataTransformator.SerializeData(turbineData);

				counterPackage++;
				Console.WriteLine("Counter = " + counterPackage);
                // wyslij do data realey
                //  var deserialize = serializer.Deserialize(serializedData);
                turbinaDataCollector.SetTimeStampsForTables(turbineGroupedByTimeStamp);

            }
		}

		internal static IGrouping<DateTime, TurbineInfo> ChangeCurrentTimeStamp(IGrouping<DateTime, TurbineInfo> turbineGroupedByTimeStamp, DateTime i)
		{
			foreach (var turbineInfo in turbineGroupedByTimeStamp)
				turbineInfo.TimeStamp = i;
			return turbineGroupedByTimeStamp;
		}
	}
}
