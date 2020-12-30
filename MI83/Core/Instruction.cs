using System.Collections.Generic;

namespace MI83.Core
{
	record Instruction
	{
		public record DispHome() : Instruction;
		public record DispGraph() : Instruction;
		public record RunPrgm(string PrgmName) : Instruction;
		public record GetKey() : Instruction;
		public record Pause() : Instruction;
		public record GetSuppDispRes() : Instruction;
		public record GetDispRes() : Instruction;
		public record SetDispRes(int DispResIdx) : Instruction;
		public record GetFG() : Instruction;
		public record SetFG(int PaletteIdx) : Instruction;
		public record GetBG() : Instruction;
		public record SetBG(int PaletteIdx) : Instruction;
		public record _CreatePrgm(string PrgmName) : Instruction;
		public record _EditPrgm(string PrgmName) : Instruction;
		public record _ExitPrgm(object Value) : Instruction;
		public record _BeginText() : Instruction;
		public record _GetText() : Instruction;
		public record _EndText() : Instruction;

		public record ClrHome() : Instruction;
		public record Output(int Row, int Col, string Text) : Instruction;
		public record Disp(object Value) : Instruction;
		public record Input(string Prompt) : Instruction;
		public record Menu(IEnumerable<object> Tabs) : Instruction;
		public record _Prompt(string Text) : Instruction;
		public record _Scroll() : Instruction;
		public record _GetHomeDim() : Instruction;
		public record _Cursor(int Row, int Col, bool On) : Instruction;

		public record ClrDraw() : Instruction;
		public record Pixel(int X, int Y) : Instruction;
		public record Line(int X1, int Y1, int X2, int Y2) : Instruction;
		public record Horizontal(int Y) : Instruction;
		public record Vertical(int X) : Instruction;
	}
}
