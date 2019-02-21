using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TfrrsCrawlerGUI
{
	public class Mark : IComparable<Mark>
	{
		public MeetEvent Event { get; }
		public Athlete Athlete { get; }
		public string Result { get; }
		public string Season { get; }
		public string Type { get; }
		public string Date { get; }
		public int Score { get; set; } = 0;

		private static readonly Regex DigitsOnly = new Regex(@"[^\d.:]");

		public Mark(MeetEvent @event, string result, string season, Athlete athlete, string date)
		{
			Event = @event;
			Result = result;
			Season = season;
			Athlete = athlete;
			Date = date;
			Type = @event.Name.EndsWith("Meters") || @event.Name.EndsWith("Relay") || @event.Name.EndsWith("(XC)") ||
			       @event.Name.EndsWith("Steeplechase") || @event.Name.EndsWith("Mile") ||
			       @event.Name.EndsWith("Hurdles")
				? "time"
				: "length";
		}

		public static bool operator <(Mark mark1, Mark mark2)
		{
			if (mark1.Type == "length")
			{
				if (mark1.Result == "FOUL") return true;
				if (mark2.Result == "FOUL") return false;

				string temp1 = DigitsOnly.Replace(mark1.Result, "");
				string temp2 = DigitsOnly.Replace(mark2.Result, "");
				if (temp1.Length < 1 || temp2.Length < 1)
					return false;
				return double.Parse(temp2) >
				       double.Parse(temp1);
			}

			if (mark1.Result == "DNF" || mark1.Result == "DNS" || mark1.Result == "DQ") return true;
			if (mark2.Result == "DNF" || mark2.Result == "DNS" || mark1.Result == "DQ") return false;

			if (DigitsOnly.Replace(mark1.Result, "").Length < 1 ||
			    DigitsOnly.Replace(mark2.Result, "").Length < 1) return false;

			var timeComponents1 = DigitsOnly.Replace(mark1.Result, "").Split(':', '.').Reverse().ToList();

			var milliseconds1 = timeComponents1.Any() ? int.Parse(timeComponents1[0]) : 0;
			var seconds1 = timeComponents1.Count() > 1 ? int.Parse(timeComponents1[1]) : 0;
			var minutes1 = timeComponents1.Count() > 2 ? int.Parse(timeComponents1[2]) : 0;

			var timeSpan1 = new TimeSpan(0, 0, minutes1, seconds1, milliseconds1);

			var timeComponents2 = DigitsOnly.Replace(mark2.Result, "").Split(':', '.').Reverse().ToList();

			var milliseconds2 = timeComponents2.Any() ? int.Parse(timeComponents2[0]) : 0;
			var seconds2 = timeComponents2.Count() > 1 ? int.Parse(timeComponents2[1]) : 0;
			var minutes2 = timeComponents2.Count() > 2 ? int.Parse(timeComponents2[2]) : 0;

			var timeSpan2 = new TimeSpan(0, 0, minutes2, seconds2, milliseconds2);

			return timeSpan1 > timeSpan2;
		}

		public static bool operator >(Mark mark1, Mark mark2)
		{
			return !(mark1 < mark2);
		}

		public static bool operator ==(Mark mark1, Mark mark2)
		{
			return mark1?.Result == mark2?.Result;
		}

		public static bool operator !=(Mark mark1, Mark mark2)
		{
			return !(mark1 == mark2);
		}

		public static bool operator <=(Mark mark1, Mark mark2)
		{
			return mark1 < mark2 || mark1 == mark2;
		}

		public static bool operator >=(Mark mark1, Mark mark2)
		{
			return !(mark1 <= mark2);
		}

		public int CompareTo(Mark other)
		{
			if (this == other) return 0;
			return this < other ? 1 : -1;
		}

		public override string ToString()
		{
			return Season + "\t:\t" + Event + "\t:\t" + Result;
		}

		public Mark Clone()
		{
			return new Mark(Event, Result, Season, Athlete, Date);
		}
	}
}