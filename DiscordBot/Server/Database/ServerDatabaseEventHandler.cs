using System.Timers;
using DiscordBot.Configs;
using DiscordBot.Database.Events;
using DiscordBot.Database.Tables;
using DiscordBot.Extensions;
using DiscordBot.Server.Commands;
using Timer = System.Timers.Timer;

namespace DiscordBot.Server.Database
{
	public class ServerDatabaseEventHandler
	{
		private static readonly TimeSpan SendStaffInfoWorksheetInterval = TimeSpan.FromMinutes(1);

		private Timer _sendStaffInfoTimer;

		private readonly Bot _bot;
		private readonly ServerDatabaseManager _databaseManager;
		private readonly ServerGlobalCommandsManager _commandsManager;
		private readonly ServerConfig _serverConfig;

		public ServerDatabaseEventHandler(
			Bot bot,
			ServerDatabaseManager databaseManager,
			ServerGlobalCommandsManager commandsManager,
			ServerConfig serverConfig)
		{
			_bot = bot;
			_databaseManager = databaseManager;
			_commandsManager = commandsManager;
			_serverConfig = serverConfig;

			databaseManager.TableUpdated += HandleTableUpdated;
			databaseManager.TableAdded += HandleTableAdded;
		}

		public void Initialize()
		{
			_sendStaffInfoTimer = new Timer(SendStaffInfoWorksheetInterval);
			_sendStaffInfoTimer.Elapsed += SendStaffInfoWorksheet;
			_sendStaffInfoTimer.AutoReset = false;
			_sendStaffInfoTimer.Start();
		}

		public void HandleTableAdded(object sender, TableAddedEventArgs e)
		{
			if (e.Table is not ModeratorTable) return;

			_sendStaffInfoTimer.Reset();
		}

		public void HandleTableUpdated(object sender, TableUpdatedEventArgs e)
		{
			if (e.Table is not ModeratorTable) return;

			_sendStaffInfoTimer.Reset();
		}

		private void SendStaffInfoWorksheet(object sender, ElapsedEventArgs e)
		{
			lock (_sendStaffInfoTimer)
			{
				_commandsManager.TrySendExcelStaffWorksheetAsync(_bot.GetChannelAsync(_serverConfig.InfoChannelId).Result).Wait();
				_commandsManager.TrySendExcelStaffWorksheetAsync(_bot.GetChannelAsync(_serverConfig.HeadModChannelId).Result, true, true).Wait();
			}
		}
	}
}
