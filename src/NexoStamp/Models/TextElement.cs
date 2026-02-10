using System.Windows;
using System.Windows.Media;

namespace NexoStamp.Models
{
    /// <summary>
    /// Represents a text element in the stamp design
    /// </summary>
    public class TextElement : StampElement
    {
        public string Text { get; set; } = "Text";
        public string FontFamily { get; set; } = "Arial";
        public double FontSize { get; set; } = 24;
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;

        public TextElement()
        {
            Width = 200;
            Height = 50;
        }

        public override StampElement Clone()
        {
            return new TextElement
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
                TextAlignment = TextAlignment
            };
        }
    }
}
