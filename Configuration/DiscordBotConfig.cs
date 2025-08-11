namespace DiscordBotUpdate.Configuration
{
    public class DiscordBotConfig
    {
        public const string SectionName = "DiscordBot";
        
        public string Token { get; set; } = string.Empty;
        public ulong GuildId { get; set; }
        public Dictionary<string, ulong> Channels { get; set; } = new();
        public List<string> DeployMethods { get; set; } = new();
        public string VersionsFilePath { get; set; } = "versions.json";
    }
}
