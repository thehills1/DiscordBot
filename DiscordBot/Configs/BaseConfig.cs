using DiscordBot.Configs.Utils;
using Newtonsoft.Json;

namespace DiscordBot.Configs
{
    public abstract class BaseConfig<T> : JsonSerializable<T> where T : BaseConfig<T>
    {
		public const string ConfigsDirectory = "configs";
		public const string BaseFileName = "config.json";

		[JsonIgnore]
		public string ConfigPath { get; private set; }

		static BaseConfig()
		{
			CustomSettings = new JsonSerializerSettings();
			CustomSettings.MissingMemberHandling = MissingMemberHandling.Error;
			CustomSettings.Formatting = Formatting.Indented;
		}

		public static T LoadOrCreate(string path)
		{
			if (!File.Exists(path))
			{
				var config = (T)typeof(T).GetConstructors().First().Invoke(null);

				config.ConfigPath = path;
				config.Save();

				return config;
			}

			return Load(path);
		}

		public static T Load(string path)
		{
			string json = File.ReadAllText(path);

			var config = Deserialize(json);
			config.ConfigPath = path;
			return config;
		}

		public void Save()
		{
			Save(ConfigPath);
		}

		public void Save(string path)
		{
			var json = Serialize();

			var dirName = Path.GetDirectoryName(path);

			if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
			{
				Directory.CreateDirectory(dirName);
			}

			File.WriteAllText(path, json);
		}
	}
}
