using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

using PerMonitorDpi.Helper;

namespace PerMonitorDpi.Views.Controls
{
    /// <summary>
    /// Attached property to center inner FrameworkElement at the center of outer FrameworkElement.
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
        /// <remarks>This property cannot have binding from this property because this is merely DependencyObject.</remarks>
        public bool IsHorizontalCentered
        {
            get { return (bool)GetValue(IsHorizontalCenteredProperty); }
            set { SetValue(IsHorizontalCenteredProperty, value); }
        }
        /// <summary>
        /// Dependency property for <see cref="IsHorizontalCentered"/>
        /// </summary>
        public static readonly DependencyProperty IsHorizontalCenteredProperty =
            DependencyProperty.Register(
                "IsHorizontalCentered",
                typeof(bool),
                typeof(FrameworkElementCenterProperty),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Whether inner FrameworkElement to be vertically centered
        /// </summary>
        /// <remarks>
        /// </remarks>
        public bool IsVerticalCentered
        {
            get { return (bool)GetValue(IsVerticalCenteredProperty); }
            set { SetValue(IsVerticalCenteredProperty, value); }
        }
        /// <summary>
        /// Dependency property for <see cref="IsVerticalCentered"/>
        /// </summary>
        public static readonly DependencyProperty IsVerticalCenteredProperty =
            DependencyProperty.Register(
                "IsVerticalCentered",
                typeof(bool),
                typeof(FrameworkElementCenterProperty),
                new FrameworkPropertyMetadata(false));

        private enum RoundingType
        {
            Round = 0,
            Floor,
            Ceiling,
        }

        /// <summary>
        /// Rounding
        /// </summary>
        /// <remarks>This string must be one of RoundingType names.</remarks>
        public string Rounding { get; set; }

        private RoundingType RoundingValue
        {
            get
            {
                if (!_roundingValue.HasValue)
                {
                    _roundingValue = (!String.IsNullOrEmpty(Rounding) && EnumAddition.IsDefined(typeof(RoundingType), Rounding, StringComparison.OrdinalIgnoreCase))
                        ? (RoundingType)EnumAddition.Parse(typeof(RoundingType), Rounding, StringComparison.OrdinalIgnoreCase)
                        : default(RoundingType);
                }

                return _roundingValue.Value;
            }
        }
        private RoundingType? _roundingValue;

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            double leftMargin = 0D;
            if (IsHorizontalCentered)
            {
                leftMargin = CalculateMargin(InnerElement.ActualWidth, OuterElement.ActualWidth);
                InnerElement.HorizontalAlignment = HorizontalAlignment.Left;
            }

            double topMargin = 0D;
            if (IsVerticalCentered)
            {
                topMargin = CalculateMargin(InnerElement.ActualHeight, OuterElement.ActualHeight);
                InnerElement.VerticalAlignment = VerticalAlignment.Top;
            }

            InnerElement.Margin = new Thickness(leftMargin, topMargin, 0, 0);
        }

        private double CalculateMargin(double innerLength, double outerLength)
        {
            var buff = Math.Abs(innerLength - outerLength) / 2;

            switch (RoundingValue)
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