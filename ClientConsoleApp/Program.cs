using System.IO.Pipes;

if (args.Length == 0)
{
    Console.WriteLine("Instruction:");
    Console.WriteLine("get - get counter value");
    Console.WriteLine("add <num> - add to counter");
    return;
}

try
{
    using (var pipeClient = new NamedPipeClientStream(".", "CountPipe", PipeDirection.InOut))
    {
        pipeClient.Connect(5000);

        using (var writer = new BinaryWriter(pipeClient))
        using (var reader = new BinaryReader(pipeClient))
        {
            if (args[0].ToLower() == "get")
            {
                writer.Write('G'); // Операция Get
                var count = reader.ReadInt32();
                Console.WriteLine($"Value: {count}");
            }
            else if (args[0].ToLower() == "add" && args.Length > 1)
            {
                if (int.TryParse(args[1], out int value))
                {
                    writer.Write('A'); // Операция Add
                    writer.Write(value);
                    var newCount = reader.ReadInt32();
                    Console.WriteLine($"Added {value}. New value: {newCount}");
                }
                else
                {
                    Console.WriteLine("Invalid number to add");
                }
            }
            else
            {
                Console.WriteLine("Unknown command");
            }
        }
    }
}
catch (TimeoutException)
{
    Console.WriteLine("Failed to connect to server. Timeout...");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}