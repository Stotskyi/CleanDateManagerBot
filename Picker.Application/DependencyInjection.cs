using FluentValidation;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;
using Picker.Application.Abstractions.Behaviours;

namespace Picker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();
            configuration.AddOpenBehavior(typeof(RequestLoggingPipelineBehaviour<,>));
            configuration.NotificationPublisher = new TaskWhenAllPublisher();
        });
        
        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyReference>();
        

        return services;
    }
}