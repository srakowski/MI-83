namespace MI83.Core
{
	using System.IO;
	using System.Linq;
	using System.Threading;

	class ProgramRegistry
	{
		private const string ProgramsDirectory = "./prgms";

		private Computer _computer;

		public ProgramRegistry(Computer computer)
		{
			_computer = computer;
		}

		public string[] GetPrgms()
		{
			CreatePrgmsDirectoryIfItDoesNotExist();
			return Directory.GetFiles(ProgramsDirectory, "*.prgm")
				.Select(f => Path.GetFileNameWithoutExtension(f))
				.ToArray();
		}

		public void CreatePrgm(string name)
		{
			CreatePrgmsDirectoryIfItDoesNotExist();
			File.WriteAllText(CreatePrgmFileName(name), null);
		}

		public string ReadPrgm(string name)
		{
			CreatePrgmsDirectoryIfItDoesNotExist();
			return File.ReadAllText(CreatePrgmFileName(name));
		}

		public void RunPrgm(string name)
		{
			var code = File.ReadAllText(CreatePrgmFileName(name));
			var progTask = new Program(_computer, code).Execute();
			while (!progTask.IsCompleted)
			{
				Thread.Sleep(1);
			}
		}

		private static void CreatePrgmsDirectoryIfItDoesNotExist()
		{
			Directory.CreateDirectory(ProgramsDirectory);
		}

		private static string CreatePrgmFileName(string name)
		{
			return Path.Combine(ProgramsDirectory, $"{name.ToUpper()}.prgm");
		}
	}
}
