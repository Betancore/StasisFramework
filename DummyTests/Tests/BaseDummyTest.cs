using DummyTests.Actors;
using NUnit.Framework;
using Stasis;
using Stasis.Enums;
using Stasis.Helpers;
using Stasis.Models.Google;

namespace DummyTests.Tests
{
	public class BaseDummyTest : BaseTest
	{
		public DummyActor User;
		public BaseDummyTest() : base("https://www.google.com/")
		{
			User = new DummyActor();
		}

		[OneTimeSetUp]
		public void Initialize()
		{
			if (SettingsHelper.GetValue(SettingsValues.UpdateTestData) == "yes")
			{
				// Mocked to avoid compilation level errors
				GoogleApiHelper.ExportTestDataTable(new GoogleApiUser(), "dummy spreadsheet id");
			}
		}
	}
}
