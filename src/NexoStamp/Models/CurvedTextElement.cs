using System.Windows;
using System.Windows.Media;

namespace NexoStamp.Models
{
    /// <summary>
    /// Represents text that follows a circular path
    /// </summary>
    public class CurvedTextElement : StampElement
    {
        public string Text { get; set; } = "Curved Text";
        public string FontFamily { get; set; } = "Arial";
        public double FontSize { get; set; } = 24;
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;
        public double Radius { get; set; } = 100;
        public double StartAngle { get; set; } = 0;
        public double EndAngle { get; set; } = 360;
        public bool IsInverted { get; set; } = false;

        public CurvedTextElement()
        {
            Width = Radius * 2;
            Height = Radius * 2;
        }

        public override StampElement Clone()
        {
            return new CurvedTextElement
            {
                Id = System.Guid.NewGuid().ToString(),
                X = X + 10,
                Y = Y + 10,
                Width = Width,
                Height = Height,
                Rotation = Rotation,
                ZIndex = ZIndex,
                Text = Text,
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontWeight = FontWeight,
                FontStyle = FontStyle,
                Radius = Radius,
                StartAngle = StartAngle,
                EndAngle = EndAngle,
                IsInverted = IsInverted
            };
        }
    }
}
