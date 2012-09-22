using NUnit.Framework;
using System;
using System.Text;
using rencodesharp;

namespace rencodesharp_tests
{
	[TestFixture()]
	public class PackTest
	{
		[Test()]
		public void BasicPack()
		{
			Assert.AreEqual(120, (int)BitConv.Pack(120, 1)[0]);
			Assert.AreEqual(246, (int)BitConv.Pack(-10, 1)[0]);
			Assert.AreEqual(226, (int)BitConv.Pack(-30, 1)[0]);

			Assert.AreEqual(125, (int)BitConv.Pack(32000, 2)[0]);
			Assert.AreEqual(131, (int)BitConv.Pack(-32000, 2)[0]);

			Assert.AreEqual("\x00\x01\x86\xa0", BitConv.Pack(100000, 4));
			Assert.AreEqual("\xff\xfey`", BitConv.Pack(-100000, 4));
		}
	}
}

