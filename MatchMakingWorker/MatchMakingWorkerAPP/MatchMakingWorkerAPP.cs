#region Title

Console.BackgroundColor = ConsoleColor.Black;
Console.ForegroundColor = ConsoleColor.Yellow;

Console.WriteLine("""
                   __  __       _       _     __  __       _    _              __        _____  ____  _  _______ ____
                  |  \/  | __ _| |_ ___| |__ |  \/  | __ _| | _(_)_ __   __ _  \ \      / / _ \|  _ \| |/ / ____|  _ \
                  | |\/| |/ _` | __/ __| '_ \| |\/| |/ _` | |/ / | '_ \ / _` |  \ \ /\ / / | | | |_) | ' /|  _| | |_) |
                  | |  | | (_| | || (__| | | | |  | | (_| |   <| | | | | (_| |   \ V  V /| |_| |  _ <| . \| |___|  _ <
                  |_|  |_|\__,_|\__\___|_| |_|_|  |_|\__,_|_|\_\_|_| |_|\__, |    \_/\_/  \___/|_| \_\_|\_\_____|_| \_\
                                                                        |___/
                  """);

Console.WriteLine();
Console.ResetColor();

#endregion

#region Builder

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("MatchMakingWorkerAPP.json");
builder.Logging.AddMatchMakingLoggerProvider();

#region Redis

var redisConnectionString = builder.Configuration.GetConnectionString(
    Constants.Configuration.ConnectionStrings.Redis)!;

var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnectionMultiplexer);

#endregion

#region Services

builder.Services.AddSingleton<MatchUserConsumerService>();
builder.Services.AddHostedService<MatchUserConsumerService>(provider =>
    provider.GetRequiredService<MatchUserConsumerService>());

builder.Services.AddHostedService<MatchResultProducerService>();

#endregion

#endregion

#region Application

var application = builder.Build();
await application.RunAsync();

#endregion