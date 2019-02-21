using System;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;

namespace TfrrsCrawlerGUI
{
	public static class Helpers
	{
		public static string MyTrim(this string toTrim)
		{
			return toTrim.Trim().Replace("\n", string.Empty).Replace("\t", string.Empty);
		}

		public static HtmlDocument GetHtmlDoc(string url)
		{
			url = url.MyTrim();
			var httpClient = new HttpClient();
			var html = httpClient.GetStringAsync(url);
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(html.Result);
			return htmlDocument;
		}

		public static void AddRangeOverride<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
		{
			dicToAdd.ForEach(x => dic[x.Key] = x.Value);
		}

		public static void AddRangeNewOnly<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
		{
			dicToAdd.ForEach(x => { if (!dic.ContainsKey(x.Key)) dic.Add(x.Key, x.Value); });
		}

		public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
		{
			dicToAdd.ForEach(x => dic.Add(x.Key, x.Value));
		}

		public static bool ContainsKeys<TKey, TValue>(this Dictionary<TKey, TValue> dic, IEnumerable<TKey> keys)
		{
			bool result = false;
			keys.ForEachOrBreak((x) => { result = dic.ContainsKey(x); return result; });
			return result;
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
				action(item);
		}

		public static void ForEachOrBreak<T>(this IEnumerable<T> source, Func<T, bool> func)
		{
			foreach (var item in source)
			{
				bool result = func(item);
				if (result) break;
			}
		}
	}
}