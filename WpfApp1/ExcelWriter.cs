using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;

namespace TfrrsCrawlerGUI
{
	public class ExcelWriter
	{
		public static IWorkbook WriteMeetToExcel(Meet meet)
		{
			var workbook = new XSSFWorkbook();

			CreateMeetSheet(workbook, meet, meet.MeetEventsBest, "All-Time Best Results", string.Empty);
			foreach (var season in meet.MeetEventsSeasonBest.Keys)
			{
				CreateMeetSheet(workbook, meet, meet.MeetEventsSeasonBest[season], $"Virtual Meet({season})", season);
			}

			foreach (var team in meet)
			{
				CreateAthleteProfiles(workbook, $"Athlete Profiles - {team.Name}", team);
			}

			XSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook);
			return workbook;
		}

		private static void CreateMeetSheet(IWorkbook workbook, Meet meet, IEnumerable<MeetEvent> events,
			string sheetName, string season)
		{
			var sheet = workbook.CreateSheet(sheetName);

			var boldFont = workbook.CreateFont();
			boldFont.FontName = "Calibri";
			boldFont.FontHeightInPoints = 11;
			boldFont.Boldweight = (short) FontBoldWeight.Bold;

			var headerStyle = (XSSFCellStyle) workbook.CreateCellStyle();
			headerStyle.FillPattern = FillPattern.SolidForeground;
			headerStyle.FillForegroundColor = IndexedColors.LightCornflowerBlue.Index;
			headerStyle.Alignment = HorizontalAlignment.Center;
			headerStyle.SetFont(boldFont);
			headerStyle.BorderBottom = BorderStyle.Thin;
			headerStyle.BorderTop = BorderStyle.Thin;
			headerStyle.BorderLeft = BorderStyle.Thin;
			headerStyle.BorderRight = BorderStyle.Thin;

			var rowStyle = (XSSFCellStyle) workbook.CreateCellStyle();
			rowStyle.BorderLeft = BorderStyle.Thin;
			rowStyle.BorderRight = BorderStyle.Thin;

			int rowCount = 0;

			var rowHeaderTeams = sheet.CreateRow(rowCount++);
			rowHeaderTeams.CreateCell(1).SetCellValue("Team");
			rowHeaderTeams.CreateCell(2).SetCellValue("(Hypothetical) Scores");
			var headerAddressRange = new CellRangeAddress(rowCount - 1, rowCount - 1, 4, 5);
			rowHeaderTeams.CreateCell(4).SetCellValue("DATA MAY BE INACCURATE");
			sheet.AddMergedRegion(headerAddressRange);
			rowHeaderTeams.Cells.ForEach(cell => cell.CellStyle = headerStyle);

			foreach (var team in meet)
			{
				var teamScoreRow = sheet.CreateRow(rowCount++);
				teamScoreRow.CreateCell(1).SetCellValue(team.Name);
				var scoreCell = teamScoreRow.CreateCell(2);
				scoreCell.SetCellType(CellType.Formula);
				scoreCell.SetCellFormula($"SUMIF(B:B, B{rowCount}, F:F)");
				teamScoreRow.Cells.ForEach(cell => cell.CellStyle = rowStyle);
			}

			rowCount++;
			foreach (var meetEvent in events)
			{
				int eventStartGroup = rowCount;
				var eventRow = sheet.CreateRow(rowCount++);
				eventRow.CreateCell(0).SetCellValue(meetEvent.Name);
				CellRangeAddress cellRange = new CellRangeAddress(rowCount - 1, rowCount - 1, 0, 4);
				sheet.AddMergedRegion(cellRange);

				var headerRow = sheet.CreateRow(rowCount++);
				headerRow.CreateCell(0).SetCellValue("#");
				headerRow.CreateCell(1).SetCellValue("Team");
				headerRow.CreateCell(2).SetCellValue("Athlete");
				headerRow.CreateCell(3).SetCellValue("Performance");
				headerRow.CreateCell(4).SetCellValue("Date");

				eventRow.Cells.ForEach(cell => cell.CellStyle = headerStyle);
				headerRow.Cells.ForEach(cell => cell.CellStyle = headerStyle);

				foreach (var kvPair in meetEvent.Results)
				{
					var row = sheet.CreateRow(rowCount++);

					int cellCount = 0;
					row.CreateCell(cellCount++)
						.SetCellValue(meetEvent.Results.Keys.ToList().IndexOf(kvPair.Key) + 1);
					row.CreateCell(cellCount++).SetCellValue(meetEvent.Results[kvPair.Key].Athlete.Team.Name);
					row.CreateCell(cellCount++).SetCellValue(meetEvent.Results[kvPair.Key].Athlete.Name);
					row.CreateCell(cellCount++).SetCellValue(meetEvent.Results[kvPair.Key].Result);
					row.CreateCell(cellCount++).SetCellValue(meetEvent.Results[kvPair.Key].Date);

					int score = meetEvent.Results[kvPair.Key].Score;
					if (score != 0)
						row.CreateCell(cellCount).SetCellValue(score);

					row.Cells.ForEach(cell => cell.CellStyle = rowStyle);

					for (int i = 0; i < row.Cells.Count; i++)
						sheet.SetColumnWidth(i, 5000);


					row.Cells[0].CellStyle = headerStyle;
					if (row.Cells.Count > 5)
						row.Cells[5].CellStyle = headerStyle;
				}

				int eventEndGroup = rowCount;

				sheet.GroupRow(eventStartGroup + 1, eventEndGroup);

				rowCount++;
			}
		}

		private static void CreateAthleteProfiles(IWorkbook workbook, string sheetName, Team team)
		{
			var sheet = workbook.CreateSheet(sheetName);

			var boldFont = workbook.CreateFont();
			boldFont.FontName = "Calibri";
			boldFont.FontHeightInPoints = 11;
			boldFont.Boldweight = (short) FontBoldWeight.Bold;

			var headerStyle = (XSSFCellStyle) workbook.CreateCellStyle();
			headerStyle.FillPattern = FillPattern.SolidForeground;
			headerStyle.FillForegroundColor = IndexedColors.LightCornflowerBlue.Index;
			headerStyle.Alignment = HorizontalAlignment.Center;
			headerStyle.SetFont(boldFont);
			headerStyle.BorderBottom = BorderStyle.Thin;
			headerStyle.BorderTop = BorderStyle.Thin;
			headerStyle.BorderLeft = BorderStyle.Thin;
			headerStyle.BorderRight = BorderStyle.Thin;

			var rowStyle = (XSSFCellStyle) workbook.CreateCellStyle();
			//rowStyle.FillPattern = FillPattern.SolidForeground;
			//rowStyle.BorderBottom = BorderStyle.Thin;
			//rowStyle.BorderTop = BorderStyle.Thin;
			rowStyle.BorderLeft = BorderStyle.Thin;
			rowStyle.BorderRight = BorderStyle.Thin;

			int rowCount = 0;

			var rowHeaderTeams = sheet.CreateRow(rowCount++);
			var headerAddressRange = new CellRangeAddress(rowCount - 1, rowCount - 1, 4, 5);
			rowHeaderTeams.CreateCell(4).SetCellValue("DATA MAY BE INACCURATE");
			sheet.AddMergedRegion(headerAddressRange);
			rowHeaderTeams.Cells.ForEach(cell => cell.CellStyle = headerStyle);

			rowCount++;
			foreach (var athlete in team)
			{
				var athleteRow = sheet.CreateRow(rowCount++);
				athleteRow.CreateCell(0).SetCellValue(athlete.Name);
				CellRangeAddress cellRange = new CellRangeAddress(rowCount - 1, rowCount - 1, 0, 5);
				sheet.AddMergedRegion(cellRange);
				athleteRow.Cells.ForEach(cell => cell.CellStyle = headerStyle);
				var bestMarksRow = sheet.CreateRow(rowCount++);
				bestMarksRow.CreateCell(0).SetCellValue("All-Time Best Results");
				bestMarksRow.Cells.ForEach(cell => cell.CellStyle = headerStyle);

				int startAthleteOverall = rowCount;

				int startBestMarksRow = rowCount;
				foreach (var mark in athlete.BestMarks)
				{
					int cellCount = 1;
					var markRow = sheet.CreateRow(rowCount++);
					markRow.CreateCell(cellCount++).SetCellValue(mark.Event.Name);
					markRow.CreateCell(cellCount++).SetCellValue(mark.Result);
					markRow.CreateCell(cellCount++).SetCellValue(mark.Season);
					markRow.CreateCell(cellCount).SetCellValue(mark.Date);

					markRow.Cells.ForEach(cell => cell.CellStyle = rowStyle);
				}

				int endBestMarksRow = rowCount;
				sheet.GroupRow(startBestMarksRow, endBestMarksRow - 1);
				rowCount++;

				var seasonBestMarks = sheet.CreateRow(rowCount++);
				seasonBestMarks.CreateCell(0).SetCellValue("Season Best Results");
				seasonBestMarks.Cells.ForEach(cell => cell.CellStyle = headerStyle);

				int startSeasonMarksRow = rowCount;
				foreach (var season in athlete.SeasonBestMarks.Keys)
				{
					var seasonRow = sheet.CreateRow(rowCount++);
					seasonRow.CreateCell(1).SetCellValue($"Best Results - {season}");
					seasonRow.Cells.ForEach(cell => cell.CellStyle = headerStyle);

					int seasonMarksRowStart = rowCount;
					foreach (var mark in athlete.SeasonBestMarks[season])
					{
						int cellCount = 2;
						var markRow = sheet.CreateRow(rowCount++);
						markRow.CreateCell(cellCount++).SetCellValue(mark.Event.Name);
						markRow.CreateCell(cellCount++).SetCellValue(mark.Result);
						markRow.CreateCell(cellCount++).SetCellValue(mark.Season);
						markRow.CreateCell(cellCount).SetCellValue(mark.Date);

						markRow.Cells.ForEach(cell => cell.CellStyle = rowStyle);
					}

					int seasonMarkRowEnd = rowCount;
					sheet.GroupRow(seasonMarksRowStart, seasonMarkRowEnd - 1);
					rowCount++;
				}

				int endSeasonMarksRow = rowCount;
				sheet.GroupRow(startSeasonMarksRow, endSeasonMarksRow - 1);

				var allMarksRow = sheet.CreateRow(rowCount++);
				allMarksRow.CreateCell(0).SetCellValue("All Results");
				allMarksRow.Cells.ForEach(cell => cell.CellStyle = headerStyle);
				int startAllMarksRow = rowCount;
				foreach (var mark in athlete)
				{
					int cellCount = 1;
					var markRow = sheet.CreateRow(rowCount++);
					markRow.CreateCell(cellCount++).SetCellValue(mark.Event.Name);
					markRow.CreateCell(cellCount++).SetCellValue(mark.Result);
					markRow.CreateCell(cellCount++).SetCellValue(mark.Season);
					markRow.CreateCell(cellCount).SetCellValue(mark.Date);

					markRow.Cells.ForEach(cell => cell.CellStyle = rowStyle);
				}

				int endAllMarksRow = rowCount;
				sheet.GroupRow(startAllMarksRow, endAllMarksRow - 1);
				rowCount++;

				int endAthleteOverall = rowCount;
				sheet.GroupRow(startAthleteOverall - 1, endAthleteOverall);
				rowCount++;
			}

			for (int i = 0; i < 6; i++)
				sheet.SetColumnWidth(i, 6000);
		}
	}
}