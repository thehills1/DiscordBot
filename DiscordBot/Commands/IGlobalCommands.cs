using DiscordBot.Database.Enums;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands
{
	public interface IGlobalCommands
	{
		Task AddModerator(
			InteractionContext context, 
			DiscordUser user,
			PermissionLevel permissionLevel,
			long reprimands, 
			string nickname, 
			string sid,
			ServerName serverName, 
			string bankNumber, 
			string forum);

		Task SetModeratorPermissionLevel(
			InteractionContext context, 
			DiscordUser user, 
			PermissionLevel permissionLevel, 
			string dismissionReason = null, 
			string reinstatement = null);

		Task WarnModerator(InteractionContext context, DiscordUser user);

		Task UnWarnModerator(InteractionContext context, DiscordUser user);

		Task EditModeratorInfo(InteractionContext context, DiscordUser user, string property, string value);

		Task SendStaffInfo(InteractionContext context, DiscordChannel channel, bool allTables);

		Task SendExcelSalaryWorksheet(InteractionContext context, long weeks);

		Task GetModeratorSalaryInfo(InteractionContext context, DiscordUser user = null);

		Task SetNorm(InteractionContext context, long count);
	}
}
