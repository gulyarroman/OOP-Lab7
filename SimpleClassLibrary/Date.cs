namespace SimpleClassLibrary;

/// <summary>Дата і час з точністю до хвилин.</summary>
public class Date
{
    private int year;
    private int month;
    private int day;
    private int hours;
    private int minutes;

    /// <summary>Конструктор за замовчуванням: поточні дата і час.</summary>
    public Date()
        : this(DateTime.Now)
    {
    }

    public Date(DateTime value)
        : this(value.Year, value.Month, value.Day, value.Hour, value.Minute)
    {
    }

    /// <summary>Дата без часу (00:00).</summary>
    public Date(int year, int month, int day)
        : this(year, month, day, 0, 0)
    {
    }

    public Date(int year, int month, int day, int hours, int minutes)
    {
        // рік і місяць задаються першими, бо від них залежить допустимий день
        Year = year;
        Month = month;
        Day = day;
        Hours = hours;
        Minutes = minutes;
    }

    /// <summary>Конструктор копіювання.</summary>
    public Date(Date other)
        : this(other.year, other.month, other.day, other.hours, other.minutes)
    {
    }

    public int Year
    {
        get => year;
        set
        {
            if (value < 1900 || value > 2100)
                throw new ArgumentException($"рік {value} поза межами 1900-2100");
            if (month != 0 && day > DateTime.DaysInMonth(value, month))
                throw new ArgumentException($"у {value} році немає {day}.{month:D2}");
            year = value;
        }
    }

    public int Month
    {
        get => month;
        set
        {
            if (value < 1 || value > 12)
                throw new ArgumentException($"місяць {value} поза межами 1-12");
            if (day > DateTime.DaysInMonth(year, value))
                throw new ArgumentException($"у місяці {value} немає {day}-го дня");
            month = value;
        }
    }

    public int Day
    {
        get => day;
        set
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (value < 1 || value > daysInMonth)
                throw new ArgumentException($"день {value} неможливий: у місяці {month} року {year} лише {daysInMonth} дн.");
            day = value;
        }
    }

    public int Hours
    {
        get => hours;
        set
        {
            if (value < 0 || value > 23)
                throw new ArgumentException($"години {value} поза межами 0-23");
            hours = value;
        }
    }

    public int Minutes
    {
        get => minutes;
        set
        {
            if (value < 0 || value > 59)
                throw new ArgumentException($"хвилини {value} поза межами 0-59");
            minutes = value;
        }
    }

    public DateTime ToDateTime() => new(year, month, day, hours, minutes, 0);

    public bool IsSameDay(Date other) => year == other.year && month == other.month && day == other.day;

    public override string ToString() => $"{day:D2}.{month:D2}.{year} {hours:D2}:{minutes:D2}";
}
