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
			[Option(nameof(owner), "Владелец магазина")] DiscordUser owner,
			[Option(nameof(serverName), "Название сервера")] ServerName serverName,
			[Option(nameof(name), "Название магазина")] string name,
			[Option(nameof(paidUntil), "Оплачен до")] string paidUntil,
			[Option(nameof(firstDeputy), "Первый заместитель")] DiscordUser firstDeputy = null,
			[Option(nameof(secondDeputy), "Второй заместитель")] DiscordUser secondDeputy = null,
			[Option(nameof(imageUrl), "Ссылка на изображение")] string imageUrl = null)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.Add(context, owner, serverName, name, paidUntil, firstDeputy, secondDeputy, imageUrl);
		}

		[SlashCommand("adddeputy", "Добавить заместителя.", false)]
		public async Task AddDeputy(InteractionContext context,
			[Option(nameof(name), "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option(nameof(deputy), "Заместитель")] DiscordUser deputy)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.AddDeputy(context, name, deputy);
		}

		[SlashCommand("changename", "Изменить название магазина.", false)]
		public async Task ChangeName(InteractionContext context,
			[Option(nameof(name), "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option(nameof(newName), "Новое название магазина")] string newName)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.ChangeName(context, name, newName);
		}

		[SlashCommand("delete", "Удалить магазин.", false)]
		public async Task DeleteShop(InteractionContext context,
			[Option(nameof(name), "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.DeleteShop(context, name);
		}

		[SlashCommand("extend", "Продлить оплату магазина.", false)]
		public async Task ExtendShop(InteractionContext context, 
			[Option(nameof(name), "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option(nameof(time), "Единица времени")] TimeCmd time,
			[Option(nameof(count), "Количество единиц времени")] long count)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.ExtendShop(context, name, time, count);
		}

		[SlashCommand("removedeputy", "Убрать заместителя из магазина.", false)]
		public async Task RemoveDeputy(InteractionContext context, 
			[Option(nameof(name), "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option(nameof(deputy), "Заместитель")] DiscordUser deputy)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.RemoveDeputy(context, name, deputy);
		}

		[SlashCommand("updateimage", "Обновить изображение магазина.", false)]
		public async Task UpdateImage(InteractionContext context, 
			[Option(nameof(name), "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option(nameof(imageUrl), "Ссылка на изображение")] string imageUrl)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.UpdateImage(context, name, imageUrl);
		}

		[SlashCommand("updatelist", "Обновить список магазинов.", false)]
		public async Task UpdateList(InteractionContext context,
			[Option(nameof(serverName), "Название сервера")] ServerName serverName)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.UpdateList(context, serverName);
		}

		[SlashCommand("setowner", "Изменить владельца магазина.", false)]
		public async Task SetOwner(InteractionContext context,
			[Option(nameof(name), "Название магазина")][Autocomplete(typeof(ShopsAutocompleteProvider))] string name,
			[Option(nameof(newOwner), "Новый владелец магазина")] DiscordUser newOwner)
		{
			await _serviceManager.GetServerService(context.Guild.Id)
				.ServerShopCommands.SetOwner(context, name, newOwner);
		}
	}
}
