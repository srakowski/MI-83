namespace MI83.Core.Programs
{
	using System.Collections.Generic;

	class MenuPrgm : Program
	{
		private readonly IEnumerable<object> _tabs;

		public MenuPrgm(Computer computer, IEnumerable<object> tabs) : base(computer)
		{
			_tabs = tabs;
		}

		protected override object Main()
		{
			ClrHome();
			return (-1, -1);
		}
	}
}
