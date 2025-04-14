using System.IO.Pipes;

namespace ServerConsoleApp
{
    public static class Server
    {
        private static int count = 0;
        private static readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public static int GetCount()
        {
            rwLock.EnterReadLock();
            try
            {
                return count;
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static void AddToCount(int value)
        {
            rwLock.EnterWriteLock();
            try
            {
                count += value;
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public static async Task HandleConnections(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (var pipeServer = new NamedPipeServerStream("CountPipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances))
                    {
                        pipeServer.WaitForConnection();
                        await Task.Run(() => HandleClient(pipeServer));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static void HandleClient(NamedPipeServerStream pipeServer)
        {
            try
            {
                using (var reader = new BinaryReader(pipeServer))
                using (var writer = new BinaryWriter(pipeServer))
                {
                    var operation = reader.ReadChar();
                    switch (operation)
                    {
                        case 'G': // GetCount
                            writer.Write(GetCount());
                            break;
                        case 'A': // AddToCount

                            var value = reader.ReadInt32();
                            AddToCount(value);
                            writer.Write(GetCount()); // Возвращаем обновленное значение
                            break;
                        default:
                            writer.Write(-1); // Неизвестная операция
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
        }
    }
}