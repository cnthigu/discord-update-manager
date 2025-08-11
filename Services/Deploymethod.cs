using Discord;
using Discord.Interactions;
using DiscordBotUpdate.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotUpdate.Services
{
    public class DeployMethodAutocompleteHandler : AutocompleteHandler
    {
        private static readonly List<string> options = new() 
        { 
            DiscordConstants.DeployMethods.Canary, 
            DiscordConstants.DeployMethods.Release, 
            DiscordConstants.DeployMethods.Beta 
        };

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction interaction,
            IParameterInfo parameter,
            IServiceProvider services)
        {
            var query = interaction.Data.Current.Value?.ToString()?.ToLower() ?? "";

            var filtered = options
                .Where(x => x.StartsWith(query))
                .Select(x => new AutocompleteResult(x, x));

            return Task.FromResult(AutocompletionResult.FromSuccess(filtered));
        }
    }
}
