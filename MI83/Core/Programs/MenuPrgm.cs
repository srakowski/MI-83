namespace MI83.Core.Programs
{
	using Microsoft.Xna.Framework.Input;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

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

			var selectedTabIdx = 0;
			var selectedOptIdx = 0;

			var tabs = _tabs.Select(tab =>
			{
				var t = (tab as IEnumerable<object>).ToArray();
				return new
				{
					Name = t[0] as string,
					Options = t.Skip(1).Select(o => o as string).ToArray()
				};
			}).ToArray();

			var selection = (-1, -1);

			var fg = GetFG();
			var bg = GetBG();
			while (selection is (-1, -1))
			{
				ClrHome();

				var selectedTab = tabs[selectedTabIdx];
				var position = (Row: 0, Col: 0);
				foreach (var tab in tabs)
				{
					if (tab == selectedTab)
					{
						SetFG(bg);
						SetBG(fg);
					}

					Output(position.Row, position.Col, tab.Name);
					position.Col += tab.Name.Length;

					if (tab == selectedTab)
					{
						SetFG(fg);
						SetBG(bg);
					}

					Output(position.Row, position.Col, " ");
					position.Col += 1;
				}

				var selectedOpt = selectedOptIdx < selectedTab.Options.Length
					? selectedTab.Options[selectedOptIdx]
					: null;

				var row = 1;
				foreach (var opt in selectedTab.Options)
				{
					var num = $"{row}:";

					if (opt == selectedOpt)
					{
						SetFG(bg);
						SetBG(fg);
					}

					Output(row, 0, num);

					if (opt == selectedOpt)
					{
						SetFG(fg);
						SetBG(bg);
					}

					var col = num.Length;
					Output(row, col, opt);
					row++;
				}

				var key = (Keys)GetKey();
				while (key < 0)
				{
					Thread.Sleep(17);
					key = (Keys)GetKey();
				}

				if (key == Keys.Right)
				{
					selectedTabIdx++;
					selectedTabIdx = selectedTabIdx >= tabs.Length ? 0 : selectedTabIdx;
					selectedOptIdx = selectedOptIdx >= tabs[selectedTabIdx].Options.Length ? 0 : selectedOptIdx;
				}
				else if (key == Keys.Left)
				{
					selectedTabIdx--;
					selectedTabIdx = selectedTabIdx < 0 ? tabs.Length - 1 : selectedTabIdx;
					selectedOptIdx = selectedOptIdx >= tabs[selectedTabIdx].Options.Length ? 0 : selectedOptIdx;
				}
				else if (key == Keys.Up)
				{
					selectedOptIdx--;
					selectedOptIdx = selectedOptIdx < 0 ? selectedTab.Options.Length - 1 : selectedOptIdx;
				}
				else if (key == Keys.Down)
				{
					selectedOptIdx++;
					selectedOptIdx = selectedOptIdx >= selectedTab.Options.Length ? 0 : selectedOptIdx;
				}
				else if (key == Keys.Enter &&
					selectedTabIdx >= 0 && selectedOptIdx >= 0)
				{
					selection = (selectedTabIdx, selectedOptIdx);
				}
			}

			return selection;
		}
	}
}

//			ClrHome();

//			var selectedTabIdx = 0;
//			var selectedOptIdx = 0;

//			var tabs = menu.Select(tab =>
//			{
//				var t = (tab as IEnumerable<object>).ToArray();
//				return new
//				{
//					Name = t[0] as string,
//					Options = t.Skip(1).Select(o => o as string).ToArray()
//				};
//			}).ToArray();

//			_keyUpBuffer = new Queue<Keys>();
//			var selection = (-1, -1);
//			while (selection is (-1, -1))
//			{
//				_buffer.Clear(FG, BG);
//				_cursor.SetPosition(0, 0);
//				var selectedTab = tabs[selectedTabIdx];
//				foreach (var tab in tabs)
//				{
//					_cursor.Write(
//						tab.Name,
//						tab == selectedTab ? BG : FG,
//						tab == selectedTab ? FG : BG,
//						OverflowMode.WrapAndTruncate);

//					_cursor.WriteChar(' ', FG, BG, OverflowMode.WrapAndTruncate);
//				}

//				var selectedOpt = selectedOptIdx < selectedTab.Options.Length
//					? selectedTab.Options[selectedOptIdx]
//					: null;

//				var r = 1;
//				_cursor.SetPosition(r, 0);
//				foreach (var opt in selectedTab.Options)
//				{
//					var num = $"{r}:";
//					_cursor.Write(
//						num,
//						opt == selectedOpt ? BG : FG,
//						opt == selectedOpt ? FG : BG,
//						OverflowMode.WrapAndTruncate);

//					_cursor.Write(opt, FG, BG, OverflowMode.WrapAndTruncate);
//					_cursor.SetPosition(++r, 0);
//				}

//				while (!_keyUpBuffer.Any())
//				{
//					Thread.Sleep(1);
//				}

//				while (_keyUpBuffer.Any())
//				{
//					var key = _keyUpBuffer.Dequeue();
//					if (key == Keys.Right)
//					{
//						selectedTabIdx++;
//						selectedTabIdx = selectedTabIdx >= tabs.Length ? 0 : selectedTabIdx;
//						selectedOptIdx = 0;
//						break;
//					}
//					else if (key == Keys.Left)
//					{
//						selectedTabIdx--;
//						selectedTabIdx = selectedTabIdx < 0 ? tabs.Length - 1 : selectedTabIdx;
//						selectedOptIdx = 0;
//						break;
//					}
//					else if (key == Keys.Up)
//					{
//						selectedOptIdx--;
//						selectedOptIdx = selectedOptIdx < 0 ? selectedTab.Options.Length - 1 : selectedOptIdx;
//						break;
//					}
//					else if (key == Keys.Down)
//					{
//						selectedOptIdx++;
//						selectedOptIdx = selectedOptIdx >= selectedTab.Options.Length ? 0 : selectedOptIdx;
//						break;
//					}
//					else if (key == Keys.Enter &&
//						selectedTabIdx >= 0 && selectedOptIdx >= 0)
//					{
//						selection = (selectedTabIdx, selectedOptIdx);
//						break;
//					}
//				}
//			}
//			_keyUpBuffer = null;
//			return selection;