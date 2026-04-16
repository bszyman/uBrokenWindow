using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using uBrokenWindow.Models;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace uBrokenWindow.Handlers;

public class BrokenWindowStartupHandler(
    IHostEnvironment environment, 
    IUserService userService, 
    IServiceScopeFactory scopeFactory, 
    IOptions<GlobalSettings> globalSettings, 
    IConfiguration configuration,
    ILogger<BrokenWindowStartupHandler> logger): 
    INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
    {
        var brokenWindowConfiguration = configuration.GetSection("uBrokenWindow").Get<BrokenWindowConfiguration>();

        if (brokenWindowConfiguration is null)
        {
            logger.LogWarning("uBrokenWindow configuration section not found. uBrokenWindow will not be enabled.");
            return;
        }
        
        if (!brokenWindowConfiguration.EnableBrokenWindow)
        {
            logger.LogWarning("uBrokenWindow is disabled. Skipping account creation...");
            return;
        }
        
        var existingUser = userService.GetByUsername(brokenWindowConfiguration.NewAdminEmail);
        if (existingUser is not null)
        {
            logger.LogWarning("uBrokenWindow (user) already exists. Skipping account creation...");
            return;
        }

        if (string.IsNullOrWhiteSpace(brokenWindowConfiguration.NewAdminEmail) ||
            string.IsNullOrWhiteSpace(brokenWindowConfiguration.NewAdminPassword))
        {
            logger.LogWarning("uBrokenWindow (user) email or password is empty. Skipping account creation...");
            return;
        }
        
        var user = BackOfficeIdentityUser.CreateNew(
            globalSettings.Value, 
            brokenWindowConfiguration.NewAdminEmail, 
            brokenWindowConfiguration.NewAdminEmail, 
            globalSettings.Value.DefaultUILanguage,
            "uBrokenWindow User"
        );

        user.IsApproved = true;
        
        using var scope = scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<IBackOfficeUserManager>();
        
        var result = await userManager.CreateAsync(user, brokenWindowConfiguration.NewAdminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRolesAsync(user, ["admin"]);
            logger.LogInformation($"uBrokenWindow (user) created: {brokenWindowConfiguration.NewAdminEmail}");
        }
        else
        {
            logger.LogError($"uBrokenWindow (user) creation failed: {result.Errors}");
        }
    }
}