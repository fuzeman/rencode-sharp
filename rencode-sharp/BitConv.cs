using System;

namespace rencodesharp
{
	public class BitConv
	{
		/// <summary>
		/// Pack the specified object into network order byte format.
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

		public static object Unpack(string x, int n)
		{
			x = Pad(x, n);

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

		public static string Pad(string x, int n)
		{
			for(int i = x.Length; i < n; i++)
			{
				x = "\x00" + x;
			}
			return x;
		}
	}
}

