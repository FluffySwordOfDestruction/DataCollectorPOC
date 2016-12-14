using System.ServiceProcess;

namespace DataCollector.Scheduler
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new DataCollectorScheduler()
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
