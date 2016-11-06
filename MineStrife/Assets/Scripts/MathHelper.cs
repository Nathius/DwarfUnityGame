using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
	public static class MathHelper
	{
        public static bool IsEven(int inNum)
        {
            return inNum % 2 == 0;
        }

        public static bool IsOdd(int inNum)
        {
            return !IsEven(inNum);
        }

	}
}
