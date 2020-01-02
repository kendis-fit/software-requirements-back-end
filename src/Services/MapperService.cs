using AutoMapper;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace SoftwareRequirements.Services
{
    public static class MapperService
    {
        public static IServiceCollection AddMapper(this IServiceCollection services, List<Profile> profiles)
        {
            var mappingConfig = new MapperConfiguration(c => {

                foreach (var profile in profiles)
                {
                    c.AddProfile(profile);
                }
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }
    }
}