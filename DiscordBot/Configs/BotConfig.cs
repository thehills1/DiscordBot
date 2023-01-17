﻿namespace DiscordBot.Configs
{
    public class BotConfig : BaseConfig<BotConfig>
    {
        /// <summary>
        /// Токен.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Id тестового сервера, на который нужно зарегистрировать тестовые команды.
        /// </summary>
        public ulong TestGuildId { get; set; }
    }
}
