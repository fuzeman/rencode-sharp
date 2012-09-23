using NUnit.Framework;
using System;
using rencodesharp;

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

			a = BStruct.ToInt2(new byte[] { 4, 6 }, 0);
			Assert.AreEqual(1540, a);

			a = BStruct.ToInt2(new byte[] { 44, 1 }, 0);
			Assert.AreEqual(300, a);

			a = BStruct.ToInt2(new byte[] { 212, 254 }, 0);
			Assert.AreEqual(-300, a);
		}

		[Test()]
		public void Int4()
		{
			int a;

			a = BStruct.ToInt4(new byte[] { 80, 195, 0, 0 }, 0);
			Assert.AreEqual(50000, a);

			a = BStruct.ToInt4(new byte[] { 176, 60, 255, 255 }, 0);
			Assert.AreEqual(-50000, a);
		}

		[Test()]
		public void Int8()
		{
			long a;

			a = BStruct.ToInt8(new byte[] { 0, 200, 23, 168, 4, 0, 0, 0 }, 0);
			Assert.AreEqual(20000000000, a);

			a = BStruct.ToInt8(new byte[] { 0, 56, 232, 87, 251, 255, 255, 255 }, 0);
			Assert.AreEqual(-20000000000, a);
		}
	}
}

