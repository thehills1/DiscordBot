namespace DiscordBot.Configs
{
	public class ServerConfig : BaseConfig<ServerConfig>
	{
		/// <summary>
		/// Id канала с информацией.
		/// </summary>
		public ulong InfoChannelId { get; set; }

		/// <summary>
		/// Id канала со статистикой для бота.
		/// </summary>
		public ulong BotStatsChannelId { get; set; }

		/// <summary>
		/// Id канала с общей статистикой.
		/// </summary>
		public ulong StatsChannelId { get; set; }

		/// <summary>
		/// Id канала главной модерации.
		/// </summary>
		public ulong HeadModChannelId { get; set; }
	}
}
