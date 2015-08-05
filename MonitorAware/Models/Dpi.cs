using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MonitorAware.Models
{
	/// <summary>
	/// DPI information
	/// </summary>
	/// <remarks>
	/// This structure is based on the same structure of https://github.com/Grabacr07/XamClaudia
	/// </remarks>
	public struct Dpi : INotifyPropertyChanged
	{
		/// <summary>
		/// Default DPI
		/// </summary>
		public static readonly Dpi Default = new Dpi(96, 96);

		/// <summary>
		/// X-axis value of DPI
		/// </summary>
		public uint X
		{
			get { return _x; }
			set
			{
				_x = value;
				RaisePropertyChanged();
			}
		}
		private uint _x;

		/// <summary>
		/// Y-axis value of DPI
		/// </summary>
		public uint Y
		{
			get { return _y; }
			set
			{
				_y = value;
				RaisePropertyChanged();
			}
		}
		private uint _y;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="x">X-axis value</param>
		/// <param name="y">Y-axis value</param>
		public Dpi(uint x, uint y)
			: this()
		{
			this.X = x;
			this.Y = y;
		}

		/// <summary>
		/// == Operator
		/// </summary>
		/// <param name="dpi1">Instance to compare</param>
		/// <param name="dpi2">Instance to compare</param>
		/// <returns>True if equal</returns>
		public static bool operator ==(Dpi dpi1, Dpi dpi2)
		{
			return (dpi1.X == dpi2.X) && (dpi1.Y == dpi2.Y);
		}

		/// <summary>
		/// != Operator
		/// </summary>
		/// <param name="dpi1">Instance to compare</param>
		/// <param name="dpi2">Instance to compare</param>
		/// <returns>True if not equal</returns>
		public static bool operator !=(Dpi dpi1, Dpi dpi2)
		{
			return !(dpi1 == dpi2);
		}

		/// <summary>
		/// Equals operator
		/// </summary>
		/// <param name="other">Other instance to compare</param>
		/// <returns>True if equal</returns>
		public bool Equals(Dpi other)
		{
			return (this == other);
		}

		/// <summary>
		/// Equals operator
		/// </summary>
		/// <param name="other">Other instance to compare</param>
		/// <returns>True if equal</returns>
		public override bool Equals(object other)
		{
			if (ReferenceEquals(null, other))
				return false;

			return (other is Dpi) && (this == (Dpi)other);
		}

		/// <summary>
		/// Get hash code.
		/// </summary>
		/// <returns>Hash code for this structure</returns>
		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode();
		}

		/// <summary>
		/// Create string representation.
		/// </summary>
		/// <returns>String containing X and Y values of this structure</returns>
		public override string ToString() => $"{this.X}-{this.Y}";

		#region INotifyPropertyChanged member

		/// <summary>
		/// PropertyChanged event
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}