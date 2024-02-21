using DiscordBot.Commands;
using DiscordBot.Configs;
using DiscordBot.Server.Commands.ModalForms;
using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Server.Commands
{
	public class ServerRulesCommands : BaseServerCommands, IRulesCommands
	{
		public ServerRulesCommandsManager CommandsManager { get; }

		public ServerRulesCommands(ServerRulesCommandsManager commandsManager)
		{
			CommandsManager = commandsManager;
		}

		public async Task AddSection(InteractionContext context)
		{
			var modalFormInfo = EditSectionModalForm.Create("Добавление раздела правил");
			await context.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalFormInfo.Form);

			var response = await context.Client.GetInteractivity().WaitForModalAsync(modalFormInfo.CustomId);

			if (response.Result == null) return;
			var result = await CommandsManager.TryAddSection(response.Result.Values);

			await SendCommandExecutionResult(response.Result.Interaction, result);
		}

		public async Task EditSection(InteractionContext context, string sectionNumber)
		{
			var result = CommandsManager.TryGetSectionByNumber(sectionNumber, out var section);
			if (!result.Success)
			{
				await SendCommandExecutionResult(context, result);
				return;
			}

			var modalFormInfo = EditSectionModalForm.Create("Изменение раздела правил", section.Description, section.Number.ToString(), section.Name);
			await context.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalFormInfo.Form);

			var response = await context.Client.GetInteractivity().WaitForModalAsync(modalFormInfo.CustomId);

			if (response.Result == null) return;
			result = await CommandsManager.TryEditSection(section, response.Result.Values);

			await SendCommandExecutionResult(response.Result.Interaction, result);
		}

		public async Task RemoveSection(InteractionContext context, string section)
		{
			await context.DeferAsync(true);

			var result = await CommandsManager.TryRemoveSection(section);

			await SendCommandExecutionResult(context, result);
		}

		public async Task AddRule(InteractionContext context)
		{
			var modalFormInfo = EditRuleModalForm.Create("Добавление правила");
			await context.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalFormInfo.Form);

			var response = await context.Client.GetInteractivity().WaitForModalAsync(modalFormInfo.CustomId);

			if (response.Result == null) return;
			var result = await CommandsManager.TryAddRule(response.Result.Values);

			await SendCommandExecutionResult(response.Result.Interaction, result);
		}

		public async Task EditRule(InteractionContext context, string sectionAndRuleNumber)
		{
			var result = CommandsManager.TryGetRuleByNumber(sectionAndRuleNumber, out var rule);
			if (!result.Success)
			{
				await SendCommandExecutionResult(context, result);
				return;
			}

			var rawRuleNumber = $"{rule.SectionNumber}.{rule.Number}";
			var modalFormInfo = EditRuleModalForm.Create("Изменение правила", rawRuleNumber, rule.SubNumber.ToString(), rule.Name, rule.Punishment, string.Join('\n', rule.Notes));
			await context.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalFormInfo.Form);

			var response = await context.Client.GetInteractivity().WaitForModalAsync(modalFormInfo.CustomId);

			if (response.Result == null) return;
			result = await CommandsManager.TryEditRule(rule, response.Result.Values);

			await SendCommandExecutionResult(response.Result.Interaction, result);
		}

		public async Task RemoveRule(InteractionContext context, string rule)
		{
			await context.DeferAsync(true);

			var result = await CommandsManager.TryRemoveRule(rule);

			await SendCommandExecutionResult(context, result);
		}

		public async Task Update(InteractionContext context, bool fullUpdate = false)
		{
			await context.DeferAsync(true);

			await CommandsManager.UpdateMessages(fullUpdate);

			await SendCommandExecutionResult(context, new CommandResult(true, "Список правил успешно обновлен."));
		}

		public async Task GenerateForumText(InteractionContext context)
		{
			await context.DeferAsync(true);

			var result = CommandsManager.GenerateForumText(context);

			await SendCommandExecutionResult(context, result);
		}
	}
}
