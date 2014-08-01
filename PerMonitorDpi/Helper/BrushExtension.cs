﻿using System;
using System.Windows.Media;

namespace PerMonitorDpi.Helper
{
	public static class BrushExtenstion
	{
		/// <summary>
		/// Check if a brush is Transparent.
		/// </summary>
		/// <param name="source">Source brush</param>
		/// <returns>True if Transparent</returns>
		public static bool IsTransparent(this Brush source)
		{
			var solid = source as SolidColorBrush;
			if (solid == null)
				return false;

			return (solid.Color == Colors.Transparent);
		}
	}
}