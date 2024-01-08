using DiscordBot.Database.Enums;

namespace DiscordBot.Configs
{
	public class ShopsConfig : BaseConfig<ShopsConfig>
	{
		/// <summary>
		/// Максимально допустимое количество магазинов на одном сервере.
		/// </summary>
		public int MaxShopsPerServer { get; set; }

		/// <summary>
		/// Каналы для отправки списка магазинов.
		/// </summary>
		public Dictionary<ServerName, ulong> InfoChannels { get; set; }
	}
}
