using DAL;
using DataCollector.Encryptor;
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
				//Get the data
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

                var tableNames = GetTableNames(turbineGroupedByTimeStamp);
				Console.WriteLine("Obtaining Data for " + tableNames);
				Console.WriteLine("Timestam: " + i.ToString() + ", Time Elapsed: = " + st.ElapsedMilliseconds);


				//send to serializator
				var serializedData = DataTransformator.SerializeData(turbineData);
               

                IEncryptor encryptor = new AESEncryptor();
                st.Reset();
                st.Start();
                var securedData = encryptor.EncryptData(serializedData);
                st.Stop();
                Console.WriteLine("Encrypting data took: " + st.ElapsedMilliseconds);

				counterPackage++;
                Console.WriteLine("Counter = " + counterPackage);

                var package = DataTransformator.PreaparePackage(securedData.Data, PackageManager.PackageBuilder.EncryptionAlgorithmType.AES, PackageManager.PackageBuilder.PayloadDataType.Data10Minutes, i);
                package.EncryptedKey = securedData.Key;
                package.EncryptedIV = securedData.Vector;
                var binaryPackage = DataTransformator.GetBinary(package);

                // send to data relay

                DataRelayService.DataRelayServiceClient client = new DataRelayService.DataRelayServiceClient();
                var result = client.Send(binaryPackage);

                turbinaDataCollector.SetTimeStampsForTables(turbineGroupedByTimeStamp);

            }
		}

        private static object GetTableNames(IGrouping<DateTime, TurbineInfo> turbineGroupedByTimeStamp)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var turbineInfo in turbineGroupedByTimeStamp)
                sb.Append(turbineInfo.TableName + ", ");
            return sb.ToString();
        }

        internal static IGrouping<DateTime, TurbineInfo> ChangeCurrentTimeStamp(IGrouping<DateTime, TurbineInfo> turbineGroupedByTimeStamp, DateTime i)
		{
			foreach (var turbineInfo in turbineGroupedByTimeStamp)
				turbineInfo.TimeStamp = i;
			return turbineGroupedByTimeStamp;
		}
	}
}
