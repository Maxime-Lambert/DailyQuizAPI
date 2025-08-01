using DailyQuizAPI.Features.Crosscutting.AppSettings.Create;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.Accept;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.Delete;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.GetAll;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.RemoveFriend;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.Send;
using DailyQuizAPI.Features.Crosscutting.Healthchecks.GetAll;
using DailyQuizAPI.Features.Crosscutting.Users.ConfirmEmail;
using DailyQuizAPI.Features.Crosscutting.Users.Create;
using DailyQuizAPI.Features.Crosscutting.Users.Delete;
using DailyQuizAPI.Features.Crosscutting.Users.ForgotPassword;
using DailyQuizAPI.Features.Crosscutting.Users.GetOne;
using DailyQuizAPI.Features.Crosscutting.Users.Login;
using DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;
using DailyQuizAPI.Features.Crosscutting.Users.Refresh;
using DailyQuizAPI.Features.Crosscutting.Users.ResetPassword;
using DailyQuizAPI.Features.Crosscutting.Users.Rollback;
using DailyQuizAPI.Features.SumotApp.SumotHistories.Add;
using DailyQuizAPI.Features.SumotApp.SumotHistories.GetAll;
using DailyQuizAPI.Features.SumotApp.Sumots.Extract;
using DailyQuizAPI.Features.SumotApp.Sumots.GetAll;

namespace DailyQuizAPI.Features;

public static class FeaturesSetup
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<CreateAppSettingCommandHandler>();

        services.AddScoped<AcceptFriendRequestCommandHandler>();
        services.AddScoped<DeleteFriendRequestCommandHandler>();
        services.AddScoped<GetFriendRequestsQueryHandler>();
        services.AddScoped<RemoveFriendCommandHandler>();
        services.AddScoped<CreateFriendRequestCommandHandler>();

        services.AddScoped<AddSumotHistoriesCommandHandler>();
        services.AddScoped<GetSumotHistoriesQueryHandler>();

        services.AddScoped<ExtractSumotsCommandHandler>();
        services.AddScoped<GetSumotsQueryHandler>();

        services.AddScoped<ConfirmEmailCommandHandler>();
        services.AddScoped<CreateUserCommandHandler>();
        services.AddScoped<DeleteUserCommandHandler>();
        services.AddScoped<ForgotPasswordCommandHandler>();
        services.AddScoped<GetUserQueryHandler>();
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<PartialUpdateUserCommandHandler>();
        services.AddScoped<RefreshCommandHandler>();
        services.AddScoped<ResetPasswordCommandHandler>();
        services.AddScoped<RollbackCommandHandler>();

        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapPostAppSettingEndpoint();

        app.MapAcceptFriendRequestEndpoint();
        app.MapDeleteFriendRequestEndpoint();
        app.MapGetFriendRequestsEndpoint();
        app.MapRemoveFriendEndpoint();
        app.MapSendFriendRequestEndpoint();

        app.MapGetHealthchecks();

        app.MapAddSumotHistoriesEndpoint();
        app.MapGetSumotHistoriesEndpoint();

        app.MapGetSumotsEndpoint();

        app.MapConfirmEmailEndpoint();
        app.MapCreateUserEndpoint();
        app.MapDeleteUserEndpoint();
        app.MapForgotPasswordEndpoint();
        app.MapGetUserEndpoint();
        app.MapLoginEndpoint();
        app.MapPartialUpdateUserEndpoint();
        app.MapRefreshEndpoint();
        app.MapResetPasswordEndpoint();
        app.MapRollbackEndpoint();

        return app;
    }
}
