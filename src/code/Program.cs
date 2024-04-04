using System.Diagnostics;
using System.Text.Json;

namespace EpicGamesStarter
{
    public static class EpicGamesStarter
    {
        public const string AppId = "33956bcb55d4452d8c47e16b94e294bd%3A729a86a5146640a2ace9e8c595414c56%3A963137e4c29d4c79a81323b8fab03a40";

        public static string Launcher = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}\Epic Games\Launcher\Portal\Binaries\Win64";

        public static readonly JsonSerializerOptions Opts = new()
        {
            TypeInfoResolver = SourceGenerator.Default
        };

        public static async Task Main(string[] args)
        {
            if (!File.Exists("Among Us.exe"))
            {
                Console.WriteLine("No among us exe detected in current folder, cannot continue");
                Console.ReadLine();
                return;
            }

            try
            {
                Console.WriteLine("Any open Among Us windows will be closed now");
                if (InstancesClosed()) await Task.Delay(1000);

                Console.WriteLine("Starting Among Us in the epic folder to retrieve arguments");
                var BaseGame = Process.Start($@"{Launcher}\EpicGamesLauncher.exe", $"com.epicgames.launcher://apps/{AppId}?action=launch&silent=true");
                while (Process.GetProcessesByName("Among Us").Length == 0)
                {
                    Console.WriteLine("Waiting...");
                    await Task.Delay(1000);
                }
                
                var data = GetIdAndCommandLine();
                var arguments = data.CommandLine[data.CommandLine.IndexOf("-AUTH_LOGIN")..]; 
                
                Console.WriteLine("Starting the game in this folder and closing epic's");
                Process.Start("Among Us.exe", arguments);
                Process.GetProcessById(data.ProcessId).Kill();
                
                Console.WriteLine("done");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
        
        public static ProcessData GetIdAndCommandLine()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "Get-CimInstance -ClassName Win32_Process | Select ProcessId, CommandLine | ConvertTo-json",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                using var process = new Process();
                process.StartInfo = processStartInfo;
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                var data = JsonSerializer.Deserialize<List<ProcessData>>(output, Opts).Where(x => x.CommandLine != null);
                return data.FirstOrDefault(x => x.CommandLine.Contains("Among Us.exe"))!;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null!;
            }
        }

        public static bool InstancesClosed()
        {
            var instances = Process.GetProcessesByName("Among Us");
            if (instances.Length == 0) return false;

            foreach (var instance in instances)
            {
                instance.Kill();
            }
            return true;
        }
    }
}
