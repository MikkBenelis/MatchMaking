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

builder.Configuration.AddJsonFile($"{nameof(MatchMakingWorker)}.json");

builder.Services.AddSingleton<MatchUserConsumer>();
builder.Services.AddHostedService<MatchUserConsumer>(provider =>
    provider.GetRequiredService<MatchUserConsumer>());

builder.Services.AddHostedService<MatchResultProducer>();

#endregion

#region Application

var application = builder.Build();
await application.RunAsync();

#endregion