using System.Text.Json.Serialization;

namespace EpicGamesStarter
{
    public class ProcessData
    {
        public int ProcessId { get; set; }

        public required string CommandLine { get; set; }
    }

    [JsonSerializable(typeof(ProcessData))]
    [JsonSerializable(typeof(List<ProcessData>))]
    
    public partial class SourceGenerator : JsonSerializerContext
    {

    }
}