using DiscordBot.Commands.AutocompleteProviders;
using DiscordBot.Database.Enums;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands
{
	[SlashModuleLifespan(SlashModuleLifespan.Singleton)]
	[SlashCommandGroup("shop", "Команды магазинов.", false)]
	public class ShopCommands : ApplicationCommandModule, IShopCommands
	{
		private readonly ServiceManager _serviceManager;

		public ShopCommands(ServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[SlashCommand("add", "Добавить магазин.", false)]
		public async Task Add(InteractionContext context,
			[Option("Owner", "Владелец магазина")] DiscordUser owner,
			[Option("ServerName", "Название сервера")] ServerName serverName,
			[Option("Name", "Название магазина")] string name,
			[Option("PaidUntil", "Оплачен до")] string paidUntil,
			[Option("FirstDeputy", "Первый заместитель")] DiscordUser firstDeputy = null,
			[Option("SecondDeputy", "Второй заместитель")] DiscordUser secondDeputy = null,
			[Option("ImageUrl", "Ссылка на изображение")] string imageUrl = null)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.Add(context, owner, serverName, name, paidUntil, firstDeputy, secondDeputy, imageUrl);
		}

		[SlashCommand("adddeputy", "Добавить заместителя.", false)]
		public async Task AddDeputy(InteractionContext context,
			[Option("Name", "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option("Deputy", "Заместитель")] DiscordUser deputy)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.AddDeputy(context, name, deputy);
		}

		[SlashCommand("changename", "Изменить название магазина.", false)]
		public async Task ChangeName(InteractionContext context,
			[Option("Name", "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option("NewName", "Новое название магазина")] string newName)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.ChangeName(context, name, newName);
		}

		[SlashCommand("delete", "Удалить магазин.", false)]
		public async Task Delete(InteractionContext context,
			[Option("Name", "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.Delete(context, name);
		}

		[SlashCommand("extend", "Продлить оплату магазина.", false)]
		public async Task Extend(InteractionContext context, 
			[Option("Name", "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option("Time", "Единица времени")] TimeCmd time,
			[Option("Count", "Количество единиц времени")] long count)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.Extend(context, name, time, count);
		}

		[SlashCommand("removedeputy", "Убрать заместителя из магазина.", false)]
		public async Task RemoveDeputy(InteractionContext context, 
			[Option("Name", "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option("Deputy", "Заместитель")] DiscordUser deputy)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.RemoveDeputy(context, name, deputy);
		}

		[SlashCommand("updateimage", "Обновить изображение магазина.", false)]
		public async Task UpdateImage(InteractionContext context, 
			[Option("Name", "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option("ImageUrl", "Ссылка на изображение")] string imageUrl)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.UpdateImage(context, name, imageUrl);
		}

		[SlashCommand("updatelist", "Обновить список магазинов.", false)]
		public async Task UpdateList(InteractionContext context,
			[Option("ServerName", "Название сервера")] ServerName serverName)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.UpdateList(context, serverName);
		}

		[SlashCommand("setowner", "Изменить владельца магазина.", false)]
		public async Task SetOwner(InteractionContext context,
			[Option("Name", "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option("NewOwner", "Новый владелец магазина")] DiscordUser newOwner)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.SetOwner(context, name, newOwner);
		}
	}
}
