using System;
using System.Collections;
using System.Collections.Generic;

namespace rencodesharp
{
	public class Util
	{
		/// <summary>
		/// Join the List of objects by converting everything to a string.
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

		public static byte[] StringBytes(string s)
		{
			byte[] b = new byte[s.Length];
			for(int i = 0; i < s.Length; i++)
			{
				b[i] = (byte)(int)s[i];
			}
			return b;
		}
	}
}

