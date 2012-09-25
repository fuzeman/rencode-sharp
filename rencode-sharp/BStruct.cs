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
		/// <summary>
		/// Pack the object 'x' into (network order byte format).
		/// </summary>
		public static string Pack(object x, int n)
		{
			byte[] b = ToBytes(x);

			string output = "";
			for(int i = 0; i < b.Length; i++)
			{
				output += (char)b[i];
			}

			return output.Substring(0, n);
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
		/// Convert object 'x' to a byte array.
		/// </summary>
		public static byte[] ToBytes(object x)
		{
			byte[] b;

			if(x.GetType() == typeof(uint))
				return BitConverter.GetBytes((uint)x);
			else if(x.GetType() == typeof(ushort))
				return BitConverter.GetBytes((ushort)x);
			else if(x.GetType() == typeof(ulong))
				return BitConverter.GetBytes((ulong)x);
			else if(x.GetType() == typeof(double))
				return BitConverter.GetBytes((double)x);
			else if(x.GetType() == typeof(float))
				return BitConverter.GetBytes((float)x);
			else if(x.GetType() == typeof(char))
				return BitConverter.GetBytes((char)x);
			else if(x.GetType() == typeof(bool))
				return BitConverter.GetBytes((bool)x);
			else if(x.GetType() == typeof(short)) {
				b = new byte[2];
				BStruct.GetBytes((short)x, b, 0);
				return b;
			}
			else if(x.GetType() == typeof(long)) {
				b = new byte[8];
				BStruct.GetBytes((long)x, b, 0);
				return b;
			}
			else if(x.GetType() == typeof(int)) {
				b = new byte[4];
				BStruct.GetBytes((int)x, b, 0);
				return b;
			}
			else {
				Console.WriteLine("pack unsupported");
				return null;
			}
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

		/// <summary>
		/// Converts byte array to INT2 (16 bit integer)
		/// </summary>
		public static int ToInt2(byte[] value, int startIndex)
		{
			return BitConverter.ToInt16(value, startIndex);
		}

		/// <summary>
		/// Converts byte array to INT4 (32 bit integer)
		/// </summary>
		public static int ToInt4(byte[] value, int startIndex)
		{
			if(value.Length == 4)
				return BitConverter.ToInt32(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 4 bytes.");
		}

		/// <summary>
		/// Converts byte array to INT8 (64 bit integer)
		/// </summary>
		public static long ToInt8(byte[] value, int startIndex)
		{
			if(value.Length == 8)
				return BitConverter.ToInt64(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 8 bytes.");
		}

		/// <summary>
		/// Converts byte array to Float (32 bit float)
		/// </summary>
		public static float ToFloat(byte[] value, int startIndex)
		{
			if(value.Length == 4)
				return BitConverter.ToSingle(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 4 bytes.");
		}

		/// <summary>
		/// Converts byte array to Double (64 bit float)
		/// </summary>
		public static double ToDouble(byte[] value, int startIndex)
		{
			if(value.Length == 8)
				return BitConverter.ToDouble(value, startIndex);
			else
				throw new ArgumentException("\"value\" doesn't have 8 bytes.");
		}
	}
}

