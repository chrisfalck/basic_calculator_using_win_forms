using System;
using System.Windows;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CalculatorDisplay.Text = "";
        }

        private void OneBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('1', CalculatorDisplay);
        }

        private void TwoBtn_Click_1(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('2', CalculatorDisplay);
        }

        private void ThreeBtn_Click_1(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('3', CalculatorDisplay);
        }

        private void FourBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('4', CalculatorDisplay);
        }

        private void FiveBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('5', CalculatorDisplay);
        }

        private void SixBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('6', CalculatorDisplay);
        }

        private void SvnBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('7', CalculatorDisplay);
        }

        private void EightBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('8', CalculatorDisplay);
        }

        private void NineBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('9', CalculatorDisplay);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('0', CalculatorDisplay);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('.', CalculatorDisplay);
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('+', CalculatorDisplay);
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('-', CalculatorDisplay);
        }

        private void MultBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('*', CalculatorDisplay);
        }

        private void DivBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('/', CalculatorDisplay);
        }

        private void OpenParenBtn_Click(object sender, RoutedEventArgs e)
        {

            CalcDisplay.UpdateDisplay('(', CalculatorDisplay);
        }

        private void CloseParenBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay(')', CalculatorDisplay);
        }

        private void EqualsBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.ShowAnswer(CalculatorDisplay);
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            CalcDisplay.UpdateDisplay('C', CalculatorDisplay);
        }
    }
}
