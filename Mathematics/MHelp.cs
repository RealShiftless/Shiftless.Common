using System;
using System.Collections.Generic;
using System.Text;

namespace Shiftless.Common.Mathematics
{
    public static class MHelp
    {
        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }


        public static float Map(float v, float oldMin, float oldMax, float newMin, float newMax)
        {
            return (v - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
        }

        public static byte Map(byte v, byte fromMax, byte toMax) => (byte)((v * toMax + fromMax / 2) / fromMax);
    }
}
