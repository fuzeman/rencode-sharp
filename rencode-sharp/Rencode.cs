using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rencodesharp
{
	public class Rencode
	{
		private delegate void EncodeDelegate(object x, List<object> dest);
		private delegate object DecodeDelegate(string x, int startIndex, out int endIndex);

		private static readonly Dictionary<Type, EncodeDelegate> EncodeFunc = new Dictionary<Type, EncodeDelegate>(){
			{typeof(string),						EncodeString},

			{typeof(int),							EncodeInt},
			{typeof(long),							EncodeInt},
			{typeof(float),							EncodeFloat},
			{typeof(double),						EncodeDouble},

			{typeof(object[]),						EncodeList},
            {typeof(List<object>),                  EncodeList},

			{typeof(Dictionary<object, object>),	EncodeDictionary},

			{typeof(bool),							EncodeBool},
		};

		private static readonly Dictionary<char, DecodeDelegate> DecodeFunc = new Dictionary<char, DecodeDelegate>(){
			{'0', 		DecodeString},
			{'1', 		DecodeString},
			{'2', 		DecodeString},
			{'3', 		DecodeString},
			{'4', 		DecodeString},
			{'5', 		DecodeString},
			{'6', 		DecodeString},
			{'7', 		DecodeString},
			{'8', 		DecodeString},
			{'9', 		DecodeString},

			{RencodeConst.CHR_INT,	DecodeInt},
			{RencodeConst.CHR_INT1,	DecodeInt1},
			{RencodeConst.CHR_INT2,	DecodeInt2},
			{RencodeConst.CHR_INT4,	DecodeInt4},
			{RencodeConst.CHR_INT8,	DecodeInt8},
			{RencodeConst.CHR_FLOAT32,	DecodeFloat},
			{RencodeConst.CHR_FLOAT64,	DecodeDouble},

			{RencodeConst.CHR_LIST,	DecodeList},
			{RencodeConst.CHR_DICT,	DecodeDictionary},

			{RencodeConst.CHR_TRUE,	DecodeBoolTrue},
			{RencodeConst.CHR_FALSE,	DecodeBoolFalse},
			{RencodeConst.CHR_NONE,	DecodeNull},
		};

		private static readonly Encoding Encoding = Encoding.GetEncoding("iso-8859-1");

		#region Initialization

		static Rencode()
		{
			for(int i = 0; i < RencodeConst.STR_FIXED_COUNT; i++) {
				DecodeFunc.Add((char)(RencodeConst.STR_FIXED_START + i), DecodeFixedString);
			}

			for(int i = 0; i < RencodeConst.LIST_FIXED_COUNT; i++) {
				DecodeFunc.Add((char)(RencodeConst.LIST_FIXED_START + i), DecodeFixedList);
			}

			for(int i = 0; i < RencodeConst.INT_POS_FIXED_COUNT; i++) {
				DecodeFunc.Add((char)(RencodeConst.INT_POS_FIXED_START + i), DecodeFixedPositiveInt);
			}

			for(int i = 0; i < RencodeConst.INT_NEG_FIXED_COUNT; i++) {
				DecodeFunc.Add((char)(RencodeConst.INT_NEG_FIXED_START + i), DecodeFixedNegativeInt);
			}

			for(int i = 0; i < RencodeConst.DICT_FIXED_COUNT; i++) {
				DecodeFunc.Add((char)(RencodeConst.DICT_FIXED_START + i), DecodeFixedDictionary);
			}
		}

		private static object DecodeFixedString(string x, int startIndex, out int endIndex)
		{
			endIndex = startIndex + 1 + x[startIndex] - RencodeConst.STR_FIXED_START;
			return x.Substring(startIndex + 1, x[startIndex] - RencodeConst.STR_FIXED_START);
		}

		private static object DecodeFixedPositiveInt(string x, int startIndex, out int endIndex)
		{
			endIndex = startIndex + 1;
			return x[startIndex] - RencodeConst.INT_POS_FIXED_START;
		}

		private static object DecodeFixedNegativeInt(string x, int startIndex, out int endIndex)
		{
			endIndex = startIndex + 1;
			return -1 - (x[startIndex] - RencodeConst.INT_NEG_FIXED_START);
		}

		private static object DecodeFixedList(string x, int startIndex, out int endIndex)
		{
			var result = new List<object>();
			int listCount = x[startIndex] - RencodeConst.LIST_FIXED_START;
			startIndex = startIndex + 1;

			for(int i = 0; i < listCount; i++)
			{
				object v = DecodeObject(x, startIndex, out startIndex);
				result.Add(v);
			}

			endIndex = startIndex;
			return result.ToArray();
		}

		private static object DecodeFixedDictionary(string x, int startIndex, out int endIndex)
		{
			var result = new Dictionary<object, object>();
			int dictCount = x[startIndex] - RencodeConst.DICT_FIXED_START;
			startIndex = startIndex + 1;

			for(int i = 0; i < dictCount; i++)
			{
				object k = DecodeObject(x, startIndex, out startIndex);
				object v = DecodeObject(x, startIndex, out startIndex);
				result.Add(k ,v);
			}

			endIndex = startIndex;
			return result;
		}

		#endregion

		#region Core

		/// <summary>
		/// Encode object 'x' into a rencode string.
		/// </summary>
		public static string Encode(object x)
		{
			if(x != null && !EncodeFunc.ContainsKey(x.GetType())){
				throw new Exception("No Encoder Found");
			}

			var dest = new List<object>();
			EncodeObject(x, dest);

			return Util.Join(dest);
		}

		/// <summary>
		/// Decode rencode string 'x' into object.
		/// </summary>
		public static object Decode(string x)
		{
			if(!DecodeFunc.ContainsKey(x[0])){
				throw new Exception("No Decoder Found");
			}

			int endIndex;
			return DecodeObject(x, 0, out endIndex);
		}

		/// <summary>
		/// Decode rencode bytes 'x' into object.
		/// </summary>
		public static object Decode(byte[] x)
		{
			return Decode(Encoding.GetString(x));
		}

		#endregion

		#region Encode

		private static void EncodeObject(object x, List<object> dest)
		{
			if(x == null)
				EncodeNull(null, dest);
			else
				EncodeFunc[x.GetType()](x, dest);
		}

		private static void EncodeString(object x, List<object> dest)
		{
			var xs = (string)x;

			if (xs.Length < RencodeConst.STR_FIXED_COUNT) {
				dest.Add((char)(RencodeConst.STR_FIXED_START + xs.Length));
				dest.Add(xs);
			} else {
				dest.Add(xs.Length.ToString());
				dest.Add(':');
				dest.Add(xs);
			}
		}

		private static void EncodeInt(object x, List<object> dest)
		{
			// Check to determine if long type is able
			// to be packed inside an Int32 or is actually
			// an Int64 value.
			bool isLong = x is long && (((long)x) > int.MaxValue || ((long)x) < int.MinValue);

			if(!isLong && 0 <= (int)x && (int)x < RencodeConst.INT_POS_FIXED_COUNT) {
				dest.Add((char)(RencodeConst.INT_POS_FIXED_START+(int)x));
			} else if(!isLong && -RencodeConst.INT_NEG_FIXED_COUNT <= (int)x && (int)x < 0) {
				dest.Add((char)(RencodeConst.INT_NEG_FIXED_START-1-(int)x));
			} else if(!isLong && -128 <= (int)x && (int)x < 128) {
				dest.Add(RencodeConst.CHR_INT1);
				dest.Add(BStruct.Pack(x, 1));
			} else if(!isLong && -32768 <= (int)x && (int)x < 32768) {
				dest.Add(RencodeConst.CHR_INT2);
				dest.Add(BStruct.Pack(x, 2));
			} else if(-2147483648L <= Convert.ToInt64(x) && Convert.ToInt64(x) < 2147483648L) {
				dest.Add(RencodeConst.CHR_INT4);
				dest.Add(BStruct.Pack(x, 4));
			} else if(-9223372036854775808L < Convert.ToInt64(x) && Convert.ToInt64(x) < 9223372036854775807L) {
				dest.Add(RencodeConst.CHR_INT8);
				dest.Add(BStruct.Pack(x, 8));
			} else {
				string s = (string)x;
				if(s.Length >= RencodeConst.MAX_INT_LENGTH)
					throw new ArgumentOutOfRangeException();
				dest.Add(RencodeConst.CHR_INT);
				dest.Add(s);
				dest.Add(RencodeConst.CHR_TERM);
			}
		}

		private static void EncodeFloat(object x, List<object> dest)
		{
			dest.Add(RencodeConst.CHR_FLOAT32);
			dest.Add(BStruct.Pack(x, 4));
		}

		private static void EncodeDouble(object x, List<object> dest)
		{
			dest.Add(RencodeConst.CHR_FLOAT64);
			dest.Add(BStruct.Pack(x, 8));
		}

		private static void EncodeList(object x, List<object> dest)
		{
		    var listItems = x as IEnumerable<object>;
		    if(listItems == null)
                throw new Exception();

		    object[] xl = listItems.ToArray();

			if(xl.Length < RencodeConst.LIST_FIXED_COUNT) {
				dest.Add((char)(RencodeConst.LIST_FIXED_START + xl.Length));
				foreach(object e in xl)
					EncodeObject(e, dest);
			} else {
				dest.Add(RencodeConst.CHR_LIST);
				foreach(object e in xl)
					EncodeObject(e, dest);
				dest.Add(RencodeConst.CHR_TERM);
			}
		}

		private static void EncodeDictionary(object x, List<object> dest)
		{
			if(x.GetType() != typeof(Dictionary<object, object>)) throw new Exception();
			var xd = (Dictionary<object, object>)x;

			if(xd.Count < RencodeConst.DICT_FIXED_COUNT)
			{
				dest.Add((char)(RencodeConst.DICT_FIXED_START + xd.Count));
				foreach(KeyValuePair<object, object> kv in xd)
				{
					EncodeObject(kv.Key, dest);
					EncodeObject(kv.Value, dest);
				}
			} else {
				dest.Add(RencodeConst.CHR_DICT);
				foreach(KeyValuePair<object, object> kv in xd)
				{
					EncodeObject(kv.Key, dest);
					EncodeObject(kv.Value, dest);
				}
				dest.Add(RencodeConst.CHR_TERM);
			}
		}

		private static void EncodeBool(object x, List<object> dest)
		{
			if(x.GetType() != typeof(bool)) throw new Exception();
			var xb = (bool)x;

		    dest.Add(xb ? RencodeConst.CHR_TRUE : RencodeConst.CHR_FALSE);
		}

		private static void EncodeNull(object x, List<object> dest)
		{
			if(x != null) throw new Exception();

			dest.Add(RencodeConst.CHR_NONE);
		}

		#endregion

		#region Decode

		private static object DecodeObject(string x, int startIndex, out int endIndex)
		{
			return DecodeFunc[x[startIndex]](x, startIndex, out endIndex);
		}

		private static string DecodeString(string x, int startIndex, out int endIndex)
		{
			int indexOfColon = x.IndexOf(':', startIndex);
			int stringLength = Convert.ToInt32(x.Substring(startIndex, indexOfColon - startIndex)); // TODO: doesn't support long length

			indexOfColon += 1;
			string s = x.Substring(indexOfColon, stringLength);

			endIndex = indexOfColon+stringLength;
			return s;
		}

		private static object DecodeInt(string x, int startIndex, out int endIndex)
		{
			startIndex += 1;
			int newf = x.IndexOf(RencodeConst.CHR_TERM, startIndex);
			if(newf - startIndex >= RencodeConst.MAX_INT_LENGTH) {
				throw new Exception("Overflow");
			}
			long n = Convert.ToInt64(x.Substring(startIndex, newf - startIndex));

			if(x[startIndex] == '-'){
				if(x[startIndex + 1] == '0') {
					throw new Exception("Value Error");
				}
			}
			else if(x[startIndex] == '0' && newf != startIndex+1) {
				throw new Exception("Value Error");
			}

			endIndex = newf+1;
			return n;
		}

		private static object DecodeInt1(string x, int startIndex, out int endIndex)
		{
			startIndex += 1;
			endIndex = startIndex + 1;

			return BStruct.ToInt1(Util.StringBytes(x.Substring(startIndex, 1)), 0);
		}

		private static object DecodeInt2(string x, int startIndex, out int endIndex)
		{
			startIndex += 1;
			endIndex = startIndex + 2;

			return BStruct.ToInt2(Util.StringBytes(x.Substring(startIndex, 2)), 0);
		}

		private static object DecodeInt4(string x, int startIndex, out int endIndex)
		{
			startIndex += 1;
			endIndex = startIndex + 4;

            return BStruct.ToInt4(Util.StringBytes(x.Substring(startIndex, 4)), 0);
		}

		private static object DecodeInt8(string x, int startIndex, out int endIndex)
		{
			startIndex += 1;
			endIndex = startIndex + 8;

			return BStruct.ToInt8(Util.StringBytes(x.Substring(startIndex, 8)), 0);
		}

		private static object DecodeFloat(string x, int startIndex, out int endIndex)
		{
			startIndex += 1;
			endIndex = startIndex + 4;

			return BStruct.ToFloat(Util.StringBytes(x.Substring(startIndex, 4)), 0);
		}

		private static object DecodeDouble(string x, int startIndex, out int endIndex)
		{
			startIndex += 1;
			endIndex = startIndex + 8;

			return BStruct.ToDouble(Util.StringBytes(x.Substring(startIndex, 8)), 0);
		}

		private static object DecodeList(string x, int startIndex, out int endIndex)
		{
			var result = new List<object>();
			startIndex = startIndex + 1;
			while(x[startIndex] != RencodeConst.CHR_TERM)
			{
				object v = DecodeObject(x, startIndex, out startIndex);
				result.Add(v);
			}
			endIndex = startIndex + 1;
			return result.ToArray();
		}

		private static object DecodeDictionary(string x, int startIndex, out int endIndex)
		{
			var result = new Dictionary<object, object>();
			startIndex = startIndex + 1;
			while(x[startIndex] != RencodeConst.CHR_TERM)
			{
				object k = DecodeObject(x, startIndex, out startIndex);
				object v = DecodeObject(x, startIndex, out startIndex);
				result.Add(k, v);
			}

			endIndex = startIndex + 1;
			return result;
		}

		private static object DecodeBoolTrue(string x, int startIndex, out int endIndex)
		{
			endIndex = startIndex + 1;
			return true;
		}

		private static object DecodeBoolFalse(string x, int startIndex, out int endIndex)
		{
			endIndex = startIndex + 1;
			return false;
		}

		private static object DecodeNull(string x, int startIndex, out int endIndex)
		{
			endIndex = startIndex + 1;
			return null;
		}

		#endregion
	}
}

