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
using MiscUtil.Conversion;

namespace rencodesharp
{
	public class BStruct
	{
		/// <summary>
		/// Pack the object 'x' into (network order byte format).
		/// </summary>
		public static string Pack(object x, int n)
		{
			byte[] b = EndianBitConverter.Big.GetBytes(x);

			string output = "";
			for(int i = 0; i < b.Length; i++)
			{
				output += (char)b[i];
			}

			return output.Substring(output.Length - n, n);
		}

		/// <summary>
		/// Unpack the string 'x' (network order byte format) into object.
		/// </summary>
		public static object Unpack(string x, int n)
		{
			x = Util.StringPad(x, n);

			byte[] b = new byte[n];
			for(int i = 0; i < x.Length; i++)
			{
				b[i] = (byte)x[i];
			}

			if(b.Length == 1) return BStruct.ToInt1(b, 0);
			if(b.Length == 2) return BStruct.ToInt2(b, 0);
			if(b.Length == 4) return BStruct.ToInt4(b, 0);
			if(b.Length == 8) return BStruct.ToInt8(b, 0);
			return null;
		}

		/// <summary>
		/// Gets the bytes of an Int32.
		/// </summary>
		public unsafe static void GetBytes(Int32 value, byte[] buffer, int startIndex)
		{
			fixed(byte* numRef = buffer)
			{
				*((int*)(numRef+startIndex)) = value;
			}
		}

		/// <summary>
		/// Gets the bytes of an Int64.
		/// </summary>
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
				return EndianBitConverter.Big.ToInt16(value, startIndex);
			else
			{
				byte[] newValue;

				if(value[0] >= 0 && value[0] < 128)
					newValue = new byte[2] { 0, 0 };
				else
					newValue = new byte[2] { 255, 255 };

				int ni = newValue.Length - 1;
				for(int i = value.Length - 1; i >= 0; i--)
				{
					newValue[ni] = value[i];
					ni--;
				}

				return EndianBitConverter.Big.ToInt16(newValue, startIndex);
			}
		}

		/// <summary>
		/// Converts byte array to INT2 (16 bit integer)
		/// </summary>
		public static int ToInt2(byte[] value, int startIndex)
		{
			if(value.Length == 2)
				return EndianBitConverter.Big.ToInt16(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 2 bytes.");
		}

		/// <summary>
		/// Converts byte array to INT4 (32 bit integer)
		/// </summary>
		public static int ToInt4(byte[] value, int startIndex)
		{
			if(value.Length == 4)
				return EndianBitConverter.Big.ToInt32(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 4 bytes.");
		}

		/// <summary>
		/// Converts byte array to INT8 (64 bit integer)
		/// </summary>
		public static long ToInt8(byte[] value, int startIndex)
		{
			if(value.Length == 8)
				return EndianBitConverter.Big.ToInt64(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 8 bytes.");
		}

		/// <summary>
		/// Converts byte array to Float (32 bit float)
		/// </summary>
		public static float ToFloat(byte[] value, int startIndex)
		{
			if(value.Length == 4)
				return EndianBitConverter.Big.ToSingle(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 4 bytes.");
		}

		/// <summary>
		/// Converts byte array to Double (64 bit float)
		/// </summary>
		public static double ToDouble(byte[] value, int startIndex)
		{
			if(value.Length == 8)
				return EndianBitConverter.Big.ToDouble(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 8 bytes.");
		}
	}
}

