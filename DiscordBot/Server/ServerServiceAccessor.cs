namespace DiscordBot.Server
{
	public class ServerServiceAccessor : IServerServiceAccessor
	{
		private ServerService _service;

		public ServerService Service => _service;

		public void SetService(ServerService service)
		{
			_service = service;
		}
	}
}
