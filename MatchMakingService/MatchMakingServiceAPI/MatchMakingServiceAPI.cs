#region Title

Console.BackgroundColor = ConsoleColor.Black;
Console.ForegroundColor = ConsoleColor.Yellow;

Console.WriteLine("""
                   __  __       _       _     __  __       _    _               ____  _____ ______     _____ ____ _____
                  |  \/  | __ _| |_ ___| |__ |  \/  | __ _| | _(_)_ __   __ _  / ___|| ____|  _ \ \   / /_ _/ ___| ____|
                  | |\/| |/ _` | __/ __| '_ \| |\/| |/ _` | |/ / | '_ \ / _` | \___ \|  _| | |_) \ \ / / | | |   |  _|
                  | |  | | (_| | || (__| | | | |  | | (_| |   <| | | | | (_| |  ___) | |___|  _ < \ V /  | | |___| |___
                  |_|  |_|\__,_|\__\___|_| |_|_|  |_|\__,_|_|\_\_|_| |_|\__, | |____/|_____|_| \_\ \_/  |___\____|_____|
                                                                        |___/
                  """);

Console.WriteLine();
Console.ResetColor();

#endregion

#region Builder

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("MatchMakingServiceAPI.json");
builder.Logging.AddMatchMakingRedisLoggerProvider();

#region Rate Limiter

var useGlobalRateLimiter = builder.Configuration.GetValue<bool>(
    Constants.Configuration.MatchMaking.RateLimiter.IsActiveGloballyKey);

var useMatchSearchRateLimiter = builder.Configuration.GetValue<bool>(
    Constants.Configuration.MatchMaking.RateLimiter.IsActiveForMatchSearchKey);

var useMatchStatusRateLimiter = builder.Configuration.GetValue<bool>(
    Constants.Configuration.MatchMaking.RateLimiter.IsActiveForMatchStatusKey);

if (useGlobalRateLimiter)
    builder.Services.AddRateLimiter(serviceOptions =>
    {
        serviceOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        serviceOptions.AddFixedWindowLimiter("PerIPRateLimit",
            limiterOptions =>
            {
                var millisecondsLimit = builder.Configuration.GetValue<int>(
                    Constants.Configuration.MatchMaking.RateLimiter.MillisecondsLimitKey);
                limiterOptions.Window = TimeSpan.FromMilliseconds(millisecondsLimit);

                limiterOptions.PermitLimit = builder.Configuration.GetValue<int>(
                    Constants.Configuration.MatchMaking.RateLimiter.RequestsCountLimitKey);
                limiterOptions.QueueLimit = builder.Configuration.GetValue<int>(
                    Constants.Configuration.MatchMaking.RateLimiter.RequestsQueueLimitKey);
            });
    });

#endregion

#region Swagger

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

#region Redis

var redisConnectionString = builder.Configuration.GetConnectionString(
    Constants.Configuration.ConnectionStrings.RedisName)!;

var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnectionMultiplexer);

#endregion

#region Services

builder.Services.AddSingleton<MatchUserProducerService>();
builder.Services.AddHostedService<MatchUserProducerService>(provider =>
    provider.GetRequiredService<MatchUserProducerService>());

builder.Services.AddSingleton<MatchResultConsumerService>();
builder.Services.AddHostedService<MatchResultConsumerService>(provider =>
    provider.GetRequiredService<MatchResultConsumerService>());

#endregion

#endregion

#region Application

var application = builder.Build();

if (builder.Configuration.GetValue<bool>(
        Constants.Configuration.AllowSwaggerKey))
{
    application.MapOpenApi();
    application.UseSwagger();
    application.UseSwaggerUI();
}

if (builder.Configuration.GetValue<bool>(
        Constants.Configuration.ForceHTTPSRedirectKey))
    application.UseHttpsRedirection();

if (useGlobalRateLimiter)
    application.UseRateLimiter();

#endregion

#region Endpoints

var matchesSearchRoute = application.MapPost("/matches/search",
        (RequestMatchSearchDTO request, MatchUserProducerService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.UserID))
                return Results.BadRequest(Constants.APIMessages.UserIDIsRequired);

            var result = service.AddUserToQueue(new MatchUserModel { UserID = request.UserID });
            return result.Success ? Results.NoContent() : Results.Conflict(result.ErrorMessage);
        })
    .WithName("Match Search Request")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status409Conflict);

if (useGlobalRateLimiter && useMatchSearchRateLimiter)
    matchesSearchRoute
        .Produces(StatusCodes.Status429TooManyRequests)
        .RequireRateLimiting("PerIPRateLimit");

var matchesStatusRoute = application.MapPost("/matches/status",
        (RequestMatchStatusDTO request, MatchResultConsumerService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.UserID))
                return Results.BadRequest(Constants.APIMessages.UserIDIsRequired);

            var matchInfo = service.GetMatchForUser(request.UserID);
            return matchInfo.Success ? Results.Ok(matchInfo) : Results.NotFound();
        })
    .WithName("Retrieve Match Information")
    // ReSharper disable once RedundantArgumentDefaultValue
    .Produces<ResponseMatchStatusDTO>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

if (useGlobalRateLimiter && useMatchStatusRateLimiter)
    matchesStatusRoute
        .Produces(StatusCodes.Status429TooManyRequests)
        .RequireRateLimiting("PerIPRateLimit");

#endregion

#region Welcome

Console.BackgroundColor = ConsoleColor.Black;
Console.ForegroundColor = ConsoleColor.Yellow;

Console.WriteLine("HTTP/S endpoints can be tested using provided URLs below:");
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Write("http://localhost:800/swagger");
Console.ForegroundColor = ConsoleColor.White;
Console.Write(" and ");
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Write("https://localhost:4430/swagger");

Console.WriteLine("\n");
Console.ResetColor();

#endregion

await application.RunAsync();