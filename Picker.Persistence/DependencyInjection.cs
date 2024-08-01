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
        
        return services;
    }
}