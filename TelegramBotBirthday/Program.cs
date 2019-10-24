using System;

namespace TelegramBotBirthday
{
	using Newtonsoft.Json;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Telegram.Bot;
	using Telegram.Bot.Types.Enums;
	using Telegram.Bot.Types.ReplyMarkups;
	using static TelegramBotBirthday.Primitives;

	class Program
	{
		private static Dictionary<int, Event> eventsInProccess = new Dictionary<int, Event>();
		private static Status Status = Status.None;
		public static Month[] Months = Data.Months();
		public static TelegramBotClient BotClient;
		static void Main(string[] args)
		{
			BotClient = new TelegramBotClient("908247388:AAFQu3EmSNZEAujP3Avh4pCXFO98SzfWqkc");

			BotClient.OnMessage += OnMessageReceived;

			BotClient.OnCallbackQuery += OnCallbackQueryReceived;

			BotClient.StartReceiving();
			Console.ReadLine();
		}

		private static async void OnCallbackQueryReceived(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
		{
			string data = e.CallbackQuery.Data;
			int chatId = e.CallbackQuery.From.Id;
			Event currentEvent = eventsInProccess[chatId];
			if (Status == Status.WantToAdd)
			{
				if (Months.Any(o => string.Equals(o.Name, data, StringComparison.OrdinalIgnoreCase)))
				{
					Month month = Months.First(o => string.Equals(o.Name, data, StringComparison.OrdinalIgnoreCase));
					currentEvent.Month = month;
					Status = Status.MonthSelected;
					await BotClient.DeleteMessageAsync(chatId, e.CallbackQuery.Message.MessageId);
					await BotClient.SendTextMessageAsync(chatId, $"Окей. Месяц - {month.Name}. Теперь выберите число:", replyMarkup: Keyboards.Days(month));
				}
			}
			else if (Status == Status.MonthSelected)
			{
				currentEvent.Day = Int32.Parse(data);
				Status = Status.DaySelected;
				await BotClient.DeleteMessageAsync(chatId, e.CallbackQuery.Message.MessageId);
				await BotClient.SendTextMessageAsync(chatId, $"Отлично, {currentEvent.Month.Name} {currentEvent.Day}. Теперь напишите название события:");
				//eventsInProccess[chatId] = currentEvent;
			}
		}

		private static async void OnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
		{
			var message = e.Message;
			if (message == null || message.Type != MessageType.Text)
				return;

			int chatId = message.From.Id;

			switch (message.Text)
			{
				case Commands.Start:
					await BotClient.SendTextMessageAsync(chatId, "Круто", replyMarkup: Keyboards.Menu());
					break;
				case Commands.Add:
					if (eventsInProccess.ContainsKey(chatId))
						eventsInProccess[chatId] = new Event();
					else
						eventsInProccess.Add(chatId, new Event());
					Status = Status.WantToAdd;
					await BotClient.SendTextMessageAsync(chatId, "Выберите месяц", replyMarkup: Keyboards.Months());
					break;
				case Commands.List:
					var list = await Data.Events(chatId);
					await BotClient.SendTextMessageAsync(chatId, ListOfEvents(list));
					break;
				case Commands.EditList:
					var events = await Data.Events(chatId);
					Status = Status.WantToEditList;
					await BotClient.DeleteMessageAsync(chatId, message.MessageId);
					await BotClient.SendTextMessageAsync(chatId, ListOfEventsForEdit(events));
					break;
				default:
					if (Status == Status.DaySelected)
					{
						Event currentEvent = eventsInProccess[chatId];
						currentEvent.Name = message.Text;
						Status = Status.None;
						await Data.AddEvent(chatId, currentEvent);
						await BotClient.SendTextMessageAsync(chatId, $"Вы добавили {currentEvent.Name} на дату {currentEvent.Day} {currentEvent.Month.Name}.");
					}
					else if (Status == Status.WantToEditList)
					{
						int id = GetEventId(message.Text);
						await Data.DeleteEvent(chatId, id);
						await BotClient.SendTextMessageAsync(chatId, "Event deleted.");
					}
					break;
			}
		}

		private static int GetEventId(string deletedEvent)
		{
			string index = deletedEvent.Substring(7);
			return Int32.Parse(index);
		}

		private static string ListOfEvents(List<Event> events)
		{
			StringBuilder result = new StringBuilder();
			result.Append("Список событий:");
			foreach(var item in events)
			{
				result.Append($"\n{item.Id}. {item.Month.Name} {item.Day} - {item.Name}");
			}
			result.Append($"\n{Commands.EditList}");
			return result.ToString();
		}

		private static string ListOfEventsForEdit(List<Event> events)
		{
			StringBuilder result = new StringBuilder();
			result.Append("Список событий:");
			foreach (var item in events)
			{
				result.Append($"\n{item.Id}. {item.Month.Name} {item.Day} - {item.Name} ({String.Format(Commands.DeleteEvent, item.Id)})");
			}
			return result.ToString();
		}
	}
}
