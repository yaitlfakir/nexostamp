using System.Collections.Generic;

namespace NexoStamp.Models
{
    /// <summary>
    /// Represents a complete stamp design
    /// </summary>
    public class StampDesign
    {
        public string Name { get; set; } = "Untitled Stamp";
        public double CanvasWidth { get; set; } = 400;
        public double CanvasHeight { get; set; } = 300;
        public List<StampElement> Elements { get; set; } = new List<StampElement>();

        public StampDesign()
        {
        }
    }
}
