using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using static TelegramBotBirthday.Primitives;

namespace TelegramBotBirthday
{
	public static class Keyboards
	{
		public static Month[] MonthList = Data.Months();
		public static ReplyKeyboardMarkup Menu()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton(Commands.Add),
				new KeyboardButton(Commands.List)
			}, true, true);
		}

		public static InlineKeyboardMarkup Months()
		{
			List<List<InlineKeyboardButton>> result = new List<List<InlineKeyboardButton>>();
			List<InlineKeyboardButton> insideResult = new List<InlineKeyboardButton>();
			int count = MonthList.Length;
			int columnAmount = 3;
			for (int i = 1; i <= count; i++)
			{
				string monthName = MonthList[i - 1].Name;
				insideResult.Add(InlineKeyboardButton.WithCallbackData(monthName));
				if (i % columnAmount == 0 || i == count)
				{
					result.Add(insideResult);
					insideResult = new List<InlineKeyboardButton>();
				}
			}
			return new InlineKeyboardMarkup(result);
		}

		public static InlineKeyboardMarkup Days(Month month)
		{
			List<List<InlineKeyboardButton>> result = new List<List<InlineKeyboardButton>>();
			List<InlineKeyboardButton> insideResult = new List<InlineKeyboardButton>();
			int count = month.AmountOfDays;
			int columnAmount = 7;
			for (int i = 1; i <= count; i++)
			{
				insideResult.Add(InlineKeyboardButton.WithCallbackData(i.ToString()));
				if (i % columnAmount == 0 || i == count)
				{
					result.Add(insideResult);
					insideResult = new List<InlineKeyboardButton>();
				}
			}
			return new InlineKeyboardMarkup(result);
		}
	}
}
