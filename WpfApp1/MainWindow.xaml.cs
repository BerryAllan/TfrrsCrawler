using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using NPOI.SS.UserModel;

namespace TfrrsCrawlerGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			TextBox1.Focus();
		}

		private void RunButton_OnClick(object sender, RoutedEventArgs e)
		{
			List<string> urls = new List<string>();
			foreach (var child in WrapPanel.Children)
				if (child is TextBox textBox)
					urls.Add(textBox.Text.MyTrim());
			urls.RemoveAll(string.IsNullOrEmpty);
			urls = urls.Distinct().ToList();

			//Meet testMeet = GenerateMeet(urls);

			Task<Meet> meet = Task.Run(() => GenerateMeet(urls));
			Task<IWorkbook> workbook = Task.Run(() => ExcelWriter.WriteMeetToExcel(meet.Result));
			var saveFileDialog = new SaveFileDialog
			{
				Filter = "Excel (*.xlsx)|*.xlsx",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				AddExtension = true,
				DefaultExt = "*.xlsx",
				Title = "Save Meet Excel File"
			};
			if (saveFileDialog.ShowDialog() == true)
				using (var filestream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
					workbook.Result.Write(filestream);
		}

		private void AddButton_OnClick(object sender, RoutedEventArgs e)
		{
			TextBox textBox = new TextBox
			{
				Width = 300, Height = 20,
				Margin = new Thickness(5)
			};
			WrapPanel.Children.Insert(WrapPanel.Children.Count - 2, textBox);
			textBox.Focus();
		}

		private static Task<Team> TeamCrawler(string url, Meet meet)
		{
			return Task.Run(() =>
			{
				var htmlDocument = Helpers.GetHtmlDoc(url);
				var rosterTable = htmlDocument.DocumentNode.Descendants("table").ToList();

				var teamName = htmlDocument.DocumentNode.Descendants("h3")
					.Where(node => node.Attributes["class"].Value.Equals("panel-title large-title")).ToList()[0]
					.InnerHtml;

				Team team = new Team(teamName);

				var athletes = rosterTable[url.Contains("/xc/") ? 0 : 1].Descendants("tbody").ToList()[0]
					.Descendants("tr")
					.ToList();
				foreach (var athlete in athletes)
				{
					var name = athlete.Descendants("td").ToList()[0].Descendants("a").ToList()[0].InnerHtml.MyTrim();
					var year = athlete.Descendants("td").ToList()[1].InnerHtml.MyTrim();
					Athlete theAthlete = new Athlete(name, year, team);

					string athleteUrl = athlete.Descendants("td").ToList()[0].Descendants("a").ToList()[0]
						.Attributes["href"].Value
						.Insert(0, "https:");
					var athleteDoc = Helpers.GetHtmlDoc(athleteUrl);

					var seasonHistory = athleteDoc.DocumentNode.Descendants("div")
						.Where(node =>
							node.Attributes.Contains("id") && node.Attributes["id"].Value.Equals("session-history"))
						.ToList();
					if (seasonHistory.Count < 1)
						continue;

					var seasons = seasonHistory[0].Descendants("h3").ToList();
					var divs = seasonHistory[0].ChildNodes.Where(node => node.Name == "div").ToList();
					foreach (var div in divs)
					{
						var tables = div.ChildNodes.Where(node => node.Name == "table").ToList();
						foreach (var @event in tables)
						{
							string eventName = @event.Descendants("span").ToList()[0].InnerHtml.MyTrim();
							var rows = @event.Descendants("tr").ToList();
							foreach (var row in rows)
							{
								if (row.Attributes.Contains("class") && row.Attributes["class"].Value == "transfer")
									continue;
								var cells = row.Descendants("td").ToList();
								int counter = 0;
								foreach (var cell in cells)
								{
									if (counter % 3 == 0 && cell.Descendants("a").ToList().Count > 0)
										theAthlete.AllMarks.Add(new Mark(new MeetEvent(eventName, meet),
											cell.Descendants("a").ToList()[0].InnerHtml.MyTrim(),
											seasons[divs.IndexOf(div)].InnerHtml.MyTrim(), theAthlete,
											cells[2].InnerHtml.MyTrim()));

									counter++;
								}
							}
						}
					}

					theAthlete.GenerateBestMarks();
					theAthlete.GenerateSeasonBestMarks();

					team.Athletes.Add(theAthlete);
				}

				return team;
			});
		}

		private static Meet GenerateMeet(List<string> urls)
		{
			Meet meet = new Meet();

			urls.ForEach(url => meet.Teams.Add(TeamCrawler(url, meet).Result));

			meet.GenerateEventsBest();
			meet.GenerateEventsSeasonBest();
			meet.SetScoring();
			meet.GenerateScores();
			meet.SetTeamScores();

			return meet;
		}
	}
}