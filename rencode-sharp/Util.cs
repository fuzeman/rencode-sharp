using System;
using System.Collections;
using System.Collections.Generic;

namespace rencodesharp
{
	public class Util
	{
		/// <summary>
		/// Join the List of objects by converting each item to a string.
		/// </summary>
		public static string Join(List<object> r)
		{
			string output = "";
			for(int i = 0; i < r.Count; i++)
			{
				output += r[i].ToString();
			}
			return output;
		}

		/// <summary>
		/// Converts string to byte array.
		/// </summary>
		public static byte[] StringBytes(string s)
		{
			byte[] b = new byte[s.Length];
			for(int i = 0; i < s.Length; i++)
			{
				b[i] = (byte)(int)s[i];
			}
			return b;
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

