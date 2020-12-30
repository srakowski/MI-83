namespace MI83.Core.Programs
{
	using Microsoft.Xna.Framework.Input;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
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
			var (rows, cols) = _GetHomeDim();

			var selectedTabIdx = 0;
			var selectedOptIdx = 0;

			var tabs = _tabs.Select(tab =>
			{
				var t = (tab as IEnumerable<object>).ToArray();
				return 
				(
					Name: t[0] as string,
					Options: t.Skip(1).Select(o => o as string).ToArray()
				);
			}).ToArray();

			var selection = (-1, -1);

			var fg = GetFG();
			var bg = GetBG();
			while (selection is (-1, -1))
			{
				ClrHome();

				var selectedTab = tabs[selectedTabIdx];
				var selectedOpt = selectedOptIdx < selectedTab.Options.Length
					? selectedTab.Options[selectedOptIdx]
					: null;

				Render(rows, cols, tabs, fg, bg, selectedTab, selectedOpt);

				while (true)
				{
					var key = (Keys)GetKey();
					while (key < 0)
					{
						key = (Keys)GetKey();
					}

					if (key == Keys.Right)
					{
						selectedTabIdx++;
						selectedTabIdx = selectedTabIdx >= tabs.Length ? 0 : selectedTabIdx;
						selectedOptIdx = selectedOptIdx >= tabs[selectedTabIdx].Options.Length ? 0 : selectedOptIdx;
						break;
					}
					else if (key == Keys.Left)
					{
						selectedTabIdx--;
						selectedTabIdx = selectedTabIdx < 0 ? tabs.Length - 1 : selectedTabIdx;
						selectedOptIdx = selectedOptIdx >= tabs[selectedTabIdx].Options.Length ? 0 : selectedOptIdx;
						break;
					}
					else if (key == Keys.Up)
					{
						selectedOptIdx--;
						selectedOptIdx = selectedOptIdx < 0 ? selectedTab.Options.Length - 1 : selectedOptIdx;
						break;
					}
					else if (key == Keys.Down)
					{
						selectedOptIdx++;
						selectedOptIdx = selectedOptIdx >= selectedTab.Options.Length ? 0 : selectedOptIdx;
						break;
					}
					else if (key == Keys.Enter &&
						selectedTabIdx >= 0 && selectedOptIdx >= 0)
					{
						selection = (selectedTabIdx, selectedOptIdx);
						break;
					}
				}
			}

			return selection;
		}

		private void Render(
			int rows,
			int cols,
			(string Name, string[] Options)[] tabs,
			int fg,
			int bg,
			(string Name, string[] Options) selectedTab,
			string selectedOpt)
		{
			var buffer = new StringBuilder();
			var line = new char[cols];

			void AppendBuffer(string l)
			{
				Array.Fill(line, '\0');
				for (var i = 0; i < l.Length && i < line.Length; i++)
				{
					line[i] = l[i];
				}
				buffer.Append(line);
			}

			AppendBuffer(string.Join(" ", tabs.Select(t => t.Name)));
			var r = 1;
			foreach (var option in selectedTab.Options)
			{
				AppendBuffer($"{r}:{option}");
				r++;
			}

			Output(0, 0, buffer.ToString());

			//var position = (Row: 0, Col: 0);
			//foreach (var tab in tabs)
			//{
			//	if (tab == selectedTab)
			//	{
			//		SetFG(bg);
			//		SetBG(fg);
			//	}

			//	Output(position.Row, position.Col, tab.Name);
			//	position.Col += tab.Name.Length;

			//	if (tab == selectedTab)
			//	{
			//		SetFG(fg);
			//		SetBG(bg);
			//	}

			//	if (tab != tabs.Last())
			//	{
			//		Output(position.Row, position.Col, " ");
			//		position.Col += 1;
			//	}
			//}

			//var row = 1;
			//foreach (var opt in selectedTab.Options)
			//{
			//	if (row >= rows)
			//	{
			//		break;
			//	}

			//	var num = $"{row}:";

			//	if (opt == selectedOpt)
			//	{
			//		SetFG(bg);
			//		SetBG(fg);
			//	}

			//	Output(row, 0, num);

			//	if (opt == selectedOpt)
			//	{
			//		SetFG(fg);
			//		SetBG(bg);
			//	}

			//	var col = num.Length;
			//	Output(row, col, opt);
			//	row++;
			//}
		}
	}
}
