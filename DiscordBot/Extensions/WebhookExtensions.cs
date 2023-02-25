using DSharpPlus.Entities;

namespace DiscordBot.Extensions
{
	public static class WebhookExtensions
	{
		public static DiscordEmbedBuilder WithSuccessResult(this DiscordEmbedBuilder builder, string description)
		{
			return builder.WithColor(DiscordRgbColor.Green).WithTitle("Выполнено успешно!").WithDescription(description);
		}

		public static DiscordEmbedBuilder WithErrorResult(this DiscordEmbedBuilder builder, string description)
		{
			return builder.WithColor(DiscordRgbColor.Red).WithTitle("Произошла ошибка!").WithDescription(description);
		}

		public static DiscordWebhookBuilder AddEmbedWithSuccessResult(this DiscordWebhookBuilder builder, string description)
		{
			return builder.AddEmbed(new DiscordEmbedBuilder().WithSuccessResult(description));
		}

		public static DiscordWebhookBuilder AddEmbedWithErrorResult(this DiscordWebhookBuilder builder, string description)
		{
			return builder.AddEmbed(new DiscordEmbedBuilder().WithErrorResult(description));
		}

		public static DiscordInteractionResponseBuilder AddEmbedWithSuccessResult(this DiscordInteractionResponseBuilder builder, string content)
		{
			return builder.AddEmbed(new DiscordEmbedBuilder().WithSuccessResult(content));
		}

		public static DiscordInteractionResponseBuilder AddEmbedWithErrorResult(this DiscordInteractionResponseBuilder builder, string content)
		{
			return builder.AddEmbed(new DiscordEmbedBuilder().WithErrorResult(content));
		}
	}
}
