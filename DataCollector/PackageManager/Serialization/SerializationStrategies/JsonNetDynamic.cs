using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text;

namespace PackageManager.Serialization.SerializationStrategies
{
	public class JsonNetDynamic : ISerializationStrategy
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

			
				var obj = BindDynamics(data);
				string jsonPart = JsonConvert.SerializeObject(obj, Formatting.Indented);

				jsonString.Append(jsonPart);
			

			sw.Stop();
			long time = sw.ElapsedMilliseconds;

			return jsonString.ToString();
		}

		public static IEnumerable<dynamic> BindDynamics(IEnumerable<DataTable> dataTables)
		{
			foreach (DataTable table in dataTables)
			{
				foreach (DataRow row in table.Rows)
				{
					yield return BindDynamic(row);
				}
			}
		}

		public static dynamic BindDynamic(DataRow dataRow)
		{
			dynamic result = null;
			if (dataRow != null)
			{
				result = new ExpandoObject();
				var resultDictionary = (IDictionary<string, object>)result;
				foreach (DataColumn column in dataRow.Table.Columns)
				{
					var dataValue = dataRow[column.ColumnName];
					resultDictionary.Add(column.ColumnName, DBNull.Value.Equals(dataValue) ? null : dataValue);
				}
			}
			return result;
		}

	}
}
