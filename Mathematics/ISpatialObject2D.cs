using System.Numerics;

namespace Shiftless.Common.Mathematics
{
    public interface ISpatialObject2D
    {
        // Properties
        float MinX { get; }
        float MinY { get; }
        float MaxX { get; }
        float MaxY { get; }


        // Functions
        public bool Intersects(ISpatialObject2D other)
        {
            return !(MinX > other.MaxX || MaxX < other.MinX ||
                     MinY > other.MaxY || MaxY < other.MinY);
        }
        public bool Intersects(ICoordinate2D coord)
        {
            return MinX <= coord.X && MinY <= coord.Y &&
                   MaxX > coord.X && MaxY > coord.Y;
        }

        /// <summary>
        /// Gets the manhatan distance between this spatial object and a coordinate
        /// </summary>
        /// <param name="coord">The coordinate to calculate the distance from.</param>
        /// <returns>The distance between the point and the object, -1 if it intersects</returns>
        public float GetManhatanDistance(ICoordinate2D coord)
        {
            if (Intersects(coord))
                return -1;

            float distX = MathF.Min(MathF.Abs(MinX - coord.X), MathF.Abs(MaxX - coord.X));
            float distY = MathF.Min(MathF.Abs(MinY - coord.Y), MathF.Abs(MaxY - coord.Y));

            return distX + distY;
        }
    }
}
