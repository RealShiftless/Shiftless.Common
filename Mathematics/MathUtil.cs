namespace Shiftless.Common.Mathematics
{
    public static class MathUtil
    {
        public static Axis ToAxis(this Direction direction) => direction switch
        {
            Direction.Up => Axis.Vertical,
            Direction.Down => Axis.Vertical,
            Direction.Left => Axis.Horizontal,
            Direction.Right => Axis.Horizontal,

            _ => throw new NotImplementedException($"Direction {direction} went unhandled!")
        };

        public static Direction Invert(this Direction direction) => direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Left => Direction.Right,
            Direction.Down => Direction.Up,
            Direction.Right => Direction.Left,

            _ => throw new NotImplementedException($"Direction {direction} went unhandled!")
        };
    }
}
