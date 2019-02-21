using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TfrrsCrawlerGUI
{
	public class MeetEvent : IEnumerable
	{
		public Meet Meet { get; }
		public Dictionary<Athlete, Mark> Results { get; } = new Dictionary<Athlete, Mark>();
		public string Name { get; }

		public MeetEvent(string name, Meet meet)
		{
			Meet = meet;
			Name = name;
		}

		public Dictionary<Athlete, Mark>.Enumerator GetEnumerator()
		{
			return Results.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void SortAthletesBest()
		{
			var keys = Results.Keys.OrderBy(athlete => athlete.BestMarkOfEvent(this)).ToList();
			var newDict = new Dictionary<Athlete, Mark>();
			keys.ForEach(key => newDict.Add(key, Results[key]));
			Results.Clear();
			Results.AddRange(newDict);
		}

		public void SortAthletesSeasonBest(string season)
		{
			var keys = Results.Keys.OrderBy(athlete => athlete.BestMarkOfSeasonEvent(this, season)).ToList();
			var newDict = new Dictionary<Athlete, Mark>();
			keys.ForEach(key => newDict.Add(key, Results[key]));
			Results.Clear();
			Results.AddRange(newDict);
		}

		

		public override string ToString()
		{
			return Name;
		}

		public static bool operator ==(MeetEvent event1, MeetEvent event2)
		{
			return event1?.Name == event2?.Name;
		}

		public static bool operator !=(MeetEvent event1, MeetEvent event2)
		{
			return !(event1 == event2);
		}
	}
}