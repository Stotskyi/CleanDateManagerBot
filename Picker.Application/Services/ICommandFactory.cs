using Picker.Application.Data;

namespace Picker.Application.Services;

public interface ICommandFactory
{
    public ICommand GetCommand(string messageText);
}