using PackageManager.Compression.CompressionStrategies;
using PackageManager.PackageBuilder;
using PackageManager.Serialization.SerializationStrategies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataRelay.QueueProcessor
{
	public class QueueManager
    {
		static Queue<Package> queue = new Queue<Package>();
		readonly static object syncLock = new object();

		static QueueManager()
		{
			Task.Factory.StartNew(
				() => QueueProcessorStart(queue, syncLock)
			);
		}

		public static void Enqueue(Package package)
		{
			lock (syncLock)
			{
				queue.Enqueue(package);
			}
		}

		private static void QueueProcessorStart(Queue<Package> queue, object syncLock)
		{
			try
			{
				List<byte[]> packageBinaryList = new List<byte[]>();

				System.Timers.Timer timer = new System.Timers.Timer();
				TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
				timer.Interval = 200;
				timer.AutoReset = true;
				timer.Elapsed += (obj, args) =>
				{
					PackageManager.PackageManager pm = new PackageManager.PackageManager(new JsonNet(), new GZipStreamCompression());
					Package package = null;

					lock (syncLock)
					{
						if (queue.Any())
							package = queue.Dequeue();
					}

					if (package != null)
					{
						byte[] packageBinary = pm.ToBinaryFormat(package);
						packageBinaryList.Add(packageBinary);
					}

					tcs.TrySetResult(true);
				};

				timer.Start();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
    }
}
