using Discord;
using Discord.Interactions;
using DiscordBotUpdate.Models;
using DiscordBotUpdate.Services;
using DiscordBotUpdate.Constants;
using DiscordBotUpdate.AutocompleteHandlers;
using Microsoft.Extensions.Logging;

namespace DiscordBotUpdate.Controllers
{
    [Group("submitupdate", "Submeter atualização")]
    public class UpdateController : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly VersionStorageService _versionStorage;
        private readonly ILogger<UpdateController> _logger;

        public UpdateController(VersionStorageService versionStorage, ILogger<UpdateController> logger)
        {
            _versionStorage = versionStorage;
            _logger = logger;
        }

        [SlashCommand("update", "Submete uma atualização")]
        public async Task SubmitUpdate(
            [Autocomplete(typeof(ClientAutocompleteHandler))][Summary("client", "Nome do cliente")] string client,
            [Summary("version", "Versão da atualização")] string version,
            [Summary("dev", "Seu nome")] string nomedev,
            [Autocomplete(typeof(DeployMethodAutocompleteHandler))]
            [Summary("deploy", "Método de Deploy")] string deploymethod,
            [Summary("percent", "Percentual (apenas para canary). Caso deploy diferente, ignore este valor.")] int percent = 0,
            [Summary("changelog1", "Alteração Linha 1")] string changelog1 = null,
            [Summary("changelog2", "Alteração Linha 2")] string changelog2 = null,
            [Summary("changelog3", "Alteração Linha 3")] string changelog3 = null,
            [Summary("changelog4", "Alteração Linha 4")] string changelog4 = null,
            [Summary("changelog5", "Alteração Linha 5")] string changelog5 = null,
            [Summary("bugs1", "Possíveis Bugs - Linha 1")] string bugs1 = null,
            [Summary("bugs2", "Possíveis Bugs - Linha 2")] string bugs2 = null,
            [Summary("bugs3", "Possíveis Bugs - Linha 3")] string bugs3 = null)
        {
            try
            {
                // Validar entrada
                var validationResult = ValidateInput(client, version, deploymethod, percent);
                if (!validationResult.IsValid)
                {
                    await RespondAsync(validationResult.ErrorMessage, ephemeral: true);
                    return;
                }

                // Criar comando de atualização
                var updateCommand = CreateUpdateCommand(client, version, nomedev, deploymethod, percent, 
                    changelog1, changelog2, changelog3, changelog4, changelog5, 
                    bugs1, bugs2, bugs3);

                // Obter ID do canal
                var channelId = GetChannelId(client);
                if (channelId == 0)
                {
                    await RespondAsync("Cliente não encontrado.", ephemeral: true);
                    return;
                }

                // Enviar atualização
                await SendUpdateToChannel(updateCommand, channelId);
                
                // Atualizar versão no storage
                _versionStorage.UpdateVersion(client, version);
                
                await RespondAsync("Atualização enviada com sucesso!", ephemeral: true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao processar comando de atualização para cliente {Client}, versão {Version}.", client, version);
                await RespondAsync("Erro ao processar atualização. Tente novamente.", ephemeral: true);
            }
        }

        private (bool IsValid, string ErrorMessage) ValidateInput(string client, string version, string deploymethod, int percent)
        {
            // Validar formato da versão
            if (!System.Text.RegularExpressions.Regex.IsMatch(version, @"^\d+\.\d+\.\d+$"))
            {
                return (false, "Formato de versão inválido. Use o formato x.y.z (ex.: 1.2.3).");
            }

            // Validar percentual para canary
            if (deploymethod.ToLower() == DiscordConstants.DeployMethods.Canary)
            {
                if (percent < 1 || percent > 100)
                {
                    return (false, "Para deploy canary, informe um percentual válido entre 1 e 100.");
                }
            }

            return (true, string.Empty);
        }

        private UpdateCommand CreateUpdateCommand(string client, string version, string nomedev, string deploymethod, int percent,
            string changelog1, string changelog2, string changelog3, string changelog4, string changelog5,
            string bugs1, string bugs2, string bugs3)
        {
            var changelogs = new[] { changelog1, changelog2, changelog3, changelog4, changelog5 }
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .ToList();

            var bugs = new[] { bugs1, bugs2, bugs3 }
                .Where(b => !string.IsNullOrWhiteSpace(b))
                .Select(b => b.Trim())
                .ToList();

            return new UpdateCommand
            {
                Client = client,
                Version = version,
                DeveloperName = nomedev,
                DeployMethod = deploymethod,
                Percent = percent,
                Changelog = changelogs,
                Bugs = bugs
            };
        }

        private ulong GetChannelId(string client)
        {
            return client switch
            {
                "Cliente1" => DiscordConstants.Channels.Cliente1,
                "Cliente2" => DiscordConstants.Channels.Cliente2,
                _ => 0
            };
        }

        private async Task SendUpdateToChannel(UpdateCommand updateCommand, ulong channelId)
        {
            var channel = Context.Client.GetChannel(channelId) as IMessageChannel;
            if (channel == null)
            {
                throw new InvalidOperationException("Canal não encontrado ou inválido.");
            }

            var embed = CreateUpdateEmbed(updateCommand);
            await channel.SendMessageAsync(embed: embed);
        }

        private Embed CreateUpdateEmbed(UpdateCommand updateCommand)
        {
            var formattedChangelog = string.Join("\n", updateCommand.Changelog.Select(c => $"- {c}"));
            var formattedBugs = updateCommand.Bugs.Any() ? string.Join("\n", updateCommand.Bugs.Select(b => $"- {b}")) : null;

            string deployText = updateCommand.IsCanaryDeploy && updateCommand.Percent > 0 
                ? $"{updateCommand.DeployMethod} {updateCommand.Percent}%" 
                : updateCommand.DeployMethod;

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"**Versão:** `{updateCommand.Version}`")
                .WithDescription(
                    $"**Deploy:** {deployText}\n\n" +
                    $"**Patch notes:**\n" +
                    $"{formattedChangelog}")
                .WithColor(new Color(DiscordConstants.Colors.Orange))
                .WithFooter(footer => footer.Text = $"Criado por {updateCommand.DeveloperName}")
                .WithCurrentTimestamp();

            if (!string.IsNullOrWhiteSpace(formattedBugs))
            {
                embedBuilder.AddField("Possíveis Bugs:", formattedBugs);
            }

            return embedBuilder.Build();
        }
    }
}
