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

