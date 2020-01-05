using Microsoft.Extensions.DependencyInjection;

using SoftwareRequirements.Repositories;

namespace SoftwareRequirements.Services
{
    public static class RepositoryService
    {
        public static IServiceCollection AddProjectRepository(this IServiceCollection services)
        {
            services.AddTransient<ProjectRepository>();
            return services;
        }

        public static IServiceCollection AddRequirementRepository(this IServiceCollection services)
        {
            services.AddTransient<RequirementRepository>();
            return services;
        }
    }
}