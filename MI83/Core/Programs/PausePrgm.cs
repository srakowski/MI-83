namespace MI83.Core.Programs
{
	using Microsoft.Xna.Framework.Input;
	using System;
	using System.Threading;

	class PausePrgm : Program
	{
		public PausePrgm(Computer computer) : base(computer) { }

		protected override object Main()
		{
			while (true)
			{
				Thread.Sleep(1);
				if (GetKey() == (int)Keys.Enter)
				{
					break;
				}
				// TODO: draw waiting
			}
			return null;
		}
	}
}
