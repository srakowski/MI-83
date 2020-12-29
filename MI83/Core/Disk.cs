namespace MI83.Core
{
	using System;
	using System.IO;
	using System.Linq;

	static class Disk
	{
		private const string ProgramsDirectory = "./prgms";

		public static string[] GetPrgms()
		{
			CreatePrgmsDirectoryIfItDoesNotExist();
			return Directory.GetFiles(ProgramsDirectory, "*.prgm")
				.Select(f => Path.GetFileNameWithoutExtension(f))
				.ToArray();
		}

		public static void WritePrgm(string prgmName, string code)
		{
			CreatePrgmsDirectoryIfItDoesNotExist();
			File.WriteAllText(CreatePrgmFileName(prgmName), code);
		}

		public static string ReadPrgm(string prgmName)
		{
			return File.ReadAllText(CreatePrgmFileName(prgmName));
		}

		public static void CreatePrgmsDirectoryIfItDoesNotExist()
		{
			Directory.CreateDirectory(ProgramsDirectory);
		}

		public static string CreatePrgmFileName(string prgmName)
		{
			return Path.Combine(ProgramsDirectory, $"{prgmName.ToUpper()}.prgm");
		}
	}
}
