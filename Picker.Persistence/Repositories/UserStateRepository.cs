using Picker.Domain.Entities.Users;
using Picker.Persistence.Data;

namespace Picker.Persistence.Repositories;

public class UserStateRepository(ApplicationContext context) : IUserStateRepository
{
    public async Task SaveUserStateAsync(UserState userState)
    {
        var existingUserState = await context.UserStates.FindAsync(userState.UserId);
        if (existingUserState == null)
        {
            context.UserStates.Add(userState);
        }
        else
        {
            context.Entry(existingUserState).CurrentValues.SetValues(userState);
        }
        await context.SaveChangesAsync();
    }

    public async Task<UserState> GetUserStateAsync(long userId)
    {
        return await context.UserStates.FindAsync(userId);
    }
}
