﻿using BossBot.Interfaces;
using Discord.WebSocket;

namespace BossBot.Commands
{
    public class ClearBossCommand(BossData bossData) : ICommand
    {
        public string[] Keys { get; } = ["c", "о"];


        public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, string[] commands)
        {
            bossData.ClearAllBossInformation(chatId);
            List<string> answer = ["Все тайминги были сброшены"];
            return Task.FromResult(answer.Select(s => s));
        }
    }
}
