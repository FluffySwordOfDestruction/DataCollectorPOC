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

		public string Serialize(object data)
		{
			StringBuilder jsonString = new StringBuilder();

			string jsonPart = Newtonsoft.Json.JsonConvert.SerializeObject(data);
			jsonString.Append(jsonPart);

			return jsonString.ToString();
		}
	}
}
