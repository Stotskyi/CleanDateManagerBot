namespace Picker.Domain.Entities.Users;

public interface IUserStateRepository
{
    public  Task SaveUserStateAsync(UserState userState);
    public  Task<UserState> GetUserStateAsync(long userId);
}