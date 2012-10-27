using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using rencodesharp;

namespace rencodesharp_tests
{
	[TestFixture()]
	public class RencodeTest
	{
		[Test()]
		public void String()
		{
			// ENCODE STRING
			Assert.AreEqual("\x85Hello", Rencode.Encode("Hello"));
			Assert.AreEqual("78:abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", Rencode.Encode("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz"));

			// DECODE STRING
			Assert.AreEqual("Hello", Rencode.Decode("\x85Hello"));
			Assert.AreEqual("abcdefghij", Rencode.Decode("10:abcdefghij"));
		}

		[Test()]
		public void Integer()
		{
			// ENCODE INT1
			Assert.AreEqual(RencodeConst.CHR_INT1 + "\x78", Rencode.Encode(120));
			Assert.AreEqual(RencodeConst.CHR_INT1 + "\x88", Rencode.Encode(-120));

			// ENCODE INT2
			Assert.AreEqual(RencodeConst.CHR_INT2 + "\x06\x04", Rencode.Encode(1540));
			Assert.AreEqual(RencodeConst.CHR_INT2 + "\xF9\xFC", Rencode.Encode(-1540));

			// ENCODE INT4
			Assert.AreEqual(RencodeConst.CHR_INT4 + "\x7F\xff\xff\xd0", Rencode.Encode(2147483600));
			Assert.AreEqual(RencodeConst.CHR_INT4 + "\x80\x00\x00\x30", Rencode.Encode(-2147483600));

			// ENCODE INT8
			Assert.AreEqual(65,  (int)Rencode.Encode(9223372036854700000L)[0]);
			Assert.AreEqual(127, (int)Rencode.Encode(9223372036854700000L)[1]);
			Assert.AreEqual(255, (int)Rencode.Encode(9223372036854700000L)[2]);
			Assert.AreEqual(255, (int)Rencode.Encode(9223372036854700000L)[3]);
			Assert.AreEqual(RencodeConst.CHR_INT8 + "\x7F\xFF\xFF\xFF\xFF\xFE\xD7\xE0", Rencode.Encode(9223372036854700000L));
			Assert.AreEqual(RencodeConst.CHR_INT8 + "\x80\x00\x00\x00\x00\x01( ", Rencode.Encode(-9223372036854700000L));


			// DECODE INT
			Assert.AreEqual(1000, Rencode.Decode(RencodeConst.CHR_INT + "1000" + RencodeConst.CHR_TERM));
;
			// DECODE INT1
			Assert.AreEqual(120, Rencode.Decode(RencodeConst.CHR_INT1 + "\x78"));
			Assert.AreEqual(-120, Rencode.Decode(RencodeConst.CHR_INT1 + "\x88"));

			// DECODE INT2
			Assert.AreEqual(1540, Rencode.Decode(RencodeConst.CHR_INT2 + "\x06\x04"));
			Assert.AreEqual(-1540, Rencode.Decode(RencodeConst.CHR_INT2 + "\xF9\xFC"));

			// DECODE INT4
			Assert.AreEqual(2147483600, Rencode.Decode(RencodeConst.CHR_INT4 + "\x7f\xff\xff\xd0"));
			Assert.AreEqual(-2147483600, Rencode.Decode(RencodeConst.CHR_INT4 + "\x80\x00\x00\x30"));

			// DECODE INT8
			Assert.AreEqual(9223372036854700000L, Rencode.Decode(RencodeConst.CHR_INT8 + "\x7F\xFF\xFF\xFF\xFF\xFE\xD7\xE0"));
			Assert.AreEqual(-9223372036854700000L, Rencode.Decode(RencodeConst.CHR_INT8 + "\x80\x00\x00\x00\x00\x01( "));
		}

		[Test()]
		public void List()
		{
			Assert.AreEqual(new object[] { "one", "two", "three" },
				Rencode.Decode(
					Rencode.Encode(new object[] { "one", "two", "three" })
				)
			);

			Assert.AreEqual(new object[] { 1, 2, 3 },
				Rencode.Decode(
					Rencode.Encode(new object[] { 1, 2, 3 })
				)
			);

			Assert.AreEqual(new object[] { -1, -2, -3 },
				Rencode.Decode(
					Rencode.Encode(new object[] { -1, -2, -3 })
				)
			);

			Assert.AreEqual(new object[] {
					new object[] { "one", "two", "three" },
					new object[] { "four", "five", "six" }
				},
				Rencode.Decode(
					Rencode.Encode(new object[] {
						new object[] { "one", "two", "three" },
						new object[] { "four", "five", "six" }
					})
				)
			);

			Assert.AreEqual(new object[] {
					new object[] { 1, 2, 3 },
					new object[] { 4, 5, 6 }
				},
				Rencode.Decode(
					Rencode.Encode(new object[] {
						new object[] { 1, 2, 3 },
						new object[] { 4, 5, 6 }
					})
				)
			);

			object[] non_fixed_list_test = new object[100];
			Random rand = new Random();
			for(int i = 0; i < 100; i++)
			{
				non_fixed_list_test[i] = rand.Next();
			}
			string dump = Rencode.Encode(non_fixed_list_test);
			Assert.AreEqual(RencodeConst.CHR_LIST, (int)dump[0]);
			Assert.AreEqual(RencodeConst.CHR_TERM, (int)dump[dump.Length - 1]);
			Assert.AreEqual(non_fixed_list_test, Rencode.Decode(dump));
		}

		[Test()]
		public void Dict()
		{
			Dictionary<object, object> dOne = new Dictionary<object, object> {
				{"Hello", 12},
				{"Blah", 15}
			};

			// Test Encode
			string dump = Rencode.Encode(dOne);
			Assert.AreEqual(((char)(RencodeConst.DICT_FIXED_START + 2)).ToString() +
			                ((char)(RencodeConst.STR_FIXED_START + 5)).ToString() +
			                "Hello" +
			                ((char)12) + 
			                ((char)(RencodeConst.STR_FIXED_START + 4)).ToString() +
			                "Blah" +
			                ((char)15), dump);

			Assert.AreEqual(dOne, Rencode.Decode(dump));


			Dictionary<object, object> dTwo = new Dictionary<object, object>();
			for(int i = 0; i < 35; i++)
			{
				dTwo.Add(i.ToString(), i);
			}

			Assert.AreEqual(dTwo, Rencode.Decode(Rencode.Encode(dTwo)));
		}

		[Test()]
		public void Bool()
		{
			Assert.AreEqual(((char)RencodeConst.CHR_TRUE).ToString(), Rencode.Encode(true));
			Assert.AreEqual(((char)RencodeConst.CHR_FALSE).ToString(), Rencode.Encode(false));

			Assert.AreEqual(true, Rencode.Decode(Rencode.Encode(true)));
			Assert.AreEqual(false, Rencode.Decode(Rencode.Encode(false)));
		}

		[Test()]
		public void Null()
		{
			Assert.AreEqual(((char)RencodeConst.CHR_NONE).ToString(), Rencode.Encode(null));

			Assert.AreEqual(null, Rencode.Decode(Rencode.Encode(null)));
		}

		[Test()]
		public void Float()
		{
			Assert.AreEqual(0.005353f, Rencode.Decode(Rencode.Encode(0.005353f)));
			Assert.AreEqual(-0.005353f, Rencode.Decode(Rencode.Encode(-0.005353f)));
		}

		[Test()]
		public void Double()
		{
			Assert.AreEqual(0.005353d, Rencode.Decode(Rencode.Encode(0.005353d)));
			Assert.AreEqual(-0.005353d, Rencode.Decode(Rencode.Encode(-0.005353d)));
		}
	}
}

