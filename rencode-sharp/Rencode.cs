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
	public class Rencode
	{
		private delegate void EncodeDelegate(object x, List<object> r);
		private delegate object DecodeDelegate(string x, int f, out int endIndex);

		private static Dictionary<Type, EncodeDelegate> encode_func = new Dictionary<Type, EncodeDelegate>(){
			{typeof(string),						encode_string},

			{typeof(int),							encode_int},
			{typeof(long),							encode_int},
			{typeof(float),							encode_float},
			{typeof(double),						encode_double},

			{typeof(object[]),						encode_list},
			{typeof(Dictionary<object, object>),	encode_dict},

			{typeof(bool),							encode_bool},
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

			{RencodeConst.CHR_INT,	decode_int},
			{RencodeConst.CHR_INT1,	decode_int1},
			{RencodeConst.CHR_INT2,	decode_int2},
			{RencodeConst.CHR_INT4,	decode_int4},
			{RencodeConst.CHR_INT8,	decode_int8},
			{RencodeConst.CHR_FLOAT32,	decode_float},
			{RencodeConst.CHR_FLOAT64,	decode_double},

			{RencodeConst.CHR_LIST,	decode_list},
			{RencodeConst.CHR_DICT,	decode_dict},

			{RencodeConst.CHR_TRUE,	decode_bool_true},
			{RencodeConst.CHR_FALSE,	decode_bool_false},
			{RencodeConst.CHR_NONE,	decode_null},
		};

		#region Initialization

		static Rencode()
		{
			for(int i = 0; i < RencodeConst.STR_FIXED_COUNT; i++) {
				decode_func.Add((char)(RencodeConst.STR_FIXED_START + i), decode_fixed_string);
			}

			for(int i = 0; i < RencodeConst.LIST_FIXED_COUNT; i++) {
				decode_func.Add((char)(RencodeConst.LIST_FIXED_START + i), decode_fixed_list);
			}

			for(int i = 0; i < RencodeConst.INT_POS_FIXED_COUNT; i++) {
				decode_func.Add((char)(RencodeConst.INT_POS_FIXED_START + i), decode_fixed_pos_int);
			}

			for(int i = 0; i < RencodeConst.INT_NEG_FIXED_COUNT; i++) {
				decode_func.Add((char)(RencodeConst.INT_NEG_FIXED_START + i), decode_fixed_neg_int);
			}

			for(int i = 0; i < RencodeConst.DICT_FIXED_COUNT; i++) {
				decode_func.Add((char)(RencodeConst.DICT_FIXED_START + i), decode_fixed_dict);
			}
		}

		private static object decode_fixed_string(string x, int f, out int endIndex)
		{
			endIndex = f + 1 + ((int)x[f]) - RencodeConst.STR_FIXED_START;
			return x.Substring(f + 1, ((int)x[f]) - RencodeConst.STR_FIXED_START);
		}

		private static object decode_fixed_pos_int(string x, int f, out int endIndex)
		{
			endIndex = f + 1;
			return ((int)x[f]) - RencodeConst.INT_POS_FIXED_START;
		}


		private static object decode_fixed_neg_int(string x, int f, out int endIndex)
		{
			endIndex = f + 1;
			return -1 - (((int)x[f]) - RencodeConst.INT_NEG_FIXED_START);
		}

		private static object decode_fixed_list(string x, int f, out int endIndex)
		{
			List<object> r = new List<object>();
			int list_count = ((int)x[f]) - RencodeConst.LIST_FIXED_START;
			f = f + 1;

			for(int i = 0; i < list_count; i++)
			{
				object v = obj_decode(x, f, out f);
				r.Add(v);
			}

			endIndex = f;
			return r.ToArray();
		}

		private static object decode_fixed_dict(string x, int f, out int endIndex)
		{
			Dictionary<object, object> r = new Dictionary<object, object>();
			int dict_count = ((int)x[f]) - RencodeConst.DICT_FIXED_START;
			f = f + 1;

			for(int i = 0; i < dict_count; i++)
			{
				object k = obj_decode(x, f, out f);
				object v = obj_decode(x, f, out f);
				r.Add(k ,v);
			}

			endIndex = f;
			return r;
		}

		#endregion

		#region Core

		/// <summary>
		/// Encode object 'x' into a rencode string.
		/// </summary>
		public static string Encode(object x)
		{
			if(x != null && !encode_func.ContainsKey(x.GetType())){
				throw new Exception("No Encoder Found");
			}

			List<object> r = new List<object>();
			obj_encode(x, r);

			return Util.Join(r);
		}

		/// <summary>
		/// Decode rencode string 'x' into object.
		/// </summary>
		public static object Decode(string x)
		{
			if(!decode_func.ContainsKey(x[0])){
				throw new Exception("No Decoder Found");
			}

			int endIndex;
			return obj_decode(x, 0, out endIndex);
		}

		#endregion

		#region Encode

		private static void obj_encode(object x, List<object> r)
		{
			if(x == null)
				encode_null(x, r);
			else
				encode_func[x.GetType()](x, r);
		}

		private static void encode_string(object x, List<object> r)
		{
			string xs = (string)x;

			if (xs.Length < RencodeConst.STR_FIXED_COUNT) {
				r.Add((char)(RencodeConst.STR_FIXED_START + xs.Length));
				r.Add(xs);
			} else {
				r.Add(xs.Length.ToString());
				r.Add(':');
				r.Add(xs);
			}
		}

		private static void encode_int(object x, List<object> r)
		{
			// Check to determine if long type is able
			// to be packed inside an Int32 or is actually
			// an Int64 value.
			bool isLong = false;
			if(x.GetType() == typeof(long) &&(
				((long)x) > int.MaxValue ||
				((long)x) < int.MinValue))
					isLong = true;


			if(!isLong && 0 <= (int)x && (int)x < RencodeConst.INT_POS_FIXED_COUNT) {
				r.Add((char)(RencodeConst.INT_POS_FIXED_START+(int)x));
			} else if(!isLong && -RencodeConst.INT_NEG_FIXED_COUNT <= (int)x && (int)x < 0) {
				r.Add((char)(RencodeConst.INT_NEG_FIXED_START-1-(int)x));
			} else if(!isLong && -128 <= (int)x && (int)x < 128) {
				r.Add(RencodeConst.CHR_INT1);
				r.Add(BStruct.Pack(x, 1));
			} else if(!isLong && -32768 <= (int)x && (int)x < 32768) {
				r.Add(RencodeConst.CHR_INT2);
				r.Add(BStruct.Pack(x, 2));
			} else if(-2147483648L <= Convert.ToInt64(x) && Convert.ToInt64(x) < 2147483648L) {
				r.Add(RencodeConst.CHR_INT4);
				r.Add(BStruct.Pack(x, 4));
			} else if(-9223372036854775808L < Convert.ToInt64(x) && Convert.ToInt64(x) < 9223372036854775807L) {
				r.Add(RencodeConst.CHR_INT8);
				r.Add(BStruct.Pack(x, 8));
			} else {
				string s = (string)x;
				if(s.Length >= RencodeConst.MAX_INT_LENGTH)
					throw new ArgumentOutOfRangeException();
				r.Add(RencodeConst.CHR_INT);
				r.Add(s);
				r.Add(RencodeConst.CHR_TERM);
			}
		}

		private static void encode_float(object x, List<object> r)
		{
			r.Add((char)RencodeConst.CHR_FLOAT32);
			r.Add(BStruct.Pack(x, 4));
		}

		private static void encode_double(object x, List<object> r)
		{
			r.Add((char)RencodeConst.CHR_FLOAT64);
			r.Add(BStruct.Pack(x, 8));
		}

		private static void encode_list(object x, List<object> r)
		{
			if(x.GetType() != typeof(object[])) throw new Exception();
			object[] xl = (object[])x;

			if(xl.Length < RencodeConst.LIST_FIXED_COUNT) {
				r.Add((char)(RencodeConst.LIST_FIXED_START + xl.Length));
				foreach(object e in xl)
					obj_encode(e, r);
			} else {
				r.Add(RencodeConst.CHR_LIST);
				foreach(object e in xl)
					obj_encode(e, r);
				r.Add(RencodeConst.CHR_TERM);
			}
		}

		private static void encode_dict(object x, List<object> r)
		{
			if(x.GetType() != typeof(Dictionary<object, object>)) throw new Exception();
			Dictionary<object, object> xd = (Dictionary<object, object>)x;

			if(xd.Count < RencodeConst.DICT_FIXED_COUNT)
			{
				r.Add((char)(RencodeConst.DICT_FIXED_START + xd.Count));
				foreach(KeyValuePair<object, object> kv in xd)
				{
					obj_encode(kv.Key, r);
					obj_encode(kv.Value, r);
				}
			} else {
				r.Add((char)RencodeConst.CHR_DICT);
				foreach(KeyValuePair<object, object> kv in xd)
				{
					obj_encode(kv.Key, r);
					obj_encode(kv.Value, r);
				}
				r.Add((char)RencodeConst.CHR_TERM);
			}
		}

		private static void encode_bool(object x, List<object> r)
		{
			if(x.GetType() != typeof(bool)) throw new Exception();
			bool xb = (bool)x;

			if(xb == true)
				r.Add((char)RencodeConst.CHR_TRUE);
			else
				r.Add((char)RencodeConst.CHR_FALSE);
		}

		private static void encode_null(object x, List<object> r)
		{
			if(x != null) throw new Exception();

			r.Add((char)RencodeConst.CHR_NONE);
		}

		#endregion

		#region Decode

		private static object obj_decode(string x, int f, out int endIndex)
		{
			return decode_func[x[f]](x, f, out endIndex);
		}

		private static string decode_string(string x, int f, out int endIndex)
		{
			int colon = x.IndexOf(':', f);
			int n = Convert.ToInt32(x.Substring(f, colon)); // TODO: doesn't support long length

			colon += 1;
			string s = x.Substring(colon, (int)n);

			endIndex = colon+n;
			return s;
		}

		private static object decode_int(string x, int f, out int endIndex)
		{
			f += 1;
			int newf = x.IndexOf(RencodeConst.CHR_TERM, f);
			if(newf - f >= RencodeConst.MAX_INT_LENGTH) {
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

		private static object decode_int1(string x, int f, out int endIndex)
		{
			f += 1;
			endIndex = f + 1;

			return BStruct.ToInt1(Util.StringBytes(x.Substring(f, 1)), 0);
		}

		private static object decode_int2(string x, int f, out int endIndex)
		{
			f += 1;
			endIndex = f + 2;

			return BStruct.ToInt2(Util.StringBytes(x.Substring(f, 2)), 0);
		}

		private static object decode_int4(string x, int f, out int endIndex)
		{
			f += 1;
			endIndex = f + 4;

			return BStruct.ToInt4(Util.StringBytes(x.Substring(f, 4)), 0);
		}

		private static object decode_int8(string x, int f, out int endIndex)
		{
			f += 1;
			endIndex = f + 8;

			return BStruct.ToInt8(Util.StringBytes(x.Substring(f, 8)), 0);
		}

		private static object decode_float(string x, int f, out int endIndex)
		{
			f += 1;
			endIndex = f + 4;

			return BStruct.ToFloat(Util.StringBytes(x.Substring(f, 4)), 0);
		}

		private static object decode_double(string x, int f, out int endIndex)
		{
			f += 1;
			endIndex = f + 8;

			return BStruct.ToDouble(Util.StringBytes(x.Substring(f, 8)), 0);
		}

		private static object decode_list(string x, int f, out int endIndex)
		{
			List<object> r = new List<object>();
			f = f + 1;
			while(x[f] != RencodeConst.CHR_TERM)
			{
				object v = obj_decode(x, f, out f);
				r.Add(v);
			}
			endIndex = f + 1;
			return r.ToArray();
		}

		private static object decode_dict(string x, int f, out int endIndex)
		{
			Dictionary<object, object> r = new Dictionary<object, object>();
			f = f + 1;
			while(x[f] != RencodeConst.CHR_TERM)
			{
				object k = obj_decode(x, f, out f);
				object v = obj_decode(x, f, out f);
				r.Add(k, v);
			}

			endIndex = f + 1;
			return r;
		}

		private static object decode_bool_true(string x, int f, out int endIndex)
		{
			endIndex = f + 1;
			return true;
		}

		private static object decode_bool_false(string x, int f, out int endIndex)
		{
			endIndex = f + 1;
			return false;
		}

		private static object decode_null(string x, int f, out int endIndex)
		{
			endIndex = f + 1;
			return null;
		}

		#endregion
	}
}

