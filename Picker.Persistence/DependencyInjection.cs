using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Picker.Application.Services;
using Picker.Domain.Entities.Users;
using Picker.Persistence.Data;
using Picker.Persistence.Repositories;

namespace Picker.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        
        services.AddScoped<IUserStateRepository,UserStateRepository>();
        services.AddScoped<IColiverRepository,ColiverRepository>();
        services.AddScoped<ICommandFactory, CommandFactory.CommandFactory>();
        
        
        services.AddHangfireServer(x => x.SchedulePollingInterval = TimeSpan.FromSeconds(1));

        services.AddHangfire(x =>
                x.UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(configuration.GetConnectionString(("hangfire"))))
            ;
        

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<UserService>();
        
        return services;
    }
}