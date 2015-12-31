using System.Collections.Generic;
using System.Text;

namespace rencodesharp
{
	public class Util
	{
		/// <summary>
		/// Join the List of objects by converting each item to a string.
		/// </summary>
		public static string Join(List<object> r)
		{
		    return string.Concat(r.ToArray());
		}

		/// <summary>
		/// Converts string to byte array.
		/// </summary>
		public static byte[] StringBytes(string s)
		{
		    return Encoding.UTF8.GetBytes(s);
		}

		/// <summary>
		/// Pads the front of a string with NUL bytes.
		/// </summary>
		public static string StringPad(string x, int n)
		{
			for(int i = x.Length; i < n; i++)
			{
				x = "\x00" + x;
			}
			return x;
		}
	}
}

