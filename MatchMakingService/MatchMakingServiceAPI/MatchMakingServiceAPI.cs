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

Console.WriteLine("It works!");
await Task.Delay(-1);