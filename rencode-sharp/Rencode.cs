using System;
using System.Collections;
using System.Collections.Generic;

namespace rencodesharp
{
	public class Rencode
	{
		// The bencode 'typecodes' such as i, d, etc have been extended and
		// relocated on the base-256 character set.
		public const char CHR_LIST		= (char)59;
		public const char CHR_DICT		= (char)60;
		public const char CHR_INT		= (char)61;
		public const char CHR_INT1		= (char)62;
		public const char CHR_INT2		= (char)63;
		public const char CHR_INT4		= (char)64;
		public const char CHR_INT8		= (char)65;
		public const char CHR_FLOAT32	= (char)66;
		public const char CHR_FLOAT64	= (char)44;
		public const char CHR_TRUE		= (char)67;
		public const char CHR_FALSE		= (char)68;
		public const char CHR_NONE		= (char)69;
		public const char CHR_TERM		= (char)127;

		// Maximum length of integer when written as base 10 string.
		public const int MAX_INT_LENGTH = 64;

		// Positive integers with value embedded in typecode.
		public const int INT_POS_FIXED_START = 0;
		public const int INT_POS_FIXED_COUNT = 44;

		// Negative integers with valuve embedded in typecode.
		public const int INT_NEG_FIXED_START = 70;
		public const int INT_NEG_FIXED_COUNT = 32;

		// Strings with length embedded in typecode.
		public const int STR_FIXED_START = 128;
		public const int STR_FIXED_COUNT = 64;

		public delegate string EncodeDelegate(object x);

		private static Dictionary<Type, EncodeDelegate> encode_func = new Dictionary<Type, EncodeDelegate>(){
			{typeof(string),	encode_string},
			{typeof(int),		encode_int},
			{typeof(long),		encode_int}
		};

		#region Core

		public static string dumps(object x) { return dumps(x, 32); }
		/// <summary>
		/// Dump data structure to str.
		/// </summary>
		public static string dumps(object x, int float_bits)
		{
			if(!encode_func.ContainsKey(x.GetType())) return null;

			return encode_func[x.GetType()](x);
		}

		#endregion

		#region Encode

		public static string encode_string(object x)
		{
			List<object> r = new List<object>();
			Console.WriteLine("encode_string");

			string xs = (string)x;

			if (xs.Length < STR_FIXED_COUNT) {
				r.Add((char)(STR_FIXED_START + xs.Length));
				r.Add(xs);
			} else {
				r.Add(xs.Length.ToString());
				r.Add(':');
				r.Add(xs);
			}

			return Util.Join(r);
		}

		public static string encode_int(object x)
		{
			List<object> r = new List<object>();
			Console.WriteLine("encode_int");

			// Check to determine if long type is able
			// to be packed inside an Int32 or is actually
			// an Int64 value.
			bool isLong = false;
			if(x.GetType() == typeof(long) &&(
				((long)x) > int.MaxValue ||
				((long)x) < int.MinValue))
					isLong = true;


			if(!isLong && 0 <= (int)x && (int)x < INT_POS_FIXED_COUNT) {
				r.Add((char)(INT_POS_FIXED_START+(int)x));
			} else if(!isLong && -INT_NEG_FIXED_COUNT <= (int)x && (int)x < 0) {
				r.Add((char)(INT_NEG_FIXED_START-1-(int)x));
			} else if(!isLong && -128 <= (int)x && (int)x < 128) {
				r.Add(CHR_INT1);
				r.Add(BitConv.Pack(x, 1));
			} else if(!isLong && -32768 <= (int)x && (int)x < 32768) {
				r.Add(CHR_INT2);
				r.Add(BitConv.Pack(x, 2));
			} else if(-2147483648L <= Convert.ToInt64(x) && Convert.ToInt64(x) < 2147483648L) {
				r.Add(CHR_INT4);
				r.Add(BitConv.Pack(x, 4));
			} else if(-9223372036854775808L < Convert.ToInt64(x) && Convert.ToInt64(x) < 9223372036854775807L) {
				r.Add(CHR_INT8);
				r.Add(BitConv.Pack(x, 8));
			} else {
				string s = (string)x;
				if(s.Length >= MAX_INT_LENGTH)
					throw new ArgumentOutOfRangeException();
				r.Add(CHR_INT);
				r.Add(s);
				r.Add(CHR_TERM);
			}

			return Util.Join(r);
		}

		#endregion

		#region Decode

		#endregion
	}
}

