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
			Assert.AreEqual("\x85Hello", Rencode.dumps("Hello"));
			Assert.AreEqual("78:abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", Rencode.dumps("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz"));

			// DECODE STRING
			Assert.AreEqual("Hello", Rencode.loads("\x85Hello"));
			Assert.AreEqual("abcdefghij", Rencode.loads("10:abcdefghij"));
		}

		[Test()]
		public void Integer()
		{
			// ENCODE INT1
			Assert.AreEqual(Rencode.CHR_INT1 + "\x78", Rencode.dumps(120));
			Assert.AreEqual(Rencode.CHR_INT1 + "\x88", Rencode.dumps(-120));

			// ENCODE INT2
			Assert.AreEqual(Rencode.CHR_INT2 + "\x04\x06", Rencode.dumps(1540));
			Assert.AreEqual(Rencode.CHR_INT2 + "\xFC\xF9", Rencode.dumps(-1540));

			// ENCODE INT4
			Assert.AreEqual(Rencode.CHR_INT4 + "\xD0\xFF\xFF\x7F", Rencode.dumps(2147483600));
			Assert.AreEqual(Rencode.CHR_INT4 + "\x30\x00\x00\x80", Rencode.dumps(-2147483600));

			// ENCODE INT8
			Assert.AreEqual(Rencode.CHR_INT8 + "\xE0\xD7\xFE\xFF\xFF\xFF\xFF\x7F", Rencode.dumps(9223372036854700000L));
			Assert.AreEqual(Rencode.CHR_INT8 + "\x20\x28\x01\x00\x00\x00\x00\x80", Rencode.dumps(-9223372036854700000L));


			// DECODE INT
			Assert.AreEqual(1000, Rencode.loads(Rencode.CHR_INT + "1000" + Rencode.CHR_TERM));
;
			// DECODE INT1
			Assert.AreEqual(120, Rencode.loads(Rencode.CHR_INT1 + "\x78"));
			Assert.AreEqual(-120, Rencode.loads(Rencode.CHR_INT1 + "\x88"));

			// DECODE INT2
			Assert.AreEqual(1540, Rencode.loads(Rencode.CHR_INT2 + "\x04\x06"));
			Assert.AreEqual(-1540, Rencode.loads(Rencode.CHR_INT2 + "\xFC\xF9"));

			// DECODE INT4
			Assert.AreEqual(2147483600, Rencode.loads(Rencode.CHR_INT4 + "\xD0\xFF\xFF\x7F"));
			Assert.AreEqual(-2147483600, Rencode.loads(Rencode.CHR_INT4 + "\x30\x00\x00\x80"));

			// DECODE INT8
			Assert.AreEqual(9223372036854700000L, Rencode.loads(Rencode.CHR_INT8 + "\xE0\xD7\xFE\xFF\xFF\xFF\xFF\x7F"));
			Assert.AreEqual(-9223372036854700000L, Rencode.loads(Rencode.CHR_INT8 + "\x20\x28\x01\x00\x00\x00\x00\x80"));
		}

		[Test()]
		public void List()
		{
			Assert.AreEqual(new object[] { "one", "two", "three" },
				Rencode.loads(
					Rencode.dumps(new object[] { "one", "two", "three" })
				)
			);

			Assert.AreEqual(new object[] { 1, 2, 3 },
				Rencode.loads(
					Rencode.dumps(new object[] { 1, 2, 3 })
				)
			);

			Assert.AreEqual(new object[] { -1, -2, -3 },
				Rencode.loads(
					Rencode.dumps(new object[] { -1, -2, -3 })
				)
			);

			Assert.AreEqual(new object[] {
					new object[] { "one", "two", "three" },
					new object[] { "four", "five", "six" }
				},
				Rencode.loads(
					Rencode.dumps(new object[] {
						new object[] { "one", "two", "three" },
						new object[] { "four", "five", "six" }
					})
				)
			);

			Assert.AreEqual(new object[] {
					new object[] { 1, 2, 3 },
					new object[] { 4, 5, 6 }
				},
				Rencode.loads(
					Rencode.dumps(new object[] {
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
			string dump = Rencode.dumps(non_fixed_list_test);
			Assert.AreEqual(Rencode.CHR_LIST, (int)dump[0]);
			Assert.AreEqual(Rencode.CHR_TERM, (int)dump[dump.Length - 1]);
			Assert.AreEqual(non_fixed_list_test, Rencode.loads(dump));
		}

		[Test()]
		public void Dict()
		{
			Dictionary<object, object> dOne = new Dictionary<object, object> {
				{"Hello", 12},
				{"Blah", 15}
			};

			// Test Encode
			string dump = Rencode.dumps(dOne);
			Assert.AreEqual(((char)(Rencode.DICT_FIXED_START + 2)).ToString() +
			                ((char)(Rencode.STR_FIXED_START + 5)).ToString() +
			                "Hello" +
			                ((char)12) + 
			                ((char)(Rencode.STR_FIXED_START + 4)).ToString() +
			                "Blah" +
			                ((char)15), dump);

			Assert.AreEqual(dOne, Rencode.loads(dump));


			Dictionary<object, object> dTwo = new Dictionary<object, object>();
			for(int i = 0; i < 35; i++)
			{
				dTwo.Add(i.ToString(), i);
			}

			Assert.AreEqual(dTwo, Rencode.loads(Rencode.dumps(dTwo)));
		}
	}
}

