using System;
using System.Collections.Generic;
using System.Text;

namespace TfrrsCrawler
{
	public class Athlete
	{
		public Dictionary<string, string> BestMarks { get; } = new Dictionary<string, string>();
		public Dictionary<string, string> SeasonBestMarks { get; } = new Dictionary<string, string>();

		public string Name { get; }
		public string Year { get; }

		public Athlete(string name, string year)
		{
			Name = name;
			Year = year;
		}
	}
}
