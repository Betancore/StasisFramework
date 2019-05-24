using System;
using System.ComponentModel;

namespace Stasis.Extensions
{
	public static class EnumExtensions
	{
		/// <summary>
		/// Gets string value of enum instance set in Description.
		/// </summary>
		/// <param name="value">Enum value.</param>
		/// <returns>String value.</returns>
		public static string GetValue(this Enum value)
		{
			var attribute = value.GetAttribute<DescriptionAttribute>();

			return attribute?.Description ?? value.ToString();
		}

		private static T GetAttribute<T>(this Enum value) where T : Attribute
		{
			var type = value.GetType();
			var memberInfo = type.GetMember(value.ToString());
			var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

			return (T)attributes[0];
		}
	}
}