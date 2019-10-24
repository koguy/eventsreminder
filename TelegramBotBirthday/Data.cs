using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TelegramBotBirthday.Primitives;

namespace TelegramBotBirthday
{
	public class Data
	{
		public static async Task<List<Event>> Events(int userId)
		{
			string json = null;
			using (StreamReader r = new StreamReader($"E:\\My\\Telegram\\TelegramBotBirthday\\TelegramBotBirthday\\Data\\{userId.ToString()}\\events.json"))
			{
				json = await r.ReadToEndAsync();
			}
			return string.IsNullOrEmpty(json) ? new List<Event>() : JsonConvert.DeserializeObject<List<Event>>(json);
		}

		public static async Task AddEvent(int userId, Event ev)
		{
			string rootPath = Path.Combine("E:\\My\\Telegram\\TelegramBotBirthday\\TelegramBotBirthday\\Data", userId.ToString());
			if (!Directory.Exists(rootPath))
				Directory.CreateDirectory(rootPath);

			string filePath = Path.Combine(rootPath, "events.json");
			if (!File.Exists(filePath))
				File.Create(filePath);
			 
			List<Event> events = await Events(userId);
			int maxId = 0;
			if (events.Count > 0)
				maxId = events.Max(o => o.Id);
			ev.Id = ++maxId;
			events.Add(ev);

			using (StreamWriter wr = new StreamWriter(filePath))
			{
				string json = JsonConvert.SerializeObject(events);
				await wr.WriteAsync(json);
			}
		}
		public static async Task DeleteEvent(int userId, int eventId)
		{
			string filePath = Path.Combine("E:\\My\\Telegram\\TelegramBotBirthday\\TelegramBotBirthday\\Data", userId.ToString(), "events.json");
			List<Event> events = await Events(userId);
			events.Remove(events.FirstOrDefault(o => o.Id == eventId));
			using (StreamWriter wr = new StreamWriter(filePath))
			{
				string json = JsonConvert.SerializeObject(events);
				await wr.WriteAsync(json);
			}
		}
		public static Month[] Months()
		{
			string json = null;
			using (StreamReader r = new StreamReader("E:\\My\\Telegram\\TelegramBotBirthday\\TelegramBotBirthday\\months.json"))
			{
				json = r.ReadToEnd();
			}
			return JsonConvert.DeserializeObject<Month[]>(json);
		}

		private static string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		private static string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}
	}
}
