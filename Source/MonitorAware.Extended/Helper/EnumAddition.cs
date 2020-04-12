using System;
using System.Linq;

namespace MonitorAware.Extended.Helper
{
	/// <summary>
	/// Additional method for <see cref="Enum"/>
	/// </summary>
	internal static class EnumAddition
	{
		/// <summary>
		/// Checks if a specified string exists in the names of a specified Enum.
		/// </summary>
		/// <param name="enumType">Enum type</param>
		/// <param name="value">Source string</param>
		/// <param name="comparisonType">StringComparison option</param>
		/// <returns>True if exists</returns>
		/// <remarks>This method acts as Enum.IsDefined method added with StringComparison option.</remarks>
		public static bool IsDefined(Type enumType, string value, StringComparison comparisonType)
		{
			if (enumType == null)
				throw new ArgumentNullException(nameof(enumType));

			if (!enumType.IsEnum)
				throw new ArgumentException("The type must be Enum.", nameof(enumType));

			if (string.IsNullOrWhiteSpace(value))
				return false;

			return Enum.GetNames(enumType).Any(x => string.Compare(x, value, comparisonType) == 0);
		}

		/// <summary>
		/// Converts a specified string to the equivalent object of a specified Enum.
		/// </summary>
		/// <param name="enumType">Enum type</param>
		/// <param name="value">Source string</param>
		/// <param name="comparisonType">StringComparison option</param>
		/// <returns>Result object of Enum</returns>
		/// <remarks>This method acts as Enum.Parse method added with StringComparison option.</remarks>
		public static object Parse(Type enumType, string value, StringComparison comparisonType)
		{
			if (enumType == null)
				throw new ArgumentNullException(nameof(enumType));

			if (!enumType.IsEnum)
				throw new ArgumentException("The type must be Enum.", nameof(enumType));

			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentNullException(nameof(value));

			var name = Enum.GetNames(enumType).FirstOrDefault(x => string.Compare(x, value, comparisonType) == 0);
			if (name == null)
				throw new ArgumentException("The value must be included in Enum names.", nameof(value));

			return Enum.Parse(enumType, name);
		}

		/// <summary>
		/// Converts a specified string to the equivalent object of a specified Enum.
		/// </summary>
		/// <typeparam name="TEnum">Enum type</typeparam>
		/// <param name="value">Source string</param>
		/// <param name="result">Result object of Enum</param>
		/// <param name="comparisonType">StringComparison option</param>
		/// <returns>True if converted successfully</returns>
		/// <remarks>This method acts as Enum.TryParse method added with StringComparison option.</remarks>
		public static bool TryParse<TEnum>(string value, out TEnum result, StringComparison comparisonType)
		{
			if (!typeof(TEnum).IsEnum)
				throw new ArgumentException("The type must be Enum.", nameof(TEnum));

			if (string.IsNullOrWhiteSpace(value))
			{
				result = default(TEnum);
				return false;
			}

			var name = Enum.GetNames(typeof(TEnum)).FirstOrDefault(x => string.Compare(x, value, comparisonType) == 0);
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