﻿using Telegram.Bot.Types;

namespace Picker.Application.Data;

public interface IDickCommand
{
    public Task<string> Execute(string username,string firstname,string lastname);
}