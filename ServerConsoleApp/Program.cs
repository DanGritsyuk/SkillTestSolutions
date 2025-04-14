using ServerConsoleApp;

Console.WriteLine("Server started. Waiting for connections...");

var cts = new CancellationTokenSource();

// Запускаем обработку подключений в отдельном потоке
var connectRun = Server.HandleConnections(cts.Token);

Console.WriteLine("Press any key to stop the server...");
Console.ReadKey();
cts.Cancel();
await connectRun;