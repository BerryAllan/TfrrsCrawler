using System;
using System.Collections;
using System.Collections.Generic;

namespace TfrrsCrawlerGUI
{
	public class Team : IEnumerable<Athlete>
	{
		public List<Athlete> Athletes { get; } = new List<Athlete>();
		public byte[] Color { get; }

		public string Name { get; }

		public Team(string name)
		{
			Name = name;
			Random random = new Random();
			byte color = (byte) random.Next(175, 225);
			Color = new[]
			{
				color, color, color
			};
		}

		public IEnumerator<Athlete> GetEnumerator()
		{
			return Athletes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override string ToString()
		{
			return Name;
		}

		public static bool operator ==(Team team1, Team team2)
		{
			return team1?.Name == team2?.Name;
		}

		public static bool operator !=(Team team1, Team team2)
		{
			return !(team1 == team2);
		}
	}
}