namespace MI83.Core;

record Instruction
{
	public record RunPrgm(string PrgmName) : Instruction;
	public record ThrowSyntaxError() : Instruction;
	public record ExitPrgm() : Instruction;

	public record EvaluateIdentifierAndPush(string Value) : Instruction;
	public record EvaluateIdentifierCallAndPush(string Value) : Instruction;
	public record PushStringLiteral(string Value) : Instruction;
	public record PushNumericLiteral(string Value) : Instruction;
	public record PopAssignPush(string VariableName) : Instruction;
	public record Label(string LabelToken) : Instruction;
	public record PopAndGoto() : Instruction;

	public record BeginList() : Instruction;
	public record PopAndAppendList() : Instruction;
	public record EndListAndPush() : Instruction;

	//public record DispHome() : Instruction;
	//public record DispGraph() : Instruction;	
	//public record GetKey() : Instruction;
	//public record Pause() : Instruction;
	//public record GetSuppDispRes() : Instruction;
	//public record GetDispRes() : Instruction;
	//public record SetDispRes(int DispResIdx) : Instruction;
	//public record GetFG() : Instruction;
	//public record SetFG(int PaletteIdx) : Instruction;
	//public record GetBG() : Instruction;
	//public record SetBG(int PaletteIdx) : Instruction;
	//public record CreatePrgm(string PrgmName) : Instruction;
	//public record EditPrgm(string PrgmName) : Instruction;
	//public record _BeginText() : Instruction;
	//public record _GetText() : Instruction;
	//public record _EndText() : Instruction;
	//public record ClrHome() : Instruction;
	//public record Output(int Row, int Col, string Text) : Instruction;
	//public record Disp(object Value) : Instruction;
	//public record Input(string Prompt) : Instruction;
	//public record Menu(IEnumerable<object> Tabs) : Instruction;
	//public record _Prompt(string Text) : Instruction;
	//public record _Scroll() : Instruction;
	//public record _GetHomeDim() : Instruction;
	//public record _Cursor(int Row, int Col, bool On) : Instruction;
	//public record ClrDraw() : Instruction;
	//public record Pixel(int X, int Y) : Instruction;
	//public record Line(int X1, int Y1, int X2, int Y2) : Instruction;
	//public record Horizontal(int Y) : Instruction;
	//public record Vertical(int X) : Instruction;
}
