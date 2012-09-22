using System;

namespace rencodesharp
{
	public class Util
	{
		/// <summary>
		/// Join the List of objects by converting everything to a string.
		/// </summary>
		/// <param name='r'>
		/// Objects to Join
		/// </param>
		public static string Join(List<object> r)
		{
			string output = "";
			for(int i = 0; i < r.Count; i++)
			{
				output += r[i].ToString();
			}
			return output;
		}
	}
}

