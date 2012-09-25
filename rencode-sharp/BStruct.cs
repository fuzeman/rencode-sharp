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

namespace rencodesharp
{
	public class BStruct
	{
		public unsafe static void GetBytes(Int32 value, byte[] buffer, int startIndex)
		{
			fixed(byte* numRef = buffer)
			{
				*((int*)(numRef+startIndex)) = value;
			}
		}

		public unsafe static void GetBytes(Int64 value, byte[] buffer, int startingIndex)
        {
            fixed(byte* numRef = buffer)
            {
                *((long*)(numRef + startingIndex)) = value;
            }
        }

		/// <summary>
		/// Converts byte array to INT1 (8 bit integer)
		/// </summary>
		public static int ToInt1(byte[] value, int startIndex)
		{
			if(value.Length == 4)
				return BitConverter.ToInt16(value, startIndex);
			else
			{
				byte[] newValue;

				if(value[0] >= 0 && value[0] < 128)
					newValue = new byte[4] { 0, 0, 0, 0 };
				else
					newValue = new byte[4] { 255, 255, 255, 255 };

				for(int i = 0; i < value.Length; i++)
					newValue[i] = value[i];

				return BitConverter.ToInt16(newValue, startIndex);
			}
		}

		public static int ToInt2(byte[] value, int startIndex)
		{
			return BitConverter.ToInt16(value, startIndex);
		}

		public static int ToInt4(byte[] value, int startIndex)
		{
			if(value.Length == 4)
				return BitConverter.ToInt32(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 4 bytes.");
		}

		public static long ToInt8(byte[] value, int startIndex)
		{
			if(value.Length == 8)
				return BitConverter.ToInt64(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 8 bytes.");
		}
	}
}

