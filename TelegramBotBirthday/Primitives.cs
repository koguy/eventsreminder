using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotBirthday
{
	public class Primitives
	{
		public const string DeleteEmoji = "\xE2";
		public const string EditEmoji = "\u270F";
		public struct Commands
		{
			public const string Add = "Add event";
			public const string List = "List of events";
			public const string EditList = "/Edit";
			public const string DeleteEvent = "/Delete{0}";
			public const string Start = "/start";
		}
		public class Event
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public int? Year { get; set; }
			public Month Month { get; set; }
			public int Day { get; set; }
		}
		public class Month
		{
			public int Number { get; private set; }
			public string Name { get; private set; }
			public int AmountOfDays { get; private set; }

			public Month(int number, string name, int amountOfDays)
			{
				Number = number;
				Name = name;
				AmountOfDays = amountOfDays;
			}
		}

		public enum Status
		{
			None,
			WantToAdd,
			MonthSelected,
			DaySelected,
			WantToEditList
		}
	}
}
