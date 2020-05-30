using System;
using System.Windows;
using System.Windows.Media;

namespace WpfExtendedWindow.Views.Controls
{
	/// <summary>
	/// Attached property to locate inner <see cref="System.Windows.FrameworkElement"/> at the center of outer <see cref="System.Windows.FrameworkElement"/>
	/// </summary>
	public class FrameworkElementCenterProperty : DependencyObject
	{
		/// <summary>
		/// Gets FrameworkElementCenterProperty instance.
		/// </summary>
		/// <param name="element">Owner <see cref="System.Windows.FrameworkElement"/></param>
		/// <returns>FrameworkElementCenterProperty</returns>
		public static FrameworkElementCenterProperty GetInstance(FrameworkElement element)
		{
			return (FrameworkElementCenterProperty)element.GetValue(InstanceProperty);
		}
		/// <summary>
		/// Sets FrameworkElementCenterProperty instance.
		/// </summary>
		/// <param name="element">Owner <see cref="System.Windows.FrameworkElement"/></param>
		/// <param name="instance">FrameworkElementCenterProperty</param>
		public static void SetInstance(FrameworkElement element, FrameworkElementCenterProperty instance)
		{
			element.SetValue(InstanceProperty, instance);
		}
		/// <summary>
		/// Attached property for Instance
		/// </summary>
		public static readonly DependencyProperty InstanceProperty =
			DependencyProperty.RegisterAttached(
				"Instance",
				typeof(FrameworkElementCenterProperty),
				typeof(FrameworkElementCenterProperty),
				new PropertyMetadata(
					null,
					(d, e) =>
					{
						var innerElement = d as FrameworkElement;
						if (innerElement is null)
							return;

						var outerElement = VisualTreeHelper.GetParent(innerElement) as FrameworkElement;
						if (outerElement is null)
							return;

						((FrameworkElementCenterProperty)e.NewValue).InnerElement = innerElement;
						((FrameworkElementCenterProperty)e.NewValue).OuterElement = outerElement;
					}));

		private FrameworkElement InnerElement
		{
			get => _innerElement;
			set
			{
				if (_innerElement != null)
					_innerElement.LayoutUpdated -= OnLayoutUpdated;

				if (value is null)
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

		/// <summary>
		/// Rounding
		/// </summary>
		/// <remarks>This string must be one of RoundingType names.</remarks>
		public string Rounding
		{
			get => _rounding;
			set
			{
				_rounding = value;

				_roundingValue = Enum.TryParse(value, true, out RoundingType buffer)
					? buffer
					: default;
			}
		}
		private string _rounding;
		private RoundingType _roundingValue;

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

			return _roundingValue switch
			{
				RoundingType.Floor => Math.Floor(marginLength),
				RoundingType.Ceiling => Math.Ceiling(marginLength),
				_ => Math.Round(marginLength),
			};
		}
	}
}