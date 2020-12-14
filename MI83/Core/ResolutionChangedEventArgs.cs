namespace MI83.Core
{
	class ResolutionChangedEventArgs
	{
		public ResolutionChangedEventArgs(Resolution newResolution)
		{
			NewResolution = newResolution;
		}

		public Resolution NewResolution { get; }
	}
}