using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TMS.Identity.Application.Validators;

namespace TMS.Identity.Application;

public static class IdentityApplicationExtensions
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        return services;
    }
}