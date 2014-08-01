﻿using System;
using System.Linq;

namespace PerMonitorDpi.Helper
{
	public static class EnumAddition
	{
		/// <summary>
		/// Check if a specified string exists in the names of a specified Enum.
		/// </summary>
		/// <param name="enumType">Enum type</param>
		/// <param name="value">Source string</param>
		/// <param name="comparisonType">StringComparison option</param>
		/// <returns>True if exists</returns>
		/// <remarks>This method acts as Enum.IsDefined method with StringComparison option.</remarks>
		public static bool IsDefined(Type enumType, string value, StringComparison comparisonType)
		{
			if ((enumType == null) || (!enumType.IsEnum))
				throw new ArgumentException("enumType");
			if (String.IsNullOrWhiteSpace(value))
				throw new ArgumentNullException("value");

			return Enum.GetNames(enumType).Any(x => String.Compare(x, value, comparisonType) == 0);
		}

		/// <summary>
		/// Convert a specified string to the equivalent object of a specified Enum.
		/// </summary>
		/// <param name="enumType">Enum type</param>
		/// <param name="value">Source string</param>
		/// <param name="comparisonType">StringComparison option</param>
		/// <returns>Result object of Enum</returns>
		/// <remarks>This method acts as Enum.Parse method with StringComparison option.</remarks>
		public static Object Parse(Type enumType, string value, StringComparison comparisonType)
		{
			if ((enumType == null) || (!enumType.IsEnum))
				throw new ArgumentException("enumType");
			if (String.IsNullOrWhiteSpace(value))
				throw new ArgumentNullException("value");

			var name = Enum.GetNames(enumType).FirstOrDefault(x => String.Compare(x, value, comparisonType) == 0);
			if (name == null)
				throw new ArgumentException("value");

			return Enum.Parse(enumType, name);
		}

		/// <summary>
		/// Convert a specified string to the equivalent object of a specified Enum.
		/// </summary>
		/// <typeparam name="TEnum">Enum type</typeparam>
		/// <param name="value">Source string</param>
		/// <param name="result">Result object of Enum</param>
		/// <param name="comparisonType">StringComparison option</param>
		/// <returns>True if converted successfully</returns>
		/// <remarks>This method acts as Enum.TryParse method with StringComparison option.</remarks>
		public static bool TryParse<TEnum>(string value, out TEnum result, StringComparison comparisonType)
		{
			if (!typeof(TEnum).IsEnum)
				throw new ArgumentException("TEnum");
			if (String.IsNullOrWhiteSpace(value))
				throw new ArgumentNullException("value");

			var name = Enum.GetNames(typeof(TEnum)).FirstOrDefault(x => String.Compare(x, value, comparisonType) == 0);
			if (name == null)
			{
				result = default(TEnum);
				return false;
			}
			else
			{
				result = (TEnum)Enum.Parse(typeof(TEnum), name);
				return true;
			}
		}
	}
}