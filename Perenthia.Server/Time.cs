using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	/// <summary>
	/// Provides static methods and properties for Time within the virtual world.
	/// </summary>
	public static class Time
	{
		//internal static DateTimeFormatInfo FormatInfo;

		static Time()
		{
			//FormatInfo = new DateTimeFormatInfo();
			//FormatInfo.AbbreviatedDayNames = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
			//FormatInfo.ShortestDayNames = FormatInfo.AbbreviatedDayNames;
			//FormatInfo.MonthNames = new string[] { "Halis", "Ebrum", "Dener", "Kilvas", "Howden", "Brimf", "Ovast", "Ferden", "Eckrin", "Wendis", "Retfer", "Vunder", "" };
			//FormatInfo.MonthGenitiveNames = FormatInfo.MonthNames;
			//FormatInfo.AbbreviatedMonthGenitiveNames = FormatInfo.MonthNames;
			//FormatInfo.AbbreviatedMonthNames = FormatInfo.MonthNames;
			//FormatInfo.AMDesignator = "AM";
			//FormatInfo.PMDesignator = "PM";
			//FormatInfo.Calendar = new PerenthiaCalendar();
			//FormatInfo.CalendarWeekRule = CalendarWeekRule.FirstDay;
			//FormatInfo.DateSeparator = "/";
			//FormatInfo.DayNames = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
			//FormatInfo.FirstDayOfWeek = DayOfWeek.Sunday;
			//FormatInfo.FullDateTimePattern = "hh:mm tt, d MMMM yyyy";
			//FormatInfo.LongDatePattern = "d MMMM yyyy";
			//FormatInfo.LongTimePattern = "HH:mm:ss";
			//FormatInfo.MonthDayPattern = "m/d";
			//FormatInfo.ShortDatePattern = "m/d/yyyy";
			//FormatInfo.ShortTimePattern = "HH:mm";
			//FormatInfo.TimeSeparator = ":";
			//FormatInfo.YearMonthPattern = "m/yyyy";
		}

		public static void Initialize(double startTime, int pulseYear, string[] monthNames, string[] hourNames)
		{
			Time.StartTime = startTime;
			Time.PulseYear = pulseYear;
			if (monthNames != null && monthNames.Length > 0)
				Time.MonthNames = monthNames;
			if (hourNames != null && hourNames.Length > 0)
				Time.WatchNames = hourNames;
		}

		/// <summary>
		/// This is the base start time that all time
		/// checks should be based on. The number of seconds
		/// of the date 8/20/1975 8:00 AM in UTC time.
		/// </summary>
		internal static double StartTime = 62313364800.0;// 8/20/1975 8:00 AM  /////63110181600.0; // 11/18/2000 17:00:00.000

		/// <summary>
		/// The pulse amount for the year calculation.
		/// </summary>
		/// <remarks>90 real days == 1 game year</remarks>
		internal static int PulseYear = 7776000;

		/// <summary>
		/// The pulse amount for the month calculation.
		/// </summary>
		/// <remarks>12 months in a year.</remarks>
		internal static int PulseMonth = PulseYear / 12;

		/// <summary>
		/// The pulse amount for the day calculation.
		/// </summary>
		/// <remarks>28 days in a month.</remarks>
		internal static int PulseDay = PulseMonth / 28;

		/// <summary>
		/// The pulse amount for the hour calculation.
		/// </summary>
		/// <remarks>24 hours in a day.</remarks>
		internal static int PulseHour = PulseDay / 24;

		/// <summary>
		/// The pulse amount for the minute calculation.
		/// </summary>
		/// <remarks>60 minutes in an hour.</remarks>
		internal static int PulseMinute = PulseHour / 60;

		/// <summary>
		/// Gets a list of Month Names.
		/// </summary>
		public static string[] MonthNames = new string[]
		{
			"Halis",
			"Ebrum",
			"Dener",
			"Kilvas",
			"Howden",
			"Brimf",
			"Ovast",
			"Ferden", 
			"Eckrin",
			"Wendis", 
			"Retfer",
			"Vunder"
		};

		/// <summary>
		/// Gets a list of Names of the Watch Hours.
		/// </summary>
		public static string[] WatchNames = new string[]
		{
			"Second Watch",
			"Third Watch",
			"Third Watch",
			"Third Watch",
			"Last Watch",
			"Last Watch",
			"Last Watch",
			"First Bells",
			"First Bells",
			"First Bells",
			"First Bells",
			"Second Bells",
			"Second Bells",
			"Second Bells",
			"Second Bells",
			"Third Bells",
			"Third Bells",
			"Third Bells",
			"Third Bells",
			"First Watch",
			"First Watch",
			"First Watch",
			"Second Watch",
			"Second Watch"
		};

		private static string GetAmPm(int hours)
		{
			if (hours > 12) return "PM";
			return "AM";
		}

		/// <summary>
		/// Formats the hours from 24 hour to 12 hour format.
		/// </summary>
		/// <param name="hours">Hours value to format.</param>
		/// <returns>A 12-hour formatted string for the provided 
		/// hour value.</returns>
		public static string FormatHours(int hours)
		{
			// If it's 0 then this is midnight
			if (hours == 0)
			{
				return "12";
			}
			else
			{
				if (hours > 12)
				{
					// Greater than 12 so format it to 12-hour format.
					return Convert.ToString(hours - 12).PadLeft(2, '0');
				}
				else
				{
					// Do nothing but return the string.
					return hours.ToString().PadLeft(2, '0');
				}
			}
		}

		/// <summary>
		/// Gets the 24 hour value of the current Hours value of the current game time.
		/// </summary>
		/// <returns></returns>
		public static int GetTwentyFourHourValue()
		{
			// Get the number of seconds in which to 
			// run the date calculations.
			int remainder = Convert.ToInt32(new TimeSpan(DateTime.Now.ToUniversalTime().Ticks).TotalSeconds - StartTime);

			int years, months, days, hours, minutes;
			SetTimeValues(remainder, out years, out months, out days, out hours, out minutes);

			return hours;
		}

		public static TimeSpan GetTime()
		{
			// Get the number of seconds in which to 
			// run the date calculations.
			int remainder = Convert.ToInt32(new TimeSpan(DateTime.Now.ToUniversalTime().Ticks).TotalSeconds - StartTime);

			int years, months, days, hours, minutes;
			SetTimeValues(remainder, out years, out months, out days, out hours, out minutes);

			return new TimeSpan(hours, minutes, 0);
		}

		public static RdlProperty GetTimeProperty()
		{
			return new RdlProperty(0, "Time", Time.GetTimeString());
		}

		/// <summary>
		/// Gets the current year, month, day and time represented
		/// as a string.
		/// </summary>
		/// <returns>Current year, month, day and time.</returns>
		public static string GetTimeString()
		{
			StringBuilder sb = new StringBuilder();

			// Get the number of seconds in which to 
			// run the date calculations.
			int remainder = Convert.ToInt32(new TimeSpan(DateTime.Now.ToUniversalTime().Ticks).TotalSeconds - StartTime);

			int years, months, days, hours, minutes;
			SetTimeValues(remainder, out years, out months, out days, out hours, out minutes);

			// Use this format:
			// 1:25 AM, Day 4 of Ebrum, Year 234 of the New Era.
			sb.Append(FormatHours(hours));
			sb.Append(":");
			sb.Append(minutes.ToString().PadLeft(2, '0'));
			sb.Append(" ");
			sb.Append(GetAmPm(hours));
			sb.Append(", Day ");
			sb.Append((days + 1).ToString());
			sb.Append(" of ");
			sb.Append(MonthNames[months]);
			sb.Append(", Year ");
			sb.Append(years.ToString());
			sb.Append(" of the New Era");

			return sb.ToString();
		}

		private static void SetTimeValues(int totalSeconds, out int years, out int months, out int days, out int hours, out int minutes)
		{
			int remainder = 0;
			// Year
			years = totalSeconds / PulseYear;
			remainder = totalSeconds % PulseYear;

			// Month
			months = remainder / PulseMonth;
			remainder = remainder % PulseMonth;

			// Day
			days = remainder / PulseDay;
			remainder = remainder % PulseDay;

			// Hour
			hours = remainder / PulseHour;
			remainder = remainder % PulseHour;

			// Minute
			minutes = remainder / PulseMinute;
		}
	}

	public class PerenthiaCalendar : Calendar
	{
		/// <summary>
		/// This is the base start time that all time
		/// checks should be based on. The number of ticks
		/// of the date 8/20/1975 8:00 AM in UTC time.
		/// </summary>
		internal static long StartTime = 623133504000000000;// 8/20/1975 8:00 AM

		/// <summary>
		/// The pulse amount for the year calculation.
		/// </summary>
		/// <remarks>90 real days == 1 game year</remarks>
		internal static int PulseYear = 7776000;

		/// <summary>
		/// The pulse amount for the month calculation.
		/// </summary>
		/// <remarks>12 months in a year.</remarks>
		internal static int PulseMonth = PulseYear / 12;

		/// <summary>
		/// The pulse amount for the day calculation.
		/// </summary>
		/// <remarks>28 days in a month.</remarks>
		internal static int PulseDay = PulseMonth / 28;

		/// <summary>
		/// The pulse amount for the hour calculation.
		/// </summary>
		/// <remarks>24 hours in a day.</remarks>
		internal static int PulseHour = PulseDay / 24;

		/// <summary>
		/// The pulse amount for the minute calculation.
		/// </summary>
		/// <remarks>60 minutes in an hour.</remarks>
		internal static int PulseMinute = PulseHour / 60;

		public override DateTime AddMonths(DateTime time, int months)
		{
			return this.AddDays(time, months * 28);
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			return this.AddMonths(time, years * 12);
		}

		public override int[] Eras
		{
			get { return new int[] { 1 }; }
		}

		public override int GetDayOfMonth(DateTime time)
		{
			return this.GetDatePart(time.ToUniversalTime().Ticks, 2);
		}

		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			return (DayOfWeek)(((int)((time.ToUniversalTime().Ticks / 0xc92a69c000L) + 1L)) % 7);
		}

		public override int GetDayOfYear(DateTime time)
		{
			return this.GetDatePart(time.ToUniversalTime().Ticks, 2);
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			return 28;
		}

		public override int GetDaysInYear(int year, int era)
		{
			return 336;
		}

		public override int GetEra(DateTime time)
		{
			return 1;
		}

		public override int GetMonth(DateTime time)
		{
			return this.GetDatePart(time.ToUniversalTime().Ticks, 1);
		}

		public override int GetMonthsInYear(int year, int era)
		{
			return 12;
		}

		public override int GetYear(DateTime time)
		{
			return this.GetDatePart(time.ToUniversalTime().Ticks, 0);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			return false;
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			return false;
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			DateTime dt = new DateTime(year, month, day, hour, minute, second, millisecond);
			return new DateTime(dt.ToUniversalTime().Ticks - StartTime);
		}

		private int GetDatePart(long ticks, int part)
		{
			int totalSeconds = (int)(new TimeSpan(ticks - StartTime).TotalSeconds);
			int remainder = 0;
			int years, months, days, hours, minutes;

			// Year
			years = totalSeconds / PulseYear;
			remainder = totalSeconds % PulseYear;
			if (part == 0) return years;

			// Month
			months = remainder / PulseMonth;
			remainder = remainder % PulseMonth;
			if (part == 1) return months;

			// Day
			days = remainder / PulseDay;
			remainder = remainder % PulseDay;
			if (part == 2) return days;

			// Hour
			hours = remainder / PulseHour;
			remainder = remainder % PulseHour;
			if (part == 3) return hours;

			// Minute
			minutes = remainder / PulseMinute;
			return minutes;
		}
	}

	//public class PerenthiaCultureInfo : CultureInfo
	//{
	//    internal static DateTimeFormatInfo FormatInfo;

	//    static PerenthiaCultureInfo()
	//    {
	//        FormatInfo = new DateTimeFormatInfo();
	//        FormatInfo.AbbreviatedDayNames = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
	//        FormatInfo.ShortestDayNames = FormatInfo.AbbreviatedDayNames;
	//        FormatInfo.MonthNames = new string[] { "Halis", "Ebrum", "Dener", "Kilvas", "Howden", "Brimf", "Ovast", "Ferden", "Eckrin", "Wendis", "Retfer", "Vunder", "" };
	//        FormatInfo.MonthGenitiveNames = FormatInfo.MonthNames;
	//        FormatInfo.AbbreviatedMonthGenitiveNames = FormatInfo.MonthNames;
	//        FormatInfo.AbbreviatedMonthNames = FormatInfo.MonthNames;
	//        FormatInfo.AMDesignator = "AM";
	//        FormatInfo.PMDesignator = "PM";
	//        //FormatInfo.Calendar = new PerenthiaCalendar();
	//        FormatInfo.CalendarWeekRule = CalendarWeekRule.FirstDay;
	//        FormatInfo.DateSeparator = "/";
	//        FormatInfo.DayNames = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
	//        FormatInfo.FirstDayOfWeek = DayOfWeek.Sunday;
	//        FormatInfo.FullDateTimePattern = "hh:mm tt, d MMMM yyyy";
	//        FormatInfo.LongDatePattern = "d MMMM yyyy";
	//        FormatInfo.LongTimePattern = "HH:mm:ss";
	//        FormatInfo.MonthDayPattern = "m/d";
	//        FormatInfo.ShortDatePattern = "m/d/yyyy";
	//        FormatInfo.ShortTimePattern = "HH:mm";
	//        FormatInfo.TimeSeparator = ":";
	//        FormatInfo.YearMonthPattern = "m/yyyy";
	//    }
	//}
}
