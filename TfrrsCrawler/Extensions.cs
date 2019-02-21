namespace TfrrsCrawler
{
	public static class Extensions
	{
		public static string MyTrim(this string toTrim)
		{
			return toTrim.Trim().Replace("\n", string.Empty).Replace("\t", string.Empty);
		}
	}
}