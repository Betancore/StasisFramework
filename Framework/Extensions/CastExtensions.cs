using System;

namespace Stasis.Extensions
{
	public static class CastExtensions
	{
		/// <summary>
		/// Casts any object to another type.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="object">Object to cast.</param>
		/// <returns></returns>
		public static T CastTo<T>(this object @object) => (T)Convert.ChangeType(@object, typeof(T));
	}
}
