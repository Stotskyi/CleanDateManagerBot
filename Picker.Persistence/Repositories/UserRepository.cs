﻿using Microsoft.EntityFrameworkCore;
using Picker.Domain.Entities.Users;
using Picker.Persistence.Data;

namespace Picker.Persistence.Repositories;

public class UserRepository(ApplicationContext dbContext) : IUserRepository
{
    public async Task<User?> GetUserAsync(string username)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task CreateUser(User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async  Task UpdateUserAsync(User user)
    {
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
    }

    public async  Task<string> GetDickTable()
    {
        var users =  await dbContext.Users
            .Select(u => new
            {
                Username = u.Username,
                DickSize = u.DickSize
            })
            .ToListAsync();
        var formattedStats = string.Join(Environment.NewLine, 
            users.Select((s, index) => $"{index + 1}. {s.Username}:  {s.DickSize}"));

        return formattedStats;
    }
}