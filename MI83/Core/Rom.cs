namespace MI83.Core
{
	using System.IO;

	static class Rom
	{
		public static string Get(string resourceName)
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var names = assembly.GetManifestResourceNames();
			using var resourceStream = System.Reflection.Assembly
				.GetExecutingAssembly()
				.GetManifestResourceStream($"{nameof(MI83)}.Resources.{resourceName}");
			using var reader = new StreamReader(resourceStream);
			return reader.ReadToEnd();
		}
	}
}
