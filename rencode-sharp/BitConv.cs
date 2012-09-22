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
			byte[] b = new byte[] {};

			if(x.GetType() == typeof(uint))
				b = BitConverter.GetBytes((uint)x);
			else if(x.GetType() == typeof(ushort))
				b = BitConverter.GetBytes((ushort)x);
			else if(x.GetType() == typeof(ulong))
				b = BitConverter.GetBytes((ulong)x);
			else if(x.GetType() == typeof(double))
				b = BitConverter.GetBytes((double)x);
			else if(x.GetType() == typeof(float))
				b = BitConverter.GetBytes((float)x);
			else if(x.GetType() == typeof(char))
				b = BitConverter.GetBytes((char)x);
			else if(x.GetType() == typeof(bool))
				b = BitConverter.GetBytes((bool)x);
			else if(x.GetType() == typeof(short))
				b = BitConverter.GetBytes((short)x);
			else if(x.GetType() == typeof(long))
				b = BitConverter.GetBytes((long)x);
			else if(x.GetType() == typeof(int))
				b = BitConverter.GetBytes((int)x);
			else {
				Console.WriteLine("pack unsupported");
				return null;
			}

			string output = "";
			for(int i = b.Length - 1; i >= 0; i--)
			{
				output += (char)b[i];
			}

			return output.Substring(output.Length - n, n);
		}
	}
}

