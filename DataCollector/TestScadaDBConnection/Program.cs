using DataCollector.Api;
using System;

namespace TestScadaDBConnection
{
	class Program
    {
        static void Main(string[] args)
        {
			DataCollectorApi.ProcessTurbineData();

            Console.ReadKey();
        }
	}
}
