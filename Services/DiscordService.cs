using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBotUpdate.Configuration;
using DiscordBotUpdate.Controllers;
using Microsoft.Extensions.Options;

namespace DiscordBotUpdate.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly DiscordBotConfig _config;
        private readonly IServiceProvider _provider;

        public DiscordService(IOptions<DiscordBotConfig> config, IServiceProvider provider)
        {
            _config = config.Value;
            _provider = provider;
            
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds
            });

            _interactionService = new InteractionService(_client.Rest);

            _client.Ready += ReadyAsync;
            _client.InteractionCreated += HandleInteractionAsync;
        }

        public async Task StartAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task ReadyAsync()
        {
            await _interactionService.AddModulesAsync(typeof(UpdateController).Assembly, _provider);
            await _interactionService.RegisterCommandsToGuildAsync(_config.GuildId, true);
        }

        private async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _provider);
        }
    }
}
