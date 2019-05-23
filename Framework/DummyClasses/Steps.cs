namespace ProductX.Framework.DummyClasses
{
	public class Steps
	{
		private readonly Pages _pages;
		public Steps(Pages pages)
		{
			_pages = pages;
		}

		public void Search(string searchText)
		{
			_pages.AtFakeForm.SetSearch(searchText);
			_pages.AtFakeForm.Search();
		}
	}
}
