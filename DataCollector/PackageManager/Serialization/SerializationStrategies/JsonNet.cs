using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PackageManager.Serialization.SerializationStrategies
{
	public class JsonNet : ISerializationStrategy
	{
		public string Deserialize(string json)
		{
			List<object> g = new List<object>();
			using (StringReader reader = new StringReader(json))
			{
				string tableData = "";
				while ((tableData = reader.ReadLine()) != null)
				{
					var tt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(tableData, typeof(DataTable));
					g.Add(tt);
				}

			}
			return g.ToString();
		}

		public string Serialize(IEnumerable<DataTable> data)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			StringBuilder jsonString = new StringBuilder();

			//foreach (var turbine in data)
			//{
				string jsonPart = Newtonsoft.Json.JsonConvert.SerializeObject(data);
				jsonString.Append(jsonPart);
			//}

			sw.Stop();
			long time = sw.ElapsedMilliseconds;

			return jsonString.ToString();
		}
	}
}
