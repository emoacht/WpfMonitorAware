using System;
using System.Windows;
using System.Windows.Media;

using PerMonitorDpi.Helper;

namespace PerMonitorDpi.Views.Controls
{
    /// <summary>
    /// Attached property to locate inner FrameworkElement at the center of outer FrameworkElement.
    /// </summary>
    public class FrameworkElementCenterProperty : DependencyObject
    {
        /// <summary>
        /// Get AttachedProperty.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static FrameworkElementCenterProperty GetAttachedProperty(DependencyObject obj)
        {
            return (FrameworkElementCenterProperty)obj.GetValue(AttachedPropertyProperty);
        }
        /// <summary>
        /// Set AttachedProperty.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetAttachedProperty(DependencyObject obj, FrameworkElementCenterProperty value)
        {
            obj.SetValue(AttachedPropertyProperty, value);
        }
        /// <summary>
        /// Attached property for AttachedProperty
        /// </summary>
        public static readonly DependencyProperty AttachedPropertyProperty =
            DependencyProperty.RegisterAttached(
                "AttachedProperty",
                typeof(FrameworkElementCenterProperty),
                typeof(FrameworkElementCenterProperty),
                new PropertyMetadata(
                    null,
                    (d, e) =>
                    {
                        var innerElement = (FrameworkElement)d;
                        if (innerElement == null)
                            return;

                        var outerElement = VisualTreeHelper.GetParent(innerElement) as FrameworkElement;
                        if (outerElement == null)
                            return;

                        ((FrameworkElementCenterProperty)e.NewValue).InnerElement = innerElement;
                        ((FrameworkElementCenterProperty)e.NewValue).OuterElement = outerElement;
                    }));

        private FrameworkElement InnerElement
        {
            get { return _innerElement; }
            set
            {
                if (_innerElement != null)
                    _innerElement.LayoutUpdated -= OnLayoutUpdated;

                if (value == null)
                    return;

                _innerElement = value;
                _innerElement.LayoutUpdated += OnLayoutUpdated;
            }
        }
        private FrameworkElement _innerElement;

        private FrameworkElement OuterElement { get; set; }

        /// <summary>
        /// Whether inner FrameworkElement to be horizontally centered
        /// </summary>
        public bool IsHorizontalAlignmentCenter { get; set; }

        /// <summary>
        /// Whether inner FrameworkElement to be vertically centered
        /// </summary>
        public bool IsVerticalAlignmentCenter { get; set; }

        private enum RoundingType
        {
            Round = 0,
            Floor,
            Ceiling,
        }

        private RoundingType roundingValue;

        /// <summary>
        /// Rounding
        /// </summary>
        /// <remarks>This string must be one of RoundingType names.</remarks>
        public string Rounding
        {
            get { return _rounding; }
            set
            {
                _rounding = value;

                roundingValue = EnumAddition.IsDefined(typeof(RoundingType), value, StringComparison.OrdinalIgnoreCase)
                    ? (RoundingType)EnumAddition.Parse(typeof(RoundingType), value, StringComparison.OrdinalIgnoreCase)
                    : default(RoundingType);
            }
        }
        private string _rounding;

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            double leftMargin = 0D;
            if (IsHorizontalAlignmentCenter)
            {
                leftMargin = CalculateMargin(InnerElement.ActualWidth, OuterElement.ActualWidth);
                InnerElement.HorizontalAlignment = HorizontalAlignment.Left;
            }

            double topMargin = 0D;
            if (IsVerticalAlignmentCenter)
            {
                topMargin = CalculateMargin(InnerElement.ActualHeight, OuterElement.ActualHeight);
                InnerElement.VerticalAlignment = VerticalAlignment.Top;
            }

            InnerElement.Margin = new Thickness(leftMargin, topMargin, 0, 0);
        }

        private double CalculateMargin(double innerLength, double outerLength)
        {
            var buff = Math.Abs(innerLength - outerLength) / 2;

            switch (roundingValue)
            {
                case RoundingType.Floor:
                    return Math.Floor(buff);

                case RoundingType.Ceiling:
                    return Math.Ceiling(buff);

                default:
                    return Math.Round(buff);
            }
        }
    }
}