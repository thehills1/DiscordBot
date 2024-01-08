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
		private readonly MessageManager _messageManager;
		private readonly ServerDatabaseManager _databaseManager;
		private readonly ShopsConfig _shopsConfig;

		public ServerShopCommandsManager(MessageManager messageManager, ServerDatabaseManager databaseManager, ShopsConfig shopsConfig)
		{
			_messageManager = messageManager;
			_databaseManager = databaseManager;
			_shopsConfig = shopsConfig;
		}

		public bool TryAddShop(
			ulong ownerId, 
			ServerName serverName, 
			string name, 
			string paidUntil,
			ulong firstDeputyId,
			ulong secondDeputyId,
			out string message, 
			string imageUrl = null)
		{
			message = "";

			var tables = _databaseManager.GetMultyDataDB<ShopTable>(table => table.ServerName == serverName).Result;
			if (tables.Count >= _shopsConfig.MaxShopsPerServer)
			{
				message = $"На данном сервере уже есть **{_shopsConfig.MaxShopsPerServer}** магазинов.";
				return false;
			}

			if (IsShopExists(name, out message, out _)) return false;
			if (!DateTime.TryParse(paidUntil, out var paidUntilTime))
			{
				message = "Указан неверный формат даты - указывайте **ДД.ММ.ГГ.**";
				return false;
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

			TryUpdateList(shopTable.ServerName, out _);

			message = $"Магазин **{name}** успешно добавлен с список магазинов сервера **{serverName}.**";
			return true;
		}

		public bool TryAddDeputy(string name, ulong deputyId, out string message)
		{
			message = "";

			if (!IsShopExists(name, out message, out var shopTable)) return false;

			if (shopTable.OwnerId == deputyId)
			{
				message = "Владельца магазина нельзя сделать его заместителем.";
				return false;
			}

			if (shopTable.FirstDeputyId != 0 && shopTable.SecondDeputyId != 0)
			{
				message = "В магазине уже есть максимальное количество заместителей.";
				return false;
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

			TryUpdateList(shopTable.ServerName, out _);

			message = $"Заместитель {deputyId.GetMention(MentionType.Username)} успешно добавлен в магазин **{name}.**";
			return true;
		}

		public bool TryChangeName(string name, string newName, out string message)
		{
			message = "";

			if (!IsShopExists(name, out message, out var shopTable)) return false;

			_databaseManager.RemoveTable(shopTable);

			shopTable.Name = newName;
			_databaseManager.AddTableDB(shopTable);

			TryUpdateList(shopTable.ServerName, out _);

			message = $"Название магазина успешно обновлено с **{name}** на **{newName}.**";
			return true;
		}

		public bool TryDelete(string name, out string message)
		{
			message = "";

			if (!IsShopExists(name, out message, out var shopTable)) return false;

			_databaseManager.RemoveTable(shopTable);

			TryUpdateList(shopTable.ServerName, out _);

			message = $"Магазин **{name}** успешно удалён.";
			return true;
		}

		public bool TryExtend(string name, TimeCmd time, int count, out string message)
		{
			message = "";

			if (count <= 0)
			{
				message = "Количество времени не может быть меньше или равно нулю.";
				return false;
			}

			if (!IsShopExists(name, out message, out var shopTable)) return false;

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

			TryUpdateList(shopTable.ServerName, out _);

			message = $"Магазин **{name}** успешно продлен на **{count} {time}.**";
			return true;
		}

		public bool TryRemoveDeputy(string name, ulong deputyId, out string message)
		{
			message = "";

			if (!IsShopExists(name, out message, out var shopTable)) return false;
			if (!(shopTable.FirstDeputyId == deputyId || shopTable.SecondDeputyId == deputyId))
			{
				message = $"Пользователя {deputyId.GetMention(MentionType.Username)} нет в списке заместителей данного магазина.";
				return false;
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

			TryUpdateList(shopTable.ServerName, out _);

			message = $"Пользователь {deputyId.GetMention(MentionType.Username)} удален из списка заместителей магазина **{name}.**";
			return true;
		}

		public bool TryUpdateImage(string name, string imageUrl, out string message)
		{
			message = "";

			if (!IsShopExists(name, out message, out var shopTable)) return false;

			shopTable.ImageUrl = imageUrl;

			_databaseManager.AddOrUpdateTableDB(shopTable);

			TryUpdateList(shopTable.ServerName, out _);

			message = "Ссылка на изображение успешно обновлена.";
			return true;
		}

		private readonly object _updateSync = new object();

		public bool TryUpdateList(ServerName serverName, out string message)
		{
			message = "";

			var content = "**Актуальный список магазинов:**";
			var embeds = new List<DiscordEmbed>();
			var tables = _databaseManager.GetMultyDataDB<ShopTable>(table => table.ServerName == serverName).Result;
			foreach (var table in tables)
			{
				embeds.Add(GenerateShopInfo(table));
			}

			lock (_updateSync)
			{
				var messageTable = _databaseManager.GetTableDB<ShopListMessageTable>(table => table.ServerName == serverName).Result;
				if (messageTable == null)
				{
					var sentMessage = _messageManager.SendMessageAsync(_shopsConfig.InfoChannels[serverName], content, embeds).Result;

					messageTable = new ShopListMessageTable()
					{
						ServerName = serverName,
						MessageId = sentMessage.Id
					};

					_databaseManager.AddOrUpdateTableDB(messageTable);
				}
				else
				{
					_messageManager.EditMessageAsync(_shopsConfig.InfoChannels[serverName], messageTable.MessageId, content, embeds);
				}
			}	

			message = "Успешно отправлено.";
			return true;
		}

		public bool TrySetOwner(string name, ulong newOwnerId, out string message)
		{
			message = "";

			if (!IsShopExists(name, out message, out var shopTable)) return false;

			if (shopTable.OwnerId == newOwnerId)
			{
				message = "Данный пользователь итак является владельцем данного магазина.";
				return false;
			}

			_databaseManager.RemoveTable(shopTable);
			
			shopTable.OwnerId = newOwnerId;

			if (shopTable.FirstDeputyId == newOwnerId) shopTable.FirstDeputyId = 0;
			if (shopTable.SecondDeputyId == newOwnerId) shopTable.SecondDeputyId = 0;

			_databaseManager.AddOrUpdateTableDB(shopTable);

			TryUpdateList(shopTable.ServerName, out _);

			message = $"Владелец магазина **{name}** успешно обновлен на {newOwnerId.GetMention(MentionType.Username)}.";
			return true;
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
