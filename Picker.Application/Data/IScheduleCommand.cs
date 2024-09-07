namespace Picker.Application.Data;

public interface IScheduleCommand
{
    Task<string> Execute();
}