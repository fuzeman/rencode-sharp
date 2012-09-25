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
			Assert.AreEqual(120, (int)BStruct.Pack(120, 1)[0]);
			Assert.AreEqual(246, (int)BStruct.Pack(-10, 1)[0]);
			Assert.AreEqual(226, (int)BStruct.Pack(-30, 1)[0]);

			Assert.AreEqual(125, (int)BStruct.Pack(32000, 2)[1]);
			Assert.AreEqual(131, (int)BStruct.Pack(-32000, 2)[1]);

			Assert.AreEqual("\xa0\x86\x01\x00", BStruct.Pack(100000, 4));
			Assert.AreEqual("`y\xfe\xff", BStruct.Pack(-100000, 4));
		}

		[Test()]
		public void BasicUnpack()
		{
			Assert.AreEqual(246, BStruct.ToBytes(-10)[0]);

			Assert.AreEqual(120, BStruct.Unpack(BStruct.Pack(120, 1), 1));

			string a = BStruct.Pack(-50, 1);
			Assert.AreEqual(206, (int)a[0]);
			Assert.AreEqual(-50, BStruct.Unpack(a, 1));

			Assert.AreEqual(32000, BStruct.Unpack(BStruct.Pack(32000, 2), 2));
		}

		[Test()]
		public void Pad()
		{
			Assert.AreEqual("\x00" + "Hello", Util.StringPad("Hello", 6));
		}
	}
}

