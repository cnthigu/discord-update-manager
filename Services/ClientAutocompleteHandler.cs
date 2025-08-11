using Discord;
using Discord.Interactions;
using DiscordBotUpdate.Constants;
using DiscordBotUpdate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotUpdate.AutocompleteHandlers
{
    public class ClientAutocompleteHandler : AutocompleteHandler
    {
        private readonly VersionStorageService _versionStorage;
        private static readonly List<string> ChannelKeys = new()
        {
            "Cliente1",
            "Cliente2"
        };

        public ClientAutocompleteHandler(VersionStorageService versionStorage)
        {
            _versionStorage = versionStorage;
        }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction interaction,
            IParameterInfo parameter,
            IServiceProvider services)
        {
            var query = interaction.Data.Current.Value?.ToString()?.ToLower() ?? "";
            var suggestions = ChannelKeys
                .Where(key => key.StartsWith(query))
                .Select(key =>
                {
                    var version = _versionStorage.GetVersion(key);
                    var display = version != null ? $"{key} (Última: {version})" : key;
                    return new AutocompleteResult(display, key);
                });

            return Task.FromResult(AutocompletionResult.FromSuccess(suggestions));
        }
    }
}