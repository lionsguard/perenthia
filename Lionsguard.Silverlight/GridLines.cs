using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Lionsguard
{
    [TemplatePart(Name = "RootElement", Type = typeof(Grid))]
    public class GridLines : Control
    {
        private Grid RootElement { get; set; }

        public double HorizontalSpacing
        {
            get { return (double)GetValue(HorizontalSpacingProperty); }
            set { SetValue(HorizontalSpacingProperty, value); }
        }
        public static readonly DependencyProperty HorizontalSpacingProperty = DependencyProperty.Register("HorizontalSpacing", typeof(double), typeof(GridLines), new PropertyMetadata(8.0, new PropertyChangedCallback(GridLines.OnHorizontalSpacingPropertyChanged)));
        private static void OnHorizontalSpacingPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as GridLines).UpdateSpacing();
        }

        public double VerticalSpacing
        {
            get { return (double)GetValue(VerticalSpacingProperty); }
            set { SetValue(VerticalSpacingProperty, value); }
        }
        public static readonly DependencyProperty VerticalSpacingProperty = DependencyProperty.Register("VerticalSpacing", typeof(double), typeof(GridLines), new PropertyMetadata(8.0, new PropertyChangedCallback(GridLines.OnVerticalSpacingPropertyChanged)));
        private static void OnVerticalSpacingPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as GridLines).UpdateSpacing();
        }
            
        public GridLines()
        {
            this.DefaultStyleKey = typeof(GridLines);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.RootElement = base.GetTemplateChild("RootElement") as Grid;
            this.UpdateSpacing();
        }

        private void UpdateSpacing()
        {
            if (this.RootElement != null)
            {
                this.RootElement.Children.Clear();

                // |--|--|--|--|--|--|--|
                // |--|--|--|--|--|--|--|
                // |--|--|--|--|--|--|--|

                // Handle the horizontal lines  --
                int horizontalLineCount = (int)(this.ActualHeight / this.HorizontalSpacing);
                for (int i = 0; i < horizontalLineCount; i++)
                {
                    // Horizontal lines from top to bottom.
                    Line line = new Line();
					line.Stroke = this.Foreground;
					line.StrokeThickness = 1.0;
                    line.X1 = 0;
                    line.X2 = this.ActualWidth;
                    line.SetValue(Canvas.TopProperty, i * horizontalLineCount);
                    this.RootElement.Children.Add(line);
                }

                // Handle the vertical lines  |
                int verticalLineCount = (int)(this.ActualWidth / this.VerticalSpacing);
                for (int i = 0; i < verticalLineCount; i++)
                {
                    // Vertical lines from left to right
					Line line = new Line();
					line.Stroke = this.Foreground;
					line.StrokeThickness = 1.0;
                    line.Y1 = 0;
                    line.Y2 = this.ActualHeight;
                    line.SetValue(Canvas.LeftProperty, i * verticalLineCount);
                    this.RootElement.Children.Add(line);
                }
            }
        }
    }
}
