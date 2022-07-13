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
}
