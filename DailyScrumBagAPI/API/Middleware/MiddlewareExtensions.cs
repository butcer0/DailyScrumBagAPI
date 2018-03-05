using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DailyScrumBagAPI.API.Services;

namespace DailyScrumBagAPI.API.Middleware
{
    public static class MiddlewareExtensions
    {
        public static async void AddSeedData(this IApplicationBuilder app)
        {
            var seedDataService = app.ApplicationServices.GetRequiredService<ISeedDataService>();
            seedDataService.EnsureSeedData();

            var seedDataUserService = app.ApplicationServices.GetRequiredService<ISeedUserDataService>();
            seedDataUserService.EnsureSeedData();
        }
    }
}
