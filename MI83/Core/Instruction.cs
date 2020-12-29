namespace MI83.Core
{
	record Instruction
	{
		public record RetPrgm(object Value) : Instruction;

		public record DispHome() : Instruction;
		public record DispGraph() : Instruction;
		public record RunPrgm(string PrgmName) : Instruction;
		public record GetKey() : Instruction;
		public record Pause() : Instruction;

		public record ClrHome() : Instruction;
	}
}
