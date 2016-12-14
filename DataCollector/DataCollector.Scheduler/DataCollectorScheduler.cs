using DataCollector.Scheduler.Jobs;
using DataCollector.Scheduler.Triggers;
using Quartz;
using Quartz.Impl;
using System.Configuration;
using System.ServiceProcess;

namespace DataCollector.Scheduler
{
	public partial class DataCollectorScheduler : ServiceBase
	{
		IScheduler scheduler;

		public DataCollectorScheduler()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			ISchedulerFactory schedFact = new StdSchedulerFactory();

			scheduler = schedFact.GetScheduler();
			scheduler.Start();

			string dataCollectCronScheduler = ConfigurationManager.AppSettings["dataCollectJobCron"];

			ITrigger cronTrigger = CronTrigger.CreateTrigger(dataCollectCronScheduler);
			IJobDetail dataCollectorJob = DataCollectorJob.Create();

			// Schedule Jobs
			scheduler.ScheduleJob(dataCollectorJob, cronTrigger);
		}

		protected override void OnStop()
		{
			scheduler.Shutdown();
		}
	}
}
