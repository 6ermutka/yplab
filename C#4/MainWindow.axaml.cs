using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;

namespace Lab9
{
    public partial class MainWindow : Window
    {
        private InputValidator validator = new InputValidator();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OnExecuteClick(object sender, RoutedEventArgs e)
        {
            resultsPanel.Children.Clear();
            lblError.Text = string.Empty;
            try
            {
                double x = validator.GetValidDoubleNumber(txtX.Text);
                double y = validator.GetValidDoubleNumber(txtY.Text);
                double N = validator.GetValidDoubleNumber(txtN.Text);
                
                LineSegment segment1 = new LineSegment();
                AddResult($"Дефолтный объект: {segment1}");
                LineSegment segment2 = new LineSegment(x, y);
                AddResult($"Объект на основе введенных x, y: {segment2}");
                AddResult($"Содержит {N}: {segment2.Contains(N)}");
                AddResult($"Длина отрезка: {!segment2}");
                segment2++;
                AddResult($"Новые координаты(+1): {segment2}");
                int xInt = (int)segment2;
                AddResult($"Целая часть: {xInt} от {segment2.X}");
                double yDouble = segment2;
                AddResult($"double Y: {yDouble}");
                LineSegment shiftedRight = segment2 + 3;
                AddResult($"Начало отрезка + 3: {shiftedRight}");
                LineSegment shiftedLeft = 2 + segment2;
                AddResult($"2 + Конец отрезка: {shiftedLeft}");
                AddResult($"Проверка {N}: {segment2 < N} (число {N} {(segment2 < N ? "входит" : "не входит")} в отрезок)");
            }
            catch (ArgumentException ex)
            {
                lblError.Text = ex.Message;
            }
            catch (Exception ex)
            {
                lblError.Text = $"Ошибка: {ex.Message}";
            }
        }
        private void AddResult(string text)
        {
            resultsPanel.Children.Add(new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap });
        }
    }
}