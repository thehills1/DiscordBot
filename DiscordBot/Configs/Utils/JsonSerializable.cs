using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DiscordBot.Configs.Utils
{
    public abstract class JsonSerializable<T>
    {
		protected static JsonSerializer Serializer { get; private set; }

		public override string ToString()
		{
			return Serialize();
		}

		public static T Deserialize(string json)
		{
			return JsonConvert.DeserializeObject<T>(json, GetSettings());
		}

		public string Serialize()
		{
			return JsonConvert.SerializeObject(this, GetSettings());
		}

		protected static JsonSerializerSettings CustomSettings { get; set; }
		public static JsonSerializerSettings GetSettings()
		{
			if (CustomSettings != null)
			{
				Serializer ??= JsonSerializer.Create(CustomSettings);
				return CustomSettings;
			}

			var settings = new JsonSerializerSettings();
			settings.NullValueHandling = NullValueHandling.Ignore;
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			settings.Converters.Add(new StringEnumConverter());

			Serializer ??= JsonSerializer.Create(settings);

			return settings;
		}
	}
}
