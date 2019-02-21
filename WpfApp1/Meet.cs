using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace TfrrsCrawlerGUI
{
	public class Meet : IEnumerable<Team>, IEnumerable<MeetEvent>
	{
		public List<Team> Teams { get; } = new List<Team>();

		public List<MeetEvent> MeetEventsBest { get; } = new List<MeetEvent>();

		public Dictionary<Team, int> ScoresBest = new Dictionary<Team, int>();
		public Dictionary<Team, int> MeetEventsBestScore { get; private set; } = new Dictionary<Team, int>();

		public Dictionary<string, List<MeetEvent>> MeetEventsSeasonBest { get; } =
			new Dictionary<string, List<MeetEvent>>();

		public Dictionary<string, Dictionary<Team, int>> ScoresSeasonBests =
			new Dictionary<string, Dictionary<Team, int>>();

		public int[] Scores { get; private set; }

		IEnumerator<MeetEvent> IEnumerable<MeetEvent>.GetEnumerator()
		{
			return MeetEventsBest.GetEnumerator();
		}

		public IEnumerator<Team> GetEnumerator()
		{
			return Teams.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void GenerateEventsBest()
		{
			foreach (var team in this)
			foreach (var athlete in team)
			foreach (var mark in athlete)
				if (MeetEventsBest.All(meetEvent => meetEvent != mark.Event))
					MeetEventsBest.Add(new MeetEvent(mark.Event.Name, this));

			foreach (var team in this)
			foreach (var athlete in team)
			foreach (var meetEvent in MeetEventsBest)
			{
				if (athlete.BestMarks.All(mark => mark.Event != meetEvent))
					continue;
				meetEvent.Results.Add(athlete, athlete.BestMarkOfEvent(meetEvent).Clone());
			}

			MeetEventsBest.ForEach(meetEvent => { meetEvent.SortAthletesBest(); });
		}

		public void GenerateEventsSeasonBest()
		{
			var seasons = new List<string>();

			foreach (Team team in this)
			foreach (Athlete athlete in team)
			foreach (Mark mark in athlete)
				if (!seasons.Contains(mark.Season))
					seasons.Add(mark.Season);

			foreach (var season in seasons)
			{
				var bestMeetEvents = new List<MeetEvent>();
				foreach (var team in this)
				foreach (var athlete in team)
				foreach (var mark in athlete)
					if (bestMeetEvents.All(meetEvent => meetEvent != mark.Event))
						bestMeetEvents.Add(new MeetEvent(mark.Event.Name, this));

				foreach (var team in this)
				foreach (var athlete in team)
				foreach (var meetEvent in bestMeetEvents)
				{
					if (athlete.SeasonBestMarks.ContainsKey(season) &&
					    athlete.SeasonBestMarks[season].Any(mark => mark.Event == meetEvent))
						meetEvent.Results.Add(athlete, athlete.BestMarkOfSeasonEvent(meetEvent, season).Clone());
				}

				bestMeetEvents.ForEach(meetEvent => meetEvent.SortAthletesSeasonBest(season));
				bestMeetEvents.RemoveAll(meetEvent => meetEvent.Results.Count == 0);
				MeetEventsSeasonBest.Add(season, bestMeetEvents);
			}
		}

		public void GenerateScores()
		{
			void SetMarkScores(IEnumerable<MeetEvent> meetEvents, string season)
			{
				foreach (var meetEvent in meetEvents)
				{
					int team1Counter = 0;
					int team2Counter = 0;
					int overallCounter = 0;
					foreach (var athlete in meetEvent.Results.Keys)
					{
						if (!season.ToLower().Contains("xc"))
						{
							if (Scores.Length == 4)
							{
								if (team1Counter < 2 && athlete.Team == Teams[0])
								{
									meetEvent.Results[athlete].Score +=
										overallCounter < Scores.Length ? Scores[overallCounter] : 0;
									team1Counter++;
									overallCounter++;
								}
								else if (team2Counter < 2 && athlete.Team == Teams[1])
								{
									meetEvent.Results[athlete].Score +=
										overallCounter < Scores.Length ? Scores[overallCounter] : 0;
									team2Counter++;
									overallCounter++;
								}
							}
							else if (Scores.Length > 4)
							{
								int index = meetEvent.Results.Keys.ToList().IndexOf(athlete);
								meetEvent.Results[athlete].Score += index < Scores.Length ? Scores[index] : 0;
							}
						}
						else
						{
							meetEvent.Results[athlete].Score += meetEvent.Results.Keys.ToList().IndexOf(athlete) + 1;
						}
					}
				}
			}

			SetMarkScores(MeetEventsBest, string.Empty);

			foreach (var season in MeetEventsSeasonBest.Keys)
			{
				SetMarkScores(MeetEventsSeasonBest[season], season);
			}
		}

		public void SetScoring()
		{
			if (Teams.Count > 2)
				Scores = new[] {10, 8, 6, 5, 4, 3, 2, 1};
			else if (Teams.Count == 2)
				Scores = new[] {5, 3, 2, 1};
			else
				Scores = new int[0];
		}

		public void SetTeamScores()
		{
			foreach (var team in this)
			{
				int score = 0;
				foreach (var meetEvent in MeetEventsBest)
				{
					foreach (var athlete in meetEvent.Results.Keys)
					{
						if (athlete.Team == team)
							score += meetEvent.Results[athlete].Score;
					}
				}

				MeetEventsBestScore.Add(team, score);
			}

			foreach (var season in MeetEventsSeasonBest.Keys)
			{
				Dictionary<Team, int> tempScore = new Dictionary<Team, int>();

				foreach (var team in this)
				{
					int score = 0;
					foreach (var meetEvent in MeetEventsSeasonBest[season])
					{
						foreach (var athlete in meetEvent.Results.Keys)
						{
							if (athlete.Team == team)
								score += meetEvent.Results[athlete].Score;
						}
					}

					tempScore.Add(team, score);
				}

				ScoresSeasonBests.Add(season, tempScore);
			}
		}
	}
}