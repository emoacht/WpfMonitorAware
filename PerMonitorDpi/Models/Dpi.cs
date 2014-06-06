using System;

namespace PerMonitorDpi.Models
{
	/// <summary>
	/// DPI information
	/// </summary>
	/// <remarks>
	/// This struct is based on the same struct of https://github.com/Grabacr07/XamClaudia
	/// </remarks>
	public struct Dpi
	{
		public static readonly Dpi Default = new Dpi(96, 96);

		public uint X { get; set; }
		public uint Y { get; set; }

		public Dpi(uint x, uint y)
			: this()
		{
			this.X = x;
			this.Y = y;
		}

		public static bool operator ==(Dpi dpi1, Dpi dpi2)
		{
			return (dpi1.X == dpi2.X) && (dpi1.Y == dpi2.Y);
		}

		public static bool operator !=(Dpi dpi1, Dpi dpi2)
		{
			return !(dpi1 == dpi2);
		}

		public bool Equals(Dpi other)
		{
			return (this.X == other.X) && (this.Y == other.Y);
		}

		public override bool Equals(object other)
		{
			if (ReferenceEquals(null, other))
				return false;

			return (other is Dpi) && Equals((Dpi)other);
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{0}-{1}", this.X, this.Y);
		}
	}
}
