using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Calculator
{
    class CalcDisplay
    {
        private static StringBuilder currDisplay;

        // Update the display for the button pressed
        public static void UpdateDisplay(char btnPressed, TextBlock display)
        {
            if (btnPressed == 'C')
            {
                // Clear current value and reset the display
                currDisplay = new StringBuilder("");
                display.Text = currDisplay.ToString();
                return;
            }
            currDisplay = new StringBuilder(display.Text);
            display.Text = currDisplay.Append(btnPressed).ToString();
        }

        public static void ShowAnswer(TextBlock display)
        {
            display.Text = InfixParser.ParseInfixString(display.Text);
        }
    }
}
