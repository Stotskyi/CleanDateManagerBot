using Picker.Infrastructure.Entities;

namespace Picker.Infrastructure.Repository.Interfaces;

public interface IUserStateRepository
{
    public  Task SaveUserStateAsync(UserState userState);
    public  Task<UserState> GetUserStateAsync(long userId);
}