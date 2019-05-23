using System;
using System.Linq;

namespace ProductX.Framework.Helpers
{
	public static class RandomHelper
	{
		private const string WithoutSpacesPattern = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private const string DatePattern = @"yyyyMMddHHmmss";
		private static readonly Random Random = new Random();

		/// <summary>
		/// Makes the random string.
		/// </summary>
		/// <param name="length">The length.</param>
		/// <param name="pattern">Pattern of possible symbols.</param>
		/// <returns>String.</returns>
		public static string GetRandomString(int length, string pattern = WithoutSpacesPattern) =>
			new string(Enumerable.Repeat(pattern, length).Select(s => s[Random.Next(s.Length)]).ToArray());

		/// <summary>
		/// Makes random string with date postfix.
		/// </summary>
		/// <param name="length">The length.</param>
		/// <param name="pattern">Pattern of possible symbols.</param>
		/// <returns>String.</returns>
		public static string GetRandomStringWithDatePostfix(int length, string pattern = WithoutSpacesPattern) =>
			$"{new string(Enumerable.Repeat(pattern, length).Select(s => s[Random.Next(s.Length)]).ToArray())}_{DateTime.UtcNow.ToString(DatePattern)}";

		/// <summary>
		/// Returns random number from 0 to range.
		/// </summary>
		/// <param name="range">Range of possible upper limit.</param>
		/// <returns>int.</returns>
		public static int GetRandomNumber(int range) => Random.Next(0, range);
	}
}