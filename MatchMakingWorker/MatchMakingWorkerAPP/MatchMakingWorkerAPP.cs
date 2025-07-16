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

Console.WriteLine("It works!");
await Task.Delay(-1);