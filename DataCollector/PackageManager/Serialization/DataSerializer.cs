using PackageManager.Serialization.SerializationStrategies;
using System.Collections.Generic;
using System.Data;

namespace PackageManager.Serialization
{
	internal class DataSerializer
	{
		private ISerializationStrategy _serializationStrategy;

		public DataSerializer(ISerializationStrategy serializationStrategy)
		{
			_serializationStrategy = serializationStrategy;
		}

		public string Deserialize(string data)
		{
			return _serializationStrategy.Deserialize(data);
		}

		public string Serialize(object data)
		{
			return _serializationStrategy.Serialize(data);
		}
	}
}