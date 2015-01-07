using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfExtendedWindow.Views.Controls
{
	public partial class DropCircle : UserControl
	{
		public DropCircle()
		{
			InitializeComponent();
		}

		public bool IsAnimating
		{
			get { return (bool)GetValue(IsAnimatingProperty); }
			set { SetValue(IsAnimatingProperty, value); }
		}
		public static readonly DependencyProperty IsAnimatingProperty =
			DependencyProperty.Register(
				"IsAnimating",
				typeof(bool),
				typeof(DropCircle),
				new FrameworkPropertyMetadata(
					false, // Default value must be false.
					async (d, e) =>
					{
						if (!(bool) e.NewValue)
							return;

						var circle = (DropCircle)d;
						circle.Opacity = 1;

						await Task.Delay(TimeSpan.FromSeconds(1.6));

						circle.IsAnimating = false;
						circle.Opacity = 0;
					}));

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == DropCircle.IsAnimatingProperty)
			{
				UpdateStates(true);
			}
		}

		private void UpdateStates(bool useTransitions)
		{
			var state = IsAnimating ? "Animating" : "Normal";

			VisualStateManager.GoToState(this, state, useTransitions);
		}
	}
}