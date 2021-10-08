﻿using System;
using System.Windows.Media;

using static System.Math;

namespace WpfExtendedWindow.Helper
{
	/// <summary>
	/// Extension methods for <see cref="System.Windows.Media.Color"/>
	/// </summary>
	internal static class ColorExtension
	{
		/// <summary>
		/// Converts a Color to an opaque Color using white background.
		/// </summary>
		/// <param name="source">Source Color</param>
		/// <returns>Opaque Color</returns>
		public static Color ToOpaque(this Color source)
		{
			if (source.A == (byte)255)
				return source;

			var perc = (double)(255 - source.A) * 100D / 255D;

			return BlendColor(source, Colors.White, perc);
		}

		/// <summary>
		/// Converts two Colors to a blended Color ignoring alpha channel.
		/// </summary>
		/// <param name="source">Source Color</param>
		/// <param name="target">Target Color</param>
		/// <param name="targetPerc">Percentage of target Color</param>
		/// <returns>Blended Color</returns>
		/// <remarks>Alpha channels of both Colors will be ignored.</remarks>
		public static Color ToBlended(this Color source, Color target, double targetPerc)
		{
			return BlendColor(source, target, targetPerc);
		}

		#region Helper

		/// <summary>
		/// Blends two Colors.
		/// </summary>
		/// <param name="color1">1st Color</param>
		/// <param name="color2">2nd Color</param>
		/// <param name="color2Perc">Percentage of 2nd Color</param>
		/// <returns>Blended Color</returns>
		/// <remarks>Alpha channels of both Colors will be ignored.</remarks>
		private static Color BlendColor(Color color1, Color color2, double color2Perc)
		{
			if ((color2Perc < 0) || (100 < color2Perc))
				throw new ArgumentOutOfRangeException(nameof(color2Perc));

			return Color.FromRgb(
				BlendColorChannel(color1.R, color2.R, color2Perc),
				BlendColorChannel(color1.G, color2.G, color2Perc),
				BlendColorChannel(color1.B, color2.B, color2Perc));
		}

		private static byte BlendColorChannel(double channel1, double channel2, double channel2Perc)
		{
			var buff = channel1 + (channel2 - channel1) * channel2Perc / 100D;
			return Min((byte)Round(buff), (byte)255); // Casting to byte does Math.Floor.
		}

		#endregion
	}
}