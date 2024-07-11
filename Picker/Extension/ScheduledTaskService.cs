using Microsoft.Extensions.Logging;
using Picker.Infrastructure.Repository.Interfaces;

namespace WebApplication2.Services;

public class ScheduledTaskService(ILogger<ScheduledTaskService> logger,IColiverRepository coliverRepository) 
{
    private Timer _timer;

    protected Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var now = DateTime.Now;
        var scheduledTime = DateTime.Today.AddHours(9); // 9 AM

        if (now > scheduledTime)
        {
            scheduledTime = scheduledTime.AddDays(1);
        }

        var initialDelay = scheduledTime - now;

        _timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromHours(24));
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        logger.LogInformation("Scheduled task running at: {time}", DateTimeOffset.Now);
        YourMethod();
    }

    private void YourMethod()
    {
        
        logger.LogInformation("YourMethod was called.");
    }
    
}