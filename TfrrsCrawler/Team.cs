using System.Collections;
using System.Collections.Generic;

namespace TfrrsCrawler
{
	public class Team : IEnumerable<Athlete>
	{
		public List<Athlete> Athletes { get; } = new List<Athlete>();
		public Dictionary<string, List<string>> TopPerformancesAllTime { get; } = new Dictionary<string, List<string>>();

		public string Name { get; }

		public Team(string name)
		{
			Name = name;
		}

		public IEnumerator<Athlete> GetEnumerator()
		{
			return Athletes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}