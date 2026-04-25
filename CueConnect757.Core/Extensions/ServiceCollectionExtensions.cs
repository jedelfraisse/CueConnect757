using ConnectAPA.GraphQL;
using CueConnect757.Core.Mappers;
using CueConnect757.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CueConnect757.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCueConnectCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHttpClient<ApaGraphQlClient>();

        services.AddSingleton<PlayerMapper>();
        services.AddSingleton<SessionMapper>();
        services.AddSingleton<DivisionMapper>();
        services.AddSingleton<TeamMapper>();
        services.AddSingleton<MatchMapper>();

        services.AddScoped<IApaSyncService, ApaSyncService>();

        return services;
    }
}
