namespace DiscordBot.Configs
{
	public class StaffInfoMessagesConfig : BaseConfig<StaffInfoMessagesConfig>
	{
		public List<MessageInfo> SentMessagesIds { get; set; }
	}

	public class MessageInfo
	{
		public ulong ChannelId { get; set; }

		public ulong MessageId { get; set; }

		public bool AllTables { get; set; }
	}
}
