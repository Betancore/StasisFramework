using DummyTests.Forms;
using NUnit.Framework;

namespace DummyTests.Tests
{
	public class DummyTest : BaseDummyTest
	{
		public const string DummySearchText = "Autotests";

		[Test]
		public void FirstDummyTest()
		{
			User.At.AtFakeForm.SetSearch(DummySearchText);
			User.At.AtFakeForm.Search();
		}

		[Test]
		public void SecondDummyTest()
		{
			User.Performs.Search(DummySearchText);
		}

		[Test]
		public void ThirdDummyTest()
		{
			var fakeForm = new FakeForm();
			fakeForm.SetSearch(DummySearchText);
			fakeForm.Search();
		}
	}
}
