using NUnit.Framework;
using System;
using rencodesharp;

namespace rencodesharp_tests
{
	[TestFixture()]
	public class UtilTests
	{
		[Test()]
		public void StringBytes()
		{
			Assert.AreEqual(16, (int)Util.StringBytes("\x10")[0]);
			Assert.AreEqual(127, (int)Util.StringBytes("\x7F")[0]);
			Assert.AreEqual(100, (int)Util.StringBytes("\x64")[0]);
		}
	}
}

