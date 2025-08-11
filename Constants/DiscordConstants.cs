namespace DiscordBotUpdate.Constants
{
    public static class DiscordConstants
    {
        // IDs dos Canais (ALTERE ESTES VALORES PARA SEUS CANAIS)
        public static class Channels
        {
            public const ulong Cliente1 = 1395299946793668609;
            public const ulong Cliente2 = 1395299958999089304;
        }

        // ID do Servidor (ALTERE ESTE VALOR PARA SEU SERVIDOR)
        public const ulong GuildId = 970160731788030002;

        // Métodos de Deploy disponíveis
        public static class DeployMethods
        {
            public const string Canary = "canary";
            public const string Release = "release";
            public const string Beta = "beta";
        }

        // Cores dos embeds
        public static class Colors
        {
            public const uint Orange = 0xFFA500;
            public const uint Green = 0x00FF00;
            public const uint Red = 0xFF0000;
        }
    }
}
