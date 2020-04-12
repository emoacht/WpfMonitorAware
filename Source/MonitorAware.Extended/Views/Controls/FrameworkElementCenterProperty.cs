using System;
using System.Windows;
using System.Windows.Media;

using MonitorAware.Extended.Helper;

namespace MonitorAware.Extended.Views.Controls
{
	/// <summary>
	/// Attached property to locate inner <see cref="FrameworkElement"/> at the center of outer <see cref="FrameworkElement"/>
	/// </summary>
	public class FrameworkElementCenterProperty : DependencyObject
	{
		/// <summary>
		/// Gets AttachedProperty.
		/// </summary>
		/// <param name="element">Owner <see cref="FrameworkElement"/></param>
		/// <returns>AttachedProperty</returns>
		public static FrameworkElementCenterProperty GetAttachedProperty(FrameworkElement element)
		{
			return (FrameworkElementCenterProperty)element.GetValue(AttachedPropertyProperty);
		}
		/// <summary>
		/// Sets AttachedProperty.
		/// </summary>
		/// <param name="element">Owner <see cref="FrameworkElement"/></param>
		/// <param name="attachedProperty">AttachedProperty</param>
		public static void SetAttachedProperty(FrameworkElement element, FrameworkElementCenterProperty attachedProperty)
		{
			element.SetValue(AttachedPropertyProperty, attachedProperty);
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
		/// Whether inner <see cref="FrameworkElement"/> to be horizontally centered
		/// </summary>
		public bool IsHorizontalAlignmentCenter { get; set; }

		/// <summary>
		/// Whether inner <see cref="FrameworkElement"/> to be vertically centered
		/// </summary>
		public bool IsVerticalAlignmentCenter { get; set; }

		private enum RoundingType
		{
			Round = 0,
			Floor,
			Ceiling,
		}

		private RoundingType _roundingValue;

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

				_roundingValue = EnumAddition.IsDefined(typeof(RoundingType), value, StringComparison.OrdinalIgnoreCase)
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
			var marginLength = (outerLength - innerLength) / 2;

			switch (_roundingValue)
			{
				case RoundingType.Floor:
					return Math.Floor(marginLength);

				case RoundingType.Ceiling:
					return Math.Ceiling(marginLength);

				default:
					return Math.Round(marginLength);
			}
		}
	}
}