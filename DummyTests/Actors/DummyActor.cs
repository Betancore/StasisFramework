using DummyTests.Forms;

namespace DummyTests.Actors
{
	public class DummyActor
	{
		public FormsContainer At;
		public Steps.Steps Performs;

		public DummyActor()
		{
			At = new FormsContainer();
			Performs = new Steps.Steps(At);
		}
	}
}
