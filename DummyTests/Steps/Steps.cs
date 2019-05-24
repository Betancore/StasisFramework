using DummyTests.Forms;

namespace DummyTests.Steps
{
	public class Steps
	{
		private readonly FormsContainer _pages;
		public Steps(FormsContainer pages)
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
