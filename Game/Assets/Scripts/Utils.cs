using System;

/// <summary>
/// Utilities class
/// </summary>
public static class Util {

	/// <summary>
	/// Gets the next value in an enum.
	/// </summary>
	/// <param name="src">The current enum value.</param>
	/// <typeparam name="T">The enum type.</typeparam>
	public static T Next<T>(this T src) where T : struct {
		if (!typeof(T).IsEnum) {
			throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));
		}
		T[] Arr = (T[]) Enum.GetValues(src.GetType());
		int j = Array.IndexOf<T>(Arr, src) + 1;
		if (j == Arr.Length) {
			j = 0;
		}
		return (Arr.Length == j) ? Arr[0] : Arr[j];
	}
}
