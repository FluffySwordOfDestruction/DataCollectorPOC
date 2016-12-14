using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCollector.Scheduler.Triggers
{
	public class CronTrigger
	{
		public static ITrigger CreateTrigger(string cronExpression)
		{
			return TriggerBuilder.Create()
			  .WithIdentity("myTrigger", "group1")
			  .StartNow()
			  .WithCronSchedule(cronExpression)
			  .Build();
		}
	}
}
