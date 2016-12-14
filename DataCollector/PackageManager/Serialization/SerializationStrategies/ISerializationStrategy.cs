using System.Collections.Generic;
using System.Data;

namespace PackageManager.Serialization.SerializationStrategies
{
	public interface ISerializationStrategy
	{
		string Serialize(IEnumerable<DataTable> data);
		string Deserialize(string json);
	}
}