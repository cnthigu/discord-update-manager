using DiscordBotUpdate.Services;
using DiscordBotUpdate.Configuration;
using DiscordBotUpdate.AutocompleteHandlers;

var builder = WebApplication.CreateBuilder(args);

// Configuração do bot Discord
builder.Services.Configure<DiscordBotConfig>(
    builder.Configuration.GetSection(DiscordBotConfig.SectionName));

// Serviços
builder.Services.AddSingleton<DiscordService>();
builder.Services.AddSingleton<VersionStorageService>();
builder.Services.AddSingleton<ClientAutocompleteHandler>();
builder.Services.AddSingleton<DeployMethodAutocompleteHandler>();

var app = builder.Build();

// Iniciar o bot Discord
var discord = app.Services.GetRequiredService<DiscordService>();
_ = discord.StartAsync();

app.Run();
