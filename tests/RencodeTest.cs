using NUnit.Framework;
using System;
using rencodesharp;

namespace rencodesharp_tests
{
	[TestFixture()]
	public class RencodeTest
	{
		[Test()]
		public void String()
		{
			// STRING
			Assert.AreEqual("\x85Hello", Rencode.dumps("Hello"));
			Assert.AreEqual("78:abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", Rencode.dumps("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz"));
		}

		[Test()]
		public void Integer()
		{
			// INT1
			Assert.AreEqual("\x3E\x78", Rencode.dumps(120));
			Assert.AreEqual("\x3E\x88", Rencode.dumps(-120));

			// INT2
			Assert.AreEqual("\x3F\x06\x04", Rencode.dumps(1540));
			Assert.AreEqual("\x3F\xF9\xFC", Rencode.dumps(-1540));

			// INT4
			Assert.AreEqual("\x40\x7F\xFF\xFF\xD0", Rencode.dumps(2147483600));
			Assert.AreEqual("\x40\x80\x00\x00\x30", Rencode.dumps(-2147483600));

			// INT8
			Assert.AreEqual("\x41\x7F\xFF\xFF\xFF\xFF\xFE\xD7\xE0", Rencode.dumps(9223372036854700000L));
			Assert.AreEqual("\x41\x80\x00\x00\x00\x00\x01\x28\x20", Rencode.dumps(-9223372036854700000L));
		}
	}
}

