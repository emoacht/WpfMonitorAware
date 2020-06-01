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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SlateElement;

namespace WpfSlateWindow.Views
{
	public partial class MainWindow : SlateWindow
	{
		public MainWindow() : base(SlateWindow.PrototypeResourceUriString)
		{
			InitializeComponent();
		}
	}
}