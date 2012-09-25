// rencode-sharp -- Web safe object pickling/unpickling.

// Copyright (C) 2012 Dean Gardiner <gardiner91@gmail.com>
// C# port of rencode.py: Public domain, Connelly Barnes 2006-2007.
// Rencode, Modified version of bencode from the BitTorrent project.

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

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

