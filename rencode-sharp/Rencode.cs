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

		// Lists with length embedded in typecode.
		public const int LIST_FIXED_START = STR_FIXED_START+STR_FIXED_COUNT;
		public const int LIST_FIXED_COUNT = 64;

		// Dictionaries with length embedded in typecode.
		public const int DICT_FIXED_START = 102;
		public const int DICT_FIXED_COUNT = 25;

		public delegate void EncodeDelegate(object x, List<object> r);
		public delegate object DecodeDelegate(string x, int f, out int endIndex);

		private static Dictionary<Type, EncodeDelegate> encode_func = new Dictionary<Type, EncodeDelegate>(){
			{typeof(string),						encode_string},
			{typeof(int),							encode_int},
			{typeof(long),							encode_int},
			{typeof(object[]),						encode_list},
			{typeof(Dictionary<object, object>),	encode_dict}
		};

		private static Dictionary<char, DecodeDelegate> decode_func = new Dictionary<char, DecodeDelegate>(){
			{'0', 		decode_string},
			{'1', 		decode_string},
			{'2', 		decode_string},
			{'3', 		decode_string},
			{'4', 		decode_string},
			{'5', 		decode_string},
			{'6', 		decode_string},
			{'7', 		decode_string},
			{'8', 		decode_string},
			{'9', 		decode_string},
			{CHR_INT,	decode_int},
			{CHR_INT1,	decode_int1},
			{CHR_INT2,	decode_int2},
			{CHR_INT4,	decode_int4},
			{CHR_INT8,	decode_int8},
			{CHR_LIST,	decode_list},
			{CHR_DICT,	decode_dict},
		};

		#region Initialization

		static Rencode()
		{
			for(int i = 0; i < STR_FIXED_COUNT; i++) {
				decode_func.Add((char)(STR_FIXED_START + i), decode_fixed_string);
			}

			for(int i = 0; i < LIST_FIXED_COUNT; i++) {
				decode_func.Add((char)(LIST_FIXED_START + i), decode_fixed_list);
			}

			for(int i = 0; i < INT_POS_FIXED_COUNT; i++) {
				decode_func.Add((char)(INT_POS_FIXED_START + i), decode_fixed_pos_int);
			}

			for(int i = 0; i < INT_NEG_FIXED_COUNT; i++) {
				decode_func.Add((char)(INT_NEG_FIXED_START + i), decode_fixed_neg_int);
			}

			for(int i = 0; i < DICT_FIXED_COUNT; i++) {
				decode_func.Add((char)(DICT_FIXED_START + i), decode_fixed_dict);
			}
		}

		private static object decode_fixed_string(string x, int f, out int endIndex)
		{
			endIndex = f + 1 + ((int)x[f]) - STR_FIXED_START;
			return x.Substring(f + 1, ((int)x[f]) - STR_FIXED_START);
		}

		private static object decode_fixed_pos_int(string x, int f, out int endIndex)
		{
			endIndex = f + 1;
			return ((int)x[f]) - INT_POS_FIXED_START;
		}


		private static object decode_fixed_neg_int(string x, int f, out int endIndex)
		{
			endIndex = f + 1;
			return -1 - (((int)x[f]) - INT_NEG_FIXED_START);
		}

		private static object decode_fixed_list(string x, int f, out int endIndex)
		{
			List<object> r = new List<object>();
			int list_count = ((int)x[f]) - LIST_FIXED_START;
			f = f + 1;

			for(int i = 0; i < list_count; i++)
			{
				object v = decode_func[x[f]](x, f, out f);
				r.Add(v);
			}

			endIndex = f;
			return r.ToArray();
		}

		private static object decode_fixed_dict(string x, int f, out int endIndex)
		{
			Dictionary<object, object> r = new Dictionary<object, object>();
			int dict_count = ((int)x[f]) - DICT_FIXED_START;
			f = f + 1;

			for(int i = 0; i < dict_count; i++)
			{
				object k = decode_func[x[f]](x, f, out f);
				object v = decode_func[x[f]](x, f, out f);
				r.Add(k ,v);
			}

			endIndex = f;
			return r;
		}

		#endregion

		#region Core

		public static string dumps(object x) { return dumps(x, 32); }
		public static string dumps(object x, int float_bits)
		{
			if(!encode_func.ContainsKey(x.GetType())){
				Console.WriteLine("No Encoder Found");
				return null;
			}

			List<object> r = new List<object>();
			encode_func[x.GetType()](x, r);

			return Util.Join(r);
		}

		public static object loads(string x)
		{
			if(!decode_func.ContainsKey(x[0])){
				Console.WriteLine("No Decoder Found");
				return null;
			}

			int endIndex;
			return decode_func[x[0]](x, 0, out endIndex);
		}

		#endregion

		#region Encode

		public static void encode_string(object x, List<object> r)
		{
			string xs = (string)x;

			if (xs.Length < STR_FIXED_COUNT) {
				r.Add((char)(STR_FIXED_START + xs.Length));
				r.Add(xs);
			} else {
				r.Add(xs.Length.ToString());
				r.Add(':');
				r.Add(xs);
			}
		}

		public static void encode_int(object x, List<object> r)
		{
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
		}

		public static void encode_list(object x, List<object> r)
		{
			if(x.GetType() != typeof(object[])) throw new Exception();
			object[] xl = (object[])x;

			if(xl.Length < LIST_FIXED_COUNT) {
				r.Add((char)(LIST_FIXED_START + xl.Length));
				foreach(object e in xl)
					encode_func[e.GetType()](e, r);
			} else {
				r.Add(CHR_LIST);
				foreach(object e in xl)
					encode_func[e.GetType()](e, r);
				r.Add(CHR_TERM);
			}
		}

		public static void encode_dict(object x, List<object> r)
		{
			if(x.GetType() != typeof(Dictionary<object, object>)) throw new Exception();
			Dictionary<object, object> xd = (Dictionary<object, object>)x;

			if(xd.Count < DICT_FIXED_COUNT)
			{
				r.Add((char)(DICT_FIXED_START + xd.Count));
				foreach(KeyValuePair<object, object> kv in xd)
				{
					encode_func[kv.Key.GetType()](kv.Key, r);
					encode_func[kv.Value.GetType()](kv.Value, r);
				}
			} else {
				r.Add((char)CHR_DICT);
				foreach(KeyValuePair<object, object> kv in xd)
				{
					encode_func[kv.Key.GetType()](kv.Key, r);
					encode_func[kv.Value.GetType()](kv.Value, r);
				}
				r.Add((char)CHR_TERM);
			}
		}

		#endregion

		#region Decode

		public static string decode_string(string x, int f, out int endIndex)
		{
			int colon = x.IndexOf(':', f);
			int n = Convert.ToInt32(x.Substring(f, colon)); // TODO: doesn't support long length

			colon += 1;
			string s = x.Substring(colon, (int)n);

			endIndex = colon+n;
			return s;
		}

		public static object decode_int(string x, int f, out int endIndex)
		{
			f += 1;
			int newf = x.IndexOf(CHR_TERM, f);
			if(newf - f >= MAX_INT_LENGTH) {
				throw new Exception("Overflow");
			}
			long n = Convert.ToInt64(x.Substring(f, newf - f));

			if(x[f] == '-'){
				if(x[f + 1] == '0') {
					throw new Exception("Value Error");
				}
			}
			else if(x[f] == '0' && newf != f+1) {
				throw new Exception("Value Error");
			}

			endIndex = newf+1;
			return n;
		}

		public static object decode_int1(string x, int f, out int endIndex)
		{
			f += 1;
			endIndex = f + 1;

			return BStruct.ToInt1(Util.StringBytes(x.Substring(f, 1)), 0);
		}

		public static object decode_int2(string x, int f, out int endIndex)
		{
			f += 1;
			endIndex = f + 2;

			return BStruct.ToInt2(Util.StringBytes(x.Substring(f, 2)), 0);
		}

		public static object decode_int4(string x, int f, out int endIndex)
		{
			f += 1;
			endIndex = f + 4;

			return BStruct.ToInt4(Util.StringBytes(x.Substring(f, 4)), 0);
		}

		public static object decode_int8(string x, int f, out int endIndex)
		{
			f += 1;
			endIndex = f + 8;

			return BStruct.ToInt8(Util.StringBytes(x.Substring(f, 8)), 0);
		}

		public static object decode_list(string x, int f, out int endIndex)
		{
			List<object> r = new List<object>();
			f = f + 1;
			while(x[f] != CHR_TERM)
			{
				object v = decode_func[x[f]](x, f, out f);
				r.Add(v);
			}
			endIndex = f + 1;
			return r.ToArray();
		}

		public static object decode_dict(string x, int f, out int endIndex)
		{
			Dictionary<object, object> r = new Dictionary<object, object>();
			f = f + 1;
			while(x[f] != CHR_TERM)
			{
				object k = decode_func[x[f]](x, f, out f);
				object v = decode_func[x[f]](x, f, out f);
				r.Add(k, v);
			}

			endIndex = f + 1;
			return r;
		}

		#endregion
	}
}

