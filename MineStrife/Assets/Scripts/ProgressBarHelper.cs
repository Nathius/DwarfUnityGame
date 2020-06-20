using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
	public static class ProgressBarHelper
	{
        public static string GetBar(float currentValue, float maxValue, char emptyChar = ' ', char fullChar = '#')
        {
            var bar = "[";

            var progress = currentValue / maxValue;
            var pips = (int)Math.Round(progress * 10);
            for (int i = 0; i <= pips; i++)
            {
                bar += fullChar;
            }
            for (int i = 0; i < (10 - pips); i++)
            {
                bar += emptyChar;
            }

            bar += "]";
            return bar;

        }

	}
}
