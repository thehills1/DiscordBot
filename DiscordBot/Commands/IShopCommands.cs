using DiscordBot.Commands.AutocompleteProviders;
using DiscordBot.Database.Enums;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands
{
	public interface IShopCommands
	{
		Task Add(
			InteractionContext context, 
			DiscordUser owner,  
			ServerName serverName, 
			string name, 
			string paidUntil, 
			DiscordUser firstDeputy = null,
			DiscordUser secondDeputy = null, 
			string imageUrl = null);

		Task Delete(InteractionContext context, string name);

		Task Extend(InteractionContext context, string name, TimeCmd time, long count);

		Task ChangeName(InteractionContext context, string name, string newName);

		Task AddDeputy(InteractionContext context, string name, DiscordUser deputy);

		Task RemoveDeputy(InteractionContext context, string name, DiscordUser deputy);

		Task UpdateImage(InteractionContext context, string name, string imageUrl);

		Task UpdateList(InteractionContext context, ServerName serverName);

		Task SetOwner(InteractionContext context, string name, DiscordUser newOwner);
	}
}
