using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using NexoStamp.Models;

namespace NexoStamp.Services
{
    /// <summary>
    /// Service for saving and loading stamp designs
    /// </summary>
    public class FileService
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public FileService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new StampElementConverter() }
            };
        }

        public void SaveDesign(StampDesign design, string filePath)
        {
            var json = JsonSerializer.Serialize(design, _jsonOptions);
            File.WriteAllText(filePath, json);
        }

        public StampDesign? LoadDesign(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<StampDesign>(json, _jsonOptions);
        }
    }

    /// <summary>
    /// Custom JSON converter for StampElement polymorphism
    /// </summary>
    public class StampElementConverter : JsonConverter<StampElement>
    {
        public override StampElement? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                
                // Check if it's a TextElement or ShapeElement
                if (root.TryGetProperty("Text", out _) || root.TryGetProperty("FontFamily", out _))
                {
                    return JsonSerializer.Deserialize<TextElement>(root.GetRawText(), options);
                }
                else if (root.TryGetProperty("ShapeType", out _))
                {
                    return JsonSerializer.Deserialize<ShapeElement>(root.GetRawText(), options);
                }
                
                return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, StampElement value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
