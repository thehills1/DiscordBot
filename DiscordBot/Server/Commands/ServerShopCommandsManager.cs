using DiscordBot.Configs;
using DiscordBot.Database.Enums;
using DiscordBot.Database.Tables;
using DiscordBot.Extensions;
using DiscordBot.Server.Database;
using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBot.Server.Commands
{
	public class ServerShopCommandsManager
	{
		private readonly Bot _bot;
		private readonly ServerDatabaseManager _databaseManager;
		private readonly ShopsConfig _shopsConfig;

		public ServerShopCommandsManager(Bot bot, ServerDatabaseManager databaseManager, ShopsConfig shopsConfig)
		{
			_bot = bot;
			_databaseManager = databaseManager;
			_shopsConfig = shopsConfig;
		}

		public async Task<CommandResult> TryAddShopAsync(
			ulong ownerId, 
			ServerName serverName, 
			string name, 
			string paidUntil,
			ulong firstDeputyId,
			ulong secondDeputyId,
			string imageUrl = null)
		{
			var tables = await _databaseManager.GetMultyDataDBAsync<ShopTable>(table => table.ServerName == serverName);
			if (tables.Count >= _shopsConfig.MaxShopsPerServer)
			{
				return new CommandResult(false, $"На данном сервере уже есть **{_shopsConfig.MaxShopsPerServer}** магазинов.");
			}

			if (IsShopExists(name, out var message, out _)) return new CommandResult(false, message);
			if (!DateTime.TryParse(paidUntil, out var paidUntilTime))
			{
				return new CommandResult(false, "Указан неверный формат даты - указывайте **ДД.ММ.ГГ.**");
			}

			var shopTable = new ShopTable()
			{
				OwnerId = ownerId,
				ServerName = serverName,
				Name = name,
				PaidUntil = paidUntilTime,
				FirstDeputyId = firstDeputyId,
				SecondDeputyId = secondDeputyId,
				ImageUrl = imageUrl
			};

			_databaseManager.AddTableDB(shopTable);

			await TryUpdateListAsync(shopTable.ServerName);

			return new CommandResult(true, $"Магазин **{name}** успешно добавлен с список магазинов сервера **{serverName}.**");
		}

		public async Task<CommandResult> TryAddDeputyAsync(string name, ulong deputyId)
		{
			if (!IsShopExists(name, out var message, out var shopTable)) return new CommandResult(false, message);
			if (shopTable.OwnerId == deputyId) return new CommandResult(false, "Владельца магазина нельзя сделать его заместителем.");
			if (shopTable.FirstDeputyId != 0 && shopTable.SecondDeputyId != 0)
			{
				return new CommandResult(false, "В магазине уже есть максимальное количество заместителей.");
			}

			if (shopTable.FirstDeputyId == 0)
			{
				shopTable.FirstDeputyId = deputyId;
			}
			else
			{
				shopTable.SecondDeputyId = deputyId;
			}

			_databaseManager.AddOrUpdateTableDB(shopTable);

			await TryUpdateListAsync(shopTable.ServerName);

			return new CommandResult(true, $"Заместитель {deputyId.GetMention(MentionType.Username)} успешно добавлен в магазин **{name}.**");
		}

		public async Task<CommandResult> TryChangeNameAsync(string name, string newName)
		{
			if (!IsShopExists(name, out var message, out var shopTable)) return new CommandResult(false, message);

			_databaseManager.RemoveTableAsync(shopTable);

			shopTable.Name = newName;
			_databaseManager.AddTableDB(shopTable);

			await TryUpdateListAsync(shopTable.ServerName);

			return new CommandResult(true, $"Название магазина успешно обновлено с **{name}** на **{newName}.**");
		}

		public async Task<CommandResult> TryDeleteShopAsync(string name)
		{
			if (!IsShopExists(name, out var message, out var shopTable)) return new CommandResult(false, message);

			await _databaseManager.RemoveTableAsync(shopTable);

			await TryUpdateListAsync(shopTable.ServerName);

			return new CommandResult(true, $"Магазин **{name}** успешно удалён.");
		}

		public async Task<CommandResult> TryExtendShopAsync(string name, TimeCmd time, int count)
		{
			if (count <= 0) return new CommandResult(false, "Количество времени не может быть меньше или равно нулю.");
			if (!IsShopExists(name, out var message, out var shopTable)) return new CommandResult(false, message);

			var actualPaidTime = shopTable.PaidUntil;
			actualPaidTime = time switch
			{
				TimeCmd.Day => actualPaidTime.AddDays(count),
				TimeCmd.Week => actualPaidTime.AddDays(count * 7),
				TimeCmd.Month => actualPaidTime.AddMonths(count),
				TimeCmd.Year => actualPaidTime.AddYears(count)
			};

			shopTable.PaidUntil = actualPaidTime;

			_databaseManager.AddOrUpdateTableDB(shopTable);

			await TryUpdateListAsync(shopTable.ServerName);

			return new CommandResult(true, $"Магазин **{name}** успешно продлен на **{count} {time}.**");
		}

		public async Task<CommandResult> TryRemoveDeputyAsync(string name, ulong deputyId)
		{
			if (!IsShopExists(name, out var message, out var shopTable)) return new CommandResult(false, message);
			if (!(shopTable.FirstDeputyId == deputyId || shopTable.SecondDeputyId == deputyId))
			{
				return new CommandResult(false, $"Пользователя {deputyId.GetMention(MentionType.Username)} нет в списке заместителей данного магазина.");
			}

			if (shopTable.FirstDeputyId == deputyId)
			{
				shopTable.FirstDeputyId = 0;
			}
			else
			{
				shopTable.SecondDeputyId = 0;
			}

			_databaseManager.AddOrUpdateTableDB(shopTable);

			await TryUpdateListAsync(shopTable.ServerName);

			return new CommandResult(true, $"Пользователь {deputyId.GetMention(MentionType.Username)} удален из списка заместителей магазина **{name}.**");
		}

		public async Task<CommandResult> TryUpdateImageAsync(string name, string imageUrl)
		{
			if (!IsShopExists(name, out var message, out var shopTable)) return new CommandResult(false, message);

			shopTable.ImageUrl = imageUrl;

			_databaseManager.AddOrUpdateTableDB(shopTable);

			await TryUpdateListAsync(shopTable.ServerName);

			return new CommandResult(true, "Ссылка на изображение успешно обновлена.");
		}

		private readonly object _updateSync = new object();

		public async Task<CommandResult> TryUpdateListAsync(ServerName serverName)
		{
			var embeds = new List<DiscordEmbed>();
			var tables = await _databaseManager.GetMultyDataDBAsync<ShopTable>(table => table.ServerName == serverName);
			foreach (var table in tables)
			{
				embeds.Add(GenerateShopInfo(table));
			}

			var content = $"**Свободно слотов для открытия магазинов: {_shopsConfig.MaxShopsPerServer - tables.Count}\n\nАктуальный список магазинов:**";
			if (tables.Count == 0) content += "\n\nМагазины отсутствуют.";

			lock (_updateSync)
			{
				var messageTable = _databaseManager.GetTableDB<ShopListMessageTable>(table => table.ServerName == serverName).Result;
				if (messageTable == null || !_bot.MessageExistsAsync(_shopsConfig.InfoChannels[serverName], messageTable?.MessageId ?? 0).Result)
				{
					var sentMessage = _bot.SendMessageAsync(_shopsConfig.InfoChannels[serverName], content, embeds).Result;

					messageTable = new ShopListMessageTable()
					{
						ServerName = serverName,
						MessageId = sentMessage.Id
					};

					_databaseManager.AddOrUpdateTableDB(messageTable);
				}
				else
				{
					if (embeds.Count == 0)
					{
						_bot.DeleteMessageAsync(_shopsConfig.InfoChannels[serverName], messageTable.MessageId).Wait();
						return TryUpdateListAsync(serverName).Result;
					}
					else
					{
						_bot.EditMessageAsync(_shopsConfig.InfoChannels[serverName], messageTable.MessageId, content, embeds).Wait();
					}				
				}
			}	

			return new CommandResult(true, "Успешно отправлено.");
		}

		public async Task<CommandResult> TrySetOwnerAsync(string name, ulong newOwnerId)
		{
			if (!IsShopExists(name, out var message, out var shopTable)) return new CommandResult(false, message);
			if (shopTable.OwnerId == newOwnerId) return new CommandResult(false, "Данный пользователь итак является владельцем данного магазина.");

			await _databaseManager.RemoveTableAsync(shopTable);
			
			shopTable.OwnerId = newOwnerId;

			if (shopTable.FirstDeputyId == newOwnerId) shopTable.FirstDeputyId = 0;
			if (shopTable.SecondDeputyId == newOwnerId) shopTable.SecondDeputyId = 0;

			_databaseManager.AddOrUpdateTableDB(shopTable);

			await TryUpdateListAsync(shopTable.ServerName);

			return new CommandResult(true, $"Владелец магазина **{name}** успешно обновлен на {newOwnerId.GetMention(MentionType.Username)}.");
		}

		private DiscordEmbedBuilder GenerateShopInfo(ShopTable shopTable)
		{
			var firstDeputy = shopTable.FirstDeputyId == 0 ? string.Empty : shopTable.FirstDeputyId.GetMention(MentionType.Username);
			var secondDeputy = shopTable.SecondDeputyId == 0 ? string.Empty : shopTable.SecondDeputyId.GetMention(MentionType.Username);
			var deputies = shopTable.DeputiesExists ? $"{firstDeputy}{secondDeputy}" : "отсутствуют";
			var content = $"**Владелец:**\n" +
				$"{shopTable.OwnerId.GetMention(MentionType.Username)}\n\n" +
				$"**Заместители:**\n" +
				$"{deputies}\n\n" +
				$"**Оплачен до:**\n" +
				$"{shopTable.PaidUntil.ToShortDateString()}";

			return new DiscordEmbedBuilder().WithTitle(shopTable.Name).WithDescription(content).WithThumbnail(shopTable.ImageUrl).WithColor(DiscordRgbColor.White);
		}

		private bool IsShopExists(string name, out string message, out ShopTable table)
		{
			message = "";

			table = _databaseManager.GetTableDB<ShopTable>(table => table.Name == name).Result;

			if (table != null)
			{
				message = "Магазин с таким именем уже существует.";
				return true;
			}

			message = "Магазина с таким именем не существует.";
			return false;
		}
	}
}
