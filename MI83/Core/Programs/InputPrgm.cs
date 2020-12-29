namespace MI83.Core.Programs
{
	class InputPrgm : Program
	{
		private readonly string _prompt;

		public InputPrgm(Computer computer, string prompt) : base(computer)
		{
			_prompt = prompt;
		}

		protected override object Main()
		{
			DispHome();
			return "TODO";
		}
	}
}
