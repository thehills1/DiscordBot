using DSharpPlus.Entities;

namespace DiscordBot.Server.Commands.ModalForms
{
	public class ModalFormInfo
	{
		public string CustomId { get; }

		public DiscordInteractionResponseBuilder Form { get; }

		public ModalFormInfo(string customId, DiscordInteractionResponseBuilder form)
		{
			CustomId = customId;
			Form = form;
		}
	}
}
