using System;

namespace ProductX.Framework.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Checks if string contains substring.
		/// </summary>
		/// <param name="originalString">String to check.</param>
		/// <param name="value"> Substring to compare with.</param>
		/// <param name="comparisonType">Comparison options.</param>
		/// <returns></returns>
		public static bool Contains(this string originalString, string value, StringComparison comparisonType) =>
			originalString.IndexOf(value, comparisonType) >= 0;
	}
}
