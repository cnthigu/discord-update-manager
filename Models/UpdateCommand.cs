namespace DiscordBotUpdate.Models
{
    public class UpdateCommand
    {
        public string Client { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string DeveloperName { get; set; } = string.Empty;
        public string DeployMethod { get; set; } = string.Empty;
        public int Percent { get; set; }
        public List<string> Changelog { get; set; } = new();
        public List<string> Bugs { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Client) &&
                   !string.IsNullOrWhiteSpace(Version) &&
                   !string.IsNullOrWhiteSpace(DeveloperName) &&
                   !string.IsNullOrWhiteSpace(DeployMethod);
        }

        public bool IsCanaryDeploy => DeployMethod.ToLower() == "canary";
        public bool IsValidCanaryPercent => IsCanaryDeploy && Percent >= 1 && Percent <= 100;
    }
}
