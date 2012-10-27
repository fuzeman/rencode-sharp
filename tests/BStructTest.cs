using NUnit.Framework;
using System;
using rencodesharp;
using MiscUtil.Conversion;

namespace rencodesharp_tests
{
	[TestFixture()]
	public class BStructTest
	{
		[Test()]
		public void Int1()
		{
			int a;

			a = BStruct.ToInt1(new byte[] { 50 }, 0);
			Assert.AreEqual(50, a);

			a = BStruct.ToInt1(new byte[] { 206 }, 0);
			Assert.AreEqual(-50, a);
		}

		[Test()]
		public void Int2()
		{
			int a;

			a = BStruct.ToInt2(new byte[] { 6, 4 }, 0);
			Assert.AreEqual(1540, a);

			a = BStruct.ToInt2(new byte[] { 1, 44 }, 0);
			Assert.AreEqual(300, a);

			a = BStruct.ToInt2(new byte[] { 254, 212 }, 0);
			Assert.AreEqual(-300, a);
		}

		[Test()]
		public void Int4()
		{
			string se = BStruct.Pack(1009025546, 4);

			Assert.AreEqual(60,		(int)se[0]);
			Assert.AreEqual(36,		(int)se[1]);
			Assert.AreEqual(130,	(int)se[2]);
			Assert.AreEqual(10,		(int)se[3]);

			int a;

			a = BStruct.ToInt4(new byte[] { 0, 0, 195, 80 }, 0);
			Assert.AreEqual(50000, a);

			a = BStruct.ToInt4(new byte[] { 255, 255, 60, 176 }, 0);
			Assert.AreEqual(-50000, a);
		}

		[Test()]
		public void Int8()
		{
			long a;

			a = BStruct.ToInt8(new byte[] { 0, 0, 0, 4, 168, 23, 200, 0 }, 0);
			Assert.AreEqual(20000000000, a);

			a = BStruct.ToInt8(new byte[] { 255, 255, 255, 251, 87, 232, 56, 0 }, 0);
			Assert.AreEqual(-20000000000, a);
		}
	}
}

