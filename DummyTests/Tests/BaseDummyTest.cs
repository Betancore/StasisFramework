using DummyTests.Actors;
using Stasis;

namespace DummyTests.Tests
{
	public class BaseDummyTest : BaseTest
	{
		public DummyActor User;
		public BaseDummyTest() : base("https://www.google.com/")
		{
			User = new DummyActor();
		}
	}
}
