namespace DiscordBot.Configs
{
	public class StaffInfoMessagesConfig : BaseConfig<StaffInfoMessagesConfig>
	{
		public Dictionary<ulong, ulong> SentMessageIds { get; set; }
	}
}
