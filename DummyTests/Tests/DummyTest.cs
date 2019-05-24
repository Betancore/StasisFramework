using NUnit.Framework;

namespace DummyTests.Tests
{
	public class DummyTest : BaseDummyTest
	{
		[Test]
		public void FirstDummyTest()
		{
			User.At.AtFakeForm.SetSearch("Autotests");
			User.At.AtFakeForm.Search();
		}
	}
}
