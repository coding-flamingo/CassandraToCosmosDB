using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CassandraToAzureFiles.models
{
    public class CassandraTableModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        public JsonObj jsonContents { get; set; } = new JsonObj();
    }
    public class JsonObj
    {
        [JsonPropertyName("columns")]
        public List<Columns> Columns { get; set; } = new();
        [JsonPropertyName("partitionKeys")]
        public List<Keys> PartitionKeys { get; set; } = new();
    }
    public class Columns
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
    public class Keys
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
