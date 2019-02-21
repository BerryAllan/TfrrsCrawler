using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TfrrsCrawlerGUI
{
	public class Athlete : IEnumerable<Mark>
	{
		public List<Mark> BestMarks { get; } = new List<Mark>();
		public Dictionary<string, List<Mark>> SeasonBestMarks { get; } = new Dictionary<string, List<Mark>>();
		public List<Mark> AllMarks { get; } = new List<Mark>();

		public string Name { get; }
		public string Year { get; }
		public Team Team { get; }

		public Athlete(string name, string year, Team team)
		{
			Name = name;
			Year = year;
			Team = team;
		}

		public Mark BestMarkOfEvent(MeetEvent meetEvent)
		{
			foreach (Mark mark in BestMarks)
			{
				if (mark.Event == meetEvent)
					return mark;
			}

			return null;
		}

		public Mark BestMarkOfSeasonEvent(MeetEvent meetEvent, string season)
		{
			foreach (var mark in SeasonBestMarks[season])
			{
				if (mark.Event == meetEvent)
					return mark;
			}

			return null;
		}

		public void GenerateBestMarks()
		{
			var allEvents = new List<MeetEvent>();
			foreach (Mark mark in AllMarks)
			{
				if (allEvents.All(meetEvent => meetEvent != mark.Event))
					allEvents.Add(mark.Event);
			}

			foreach (var meetEvent in allEvents)
			{
				var marksOfEvent = AllMarks.Where(mark => mark.Event == meetEvent).ToList();
				var bestMark = marksOfEvent[0].Type == "time"
					? new Mark(meetEvent, "999:99.99", string.Empty, this, string.Empty)
					: new Mark(meetEvent, "0.0m", string.Empty, this, string.Empty);
				foreach (var mark in marksOfEvent)
				{
					if (mark > bestMark)
						bestMark = mark;
				}

				BestMarks.Add(bestMark);
			}
		}

		public void GenerateSeasonBestMarks()
		{
			var seasons = new List<string>();
			foreach (Mark mark in AllMarks)
			{
				if (!seasons.Contains(mark.Season))
					seasons.Add(mark.Season);
			}

			foreach (var season in seasons)
			{
				var allEvents = new List<MeetEvent>();
				foreach (Mark mark in AllMarks)
				{
					if (allEvents.All(meetEvent => meetEvent != mark.Event) && mark.Season == season)
						allEvents.Add(mark.Event);
				}

				var bestMarks = new List<Mark>();
				foreach (var @event in allEvents)
				{
					var marksOfEvent = AllMarks.Where(mark => mark.Event == @event && mark.Season == season).ToList();
					var bestMark = marksOfEvent[0].Type == "time"
						? new Mark(@event, "999:99.99", season, this, string.Empty)
						: new Mark(@event, "0.0m", season, this, string.Empty);
					foreach (var mark in marksOfEvent)
					{
						if (mark > bestMark)
							bestMark = mark;
					}


					bestMarks.Add(bestMark);
				}

				SeasonBestMarks.Add(season, bestMarks);
			}
		}

		public IEnumerator<Mark> GetEnumerator()
		{
			return AllMarks.GetEnumerator();
		}

		public override string ToString()
		{
			return Team + "\t:\t" + Name + "\t:\t" + Year;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}