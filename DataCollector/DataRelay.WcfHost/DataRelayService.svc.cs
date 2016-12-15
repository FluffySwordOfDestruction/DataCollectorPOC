using DataRelay.QueueProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DataRelay.WcfHost
{
	public class DataRelayService : IDataRelayService
	{
		public bool Send(byte[] package)
		{
			return DataProcessor.RecieveData(package);
		}

		public bool KnockKnock(byte[] package)
		{
			return true;
		}
	}
}
