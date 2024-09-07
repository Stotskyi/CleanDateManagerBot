using Picker.Application.Data;

namespace Picker.Application.Services;

public interface ICommandFactory
{
    public ICommand? GetCommand(string message);

    public IScheduleCommand GetScheduleCommand(string message);

    public IDickCommand GetDickCommand(string message);
}