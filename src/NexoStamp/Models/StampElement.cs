using System;
using System.Windows;

namespace NexoStamp.Models
{
    /// <summary>
    /// Base class for all stamp design elements
    /// </summary>
    public abstract class StampElement
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Rotation { get; set; }
        public bool IsSelected { get; set; }
        public int ZIndex { get; set; }

        protected StampElement()
        {
        }

        public abstract StampElement Clone();
    }
}
