using DataCollector.Api;
using Quartz;

namespace DataCollector.Scheduler.Jobs
{
	public class DataCollectorJob : IJob
	{
		public void Execute(IJobExecutionContext context)
		{
			DataCollectorApi.ProcessTurbineData();
		}

		public static IJobDetail Create()
		{
			return JobBuilder.Create<DataCollectorJob>()
			.WithIdentity("dataCollectionJob")
			.Build();
		}
	}
}
