using System.Collections.Concurrent;
using DiscordBot.Server;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
	public class ServiceManager
    {
		private readonly ConcurrentDictionary<ulong, ServerService> _serverServices = new();

		private readonly IServiceProvider _serviceProvider;
		private readonly Bot _bot;

		public ServiceManager(IServiceProvider serviceProvider, Bot bot) 
        {
			_serviceProvider = serviceProvider;
			_bot = bot;
		}

		public void Initialize()
		{
			_bot.Initialize();
		}

		public ServerService GetServerService(ulong serverId)
		{
			return _serverServices.GetOrAdd(serverId, _serverId =>
			{
				var scope = _serviceProvider.CreateScope();
				var accessor = (ServerServiceAccessor) scope.ServiceProvider.GetService<IServerServiceAccessor>();

				var serverContext = scope.ServiceProvider.GetService<ServerContext>();
				serverContext.Setup(serverId);

				var serverService = scope.ServiceProvider.GetService<ServerService>();
                accessor.SetService(serverService);
				serverService.Initialize();

				return serverService;
			});
		}
    }
}
