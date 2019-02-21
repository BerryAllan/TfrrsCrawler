using System;
using System.Diagnostics;
using System.Linq;
using HtmlAgilityPack;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TfrrsCrawler
{
	class Program
	{
		static void Main(string[] args)
		{
			StartCrawlerAsync();
		}

		private static void StartCrawlerAsync()
		{
			Team team = new Team("Wisconsin");
			var htmlDocument = GetHtmlDoc("https://www.tfrrs.org/teams/MD_college_m_Navy.html");
			var rosterTable = htmlDocument.DocumentNode.Descendants("table").ToList();

			if (rosterTable.Count < 1)
			{
				Console.WriteLine("error!");
				return;
			}

			var athletes = rosterTable[1].Descendants("tbody").ToList()[0].Descendants("tr").ToList();
			foreach (var athlete in athletes)
			{
				var name = athlete.Descendants("td").ToList()[0].Descendants("a").ToList()[0].InnerHtml.MyTrim();
				var year = athlete.Descendants("td").ToList()[1].InnerHtml.MyTrim();
				Athlete theAthlete = new Athlete(name, year);

				string athleteUrl = athlete.Descendants("td").ToList()[0].Descendants("a").ToList()[0]
					.Attributes["href"].Value
					.Insert(0, "https:");
				var athleteDoc = GetHtmlDoc(athleteUrl);
				var bestsTable = athleteDoc.DocumentNode.Descendants("table")
					.Where(table => table.Attributes["class"].Value.Equals("table bests")).ToList();
				var rows = bestsTable[0].Descendants("tr").ToList();
				foreach (var row in rows)
				{
					var tds = row.Descendants("td").ToList();
					var eventName1 = tds[0].InnerHtml.Trim().Replace("\n", string.Empty);
					var bestMark1 = tds[1].Descendants("a").ToList()[0].InnerHtml.MyTrim();
					theAthlete.BestMarks.Add(eventName1, bestMark1);

					if (tds.Count != 4 || !tds[3].Descendants("a").Any())
						continue;
					var eventName2 = tds[2].InnerHtml.Trim().Replace("\n", string.Empty);
					var bestMark2 = tds[3].Descendants("a").ToList()[0].InnerHtml.MyTrim();

					theAthlete.BestMarks.Add(eventName2, bestMark2);
				}

				team.Athletes.Add(theAthlete);
			}

			foreach (Athlete athlete in team)
			{
				Console.WriteLine(athlete.Name.MyTrim() + " : " + athlete.Year.MyTrim());
				foreach (var key in athlete.BestMarks.Keys)
				{
					Console.WriteLine("\t" + key.MyTrim() + " : " + athlete.BestMarks[key].MyTrim());
				}
			}
		}

		private static HtmlDocument GetHtmlDoc(string url)
		{
			var httpClient = new HttpClient();
			var html = httpClient.GetStringAsync(url);
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(html.Result);
			return htmlDocument;
		}		
	}
}