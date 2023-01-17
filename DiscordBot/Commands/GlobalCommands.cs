using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands
{
	[SlashModuleLifespan(SlashModuleLifespan.Singleton)]
	public class GlobalCommands : ApplicationCommandModule, IGlobalCommands
	{
		private readonly ServiceManager _serviceManager;

		public GlobalCommands(ServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[SlashCommand("abcde", "desc")]
		public async Task Test(InteractionContext context, [Option("text", "text")] string text)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.Test(context, text);
		}
	}
}
