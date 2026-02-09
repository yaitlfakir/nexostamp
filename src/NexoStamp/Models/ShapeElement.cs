namespace NexoStamp.Models
{
    /// <summary>
    /// Represents a shape element in the stamp design
    /// </summary>
    public class ShapeElement : StampElement
    {
        public ShapeType ShapeType { get; set; }
        public double StrokeThickness { get; set; } = 2;

        public ShapeElement()
        {
            Width = 100;
            Height = 100;
        }

        public override StampElement Clone()
        {
            return new ShapeElement
            {
                Id = System.Guid.NewGuid().ToString(),
                X = X + 10,
                Y = Y + 10,
                Width = Width,
                Height = Height,
                Rotation = Rotation,
                ZIndex = ZIndex,
                ShapeType = ShapeType,
                StrokeThickness = StrokeThickness
            };
        }
    }

    public enum ShapeType
    {
        Rectangle,
        Circle,
        Line
    }
}
