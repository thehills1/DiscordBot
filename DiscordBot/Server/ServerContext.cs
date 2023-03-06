namespace DiscordBot.Server
{
	public class ServerContext
	{
		private const string ServerDataPath = "data";

		public ulong ServerId { get; private set; }

		public string RootServerPath => Path.Combine(BotEnvironment.ServersDirectoryPath, ServerId.ToString());
		public string ModeratorsWorksheetsPath => Path.Combine(RootServerPath, ServerDataPath, "ModeratorsWorksheets");
		public string SalaryWorksheetsPath => Path.Combine(RootServerPath, ServerDataPath, "SalaryWorksheets");

		private bool _isInitialized = false;
		
		public void Setup(ulong serverId)
		{
			if (_isInitialized) throw new Exception("ServerContext is initialized, can't change server id.");

			ServerId = serverId;
			_isInitialized = true;
		}
	}
}
