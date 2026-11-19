using System.Globalization;
using System.Text;
using SimpleClassLibrary;

namespace FlightsApp;

internal class Program
{
    private const string DateHint = "дд.мм.рррр гг:хх";
    private static Airplane[] flights = [];

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        // UTF-16 input makes .NET read the console with ReadConsoleW, so і, ї, є, ґ are not lost
        if (!Console.IsInputRedirected)
            Console.InputEncoding = Encoding.Unicode;
        Console.Title = "Рейси літаків: бібліотека класів (варіант 2)";

        while (true)
        {
            if (!Console.IsOutputRedirected)
                Console.Clear();
            PrintMenu();
            int choice = ReadInt("Ваш вибір: ", 0, 5);
            if (choice == 0)
                return;

            Console.WriteLine();
            switch (choice)
            {
                case 1: flights = ReadAirplanes(); break;
                case 2: PrintAllAirplanes(flights); break;
                case 3: ShowAirplaneByNumber(); break;
                case 4: PrintDistances(flights); break;
                case 5: DemonstrateConstructors(); break;
            }

            Console.WriteLine();
            Console.Write("Натисніть Enter, щоб повернутися до меню...");
            Console.ReadLine();
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("Рейси літаків: класи з бібліотеки SimpleClassLibrary (варіант 2)");
        Console.WriteLine($"Рейсів у масиві: {flights.Length}");
        Console.WriteLine(new string('-', 64));
        Console.WriteLine(" 1 - Ввести масив рейсів з клавіатури");
        Console.WriteLine(" 2 - Показати всі рейси");
        Console.WriteLine(" 3 - Показати рейс за номером");
        Console.WriteLine(" 4 - Дальність польоту рейсів у кілометрах, метрах та милях");
        Console.WriteLine(" 5 - Демонстрація конструкторів і копіювання об'єктів");
        Console.WriteLine(" 0 - Вихід");
        Console.WriteLine(new string('-', 64));
    }

    /// <summary>Читає з клавіатури n рейсів і повертає масив об'єктів Airplane.</summary>
    private static Airplane[] ReadAirplanes()
    {
        int n = ReadInt("Кількість рейсів n = ", 1, 10);
        var result = new Airplane[n];
        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"Рейс №{i + 1}:");
            string from = ReadText("   Місто відправлення: ");
            string to = ReadText("   Місто прибуття: ");
            Date departure = ReadDate($"   Відправлення ({DateHint}): ");
            while (true)
            {
                Date arrival = ReadDate($"   Прибуття ({DateHint}): ");
                try
                {
                    result[i] = new Airplane(from, to, departure, arrival);
                    break;
                }
                catch (ArgumentException ex)
                {
                    WriteInputError(ex.Message);
                }
            }
            ReadDistance(result[i]);
        }

        WriteColored($"Сформовано масив з {n} рейсів.", ConsoleColor.Green);
        return result;
    }

    /// <summary>Вводить дальність польоту в одиницях, обраних користувачем через меню.</summary>
    private static void ReadDistance(Airplane airplane)
    {
        DistanceUnit unit = (DistanceUnit)ReadInt("   Одиниці дальності (1 - кілометри, 2 - метри, 3 - милі): ", 1, 3);
        while (true)
        {
            Console.Write($"   Дальність польоту, {UnitName(unit)}: ");
            string text = (Console.ReadLine() ?? string.Empty).Trim().Replace(',', '.');
            if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                WriteInputError("потрібне число");
                continue;
            }

            try
            {
                airplane.SetDistance(value, unit);
                return;
            }
            catch (ArgumentException ex)
            {
                WriteInputError(ex.Message);
            }
        }
    }

    /// <summary>Виводить дальність польоту кожного рейсу в усіх трьох одиницях вимірювання.</summary>
    private static void PrintDistances(Airplane[] airplanes)
    {
        if (airplanes.Length == 0)
        {
            WriteColored("Масив рейсів порожній. Спочатку введіть дані (пункт 1).", ConsoleColor.Yellow);
            return;
        }

        Console.WriteLine($"{"№",3}  {"Маршрут",-26}{"Кілометри",12}{"Метри",14}{"Милі",12}");
        Console.WriteLine(new string('-', 67));
        for (int i = 0; i < airplanes.Length; i++)
        {
            Airplane a = airplanes[i];
            Console.WriteLine($"{i + 1,3}  {a.DepartureCity + " -> " + a.ArrivalCity,-26}" +
                              $"{a.GetDistance(DistanceUnit.Kilometers),12:N1}" +
                              $"{a.GetDistance(DistanceUnit.Meters),14:N0}" +
                              $"{a.GetDistance(DistanceUnit.Miles),12:N1}");
        }
        Console.WriteLine(new string('-', 67));
        Console.WriteLine($"1 миля = {Airplane.KilometersPerMile} км, 1 км = {Airplane.MetersPerKilometer} м");
    }

    /// <summary>Виводить на екран поля одного рейсу та результати його методів.</summary>
    private static void PrintAirplane(Airplane airplane, int number)
    {
        Console.WriteLine($"Рейс №{number}: {airplane.DepartureCity} -> {airplane.ArrivalCity}");
        PrintField("Місто відправлення (DepartureCity)", airplane.DepartureCity);
        PrintField("Місто прибуття (ArrivalCity)", airplane.ArrivalCity);
        PrintField("Відправлення (DepartureDate)", airplane.DepartureDate);
        PrintField("Прибуття (ArrivalDate)", airplane.ArrivalDate);
        PrintField("Дальність (DistanceKilometers)", $"{airplane.DistanceKilometers:N1} км");
        PrintField("Дальність (DistanceMeters)", $"{airplane.DistanceMeters:N0} м");
        PrintField("Дальність (DistanceMiles)", $"{airplane.DistanceMiles:N1} миль");
        PrintField("Час у дорозі (GetTotalTime)", FormatMinutes(airplane.GetTotalTime()));
        PrintField("Прибуття в той же день (IsArrivingToday)", airplane.IsArrivingToday() ? "так" : "ні");
        PrintField("ToString()", airplane);
    }

    private static void PrintField(string label, object value) =>
        Console.WriteLine($"   {label + ":",-44}{value}");

    /// <summary>Виводить на екран усі рейси масиву у вигляді таблиці.</summary>
    private static void PrintAllAirplanes(Airplane[] airplanes)
    {
        if (airplanes.Length == 0)
        {
            WriteColored("Масив рейсів порожній. Спочатку введіть дані (пункт 1).", ConsoleColor.Yellow);
            return;
        }

        Console.WriteLine($"{"№",3}  {"Звідки",-12}{"Куди",-12}{"Відправлення",-18}{"Прибуття",-18}" +
                          $"{"Час, хв",8}{"Дальн., км",12}  Той же день");
        Console.WriteLine(new string('-', 98));
        for (int i = 0; i < airplanes.Length; i++)
        {
            Airplane a = airplanes[i];
            Console.WriteLine($"{i + 1,3}  {a.DepartureCity,-12}{a.ArrivalCity,-12}{a.DepartureDate,-18}" +
                              $"{a.ArrivalDate,-18}{a.GetTotalTime(),8}{a.DistanceKilometers,12:N1}  " +
                              $"{(a.IsArrivingToday() ? "так" : "ні")}");
        }
        Console.WriteLine(new string('-', 98));
    }

    private static void ShowAirplaneByNumber()
    {
        if (flights.Length == 0)
        {
            WriteColored("Масив рейсів порожній. Спочатку введіть дані (пункт 1).", ConsoleColor.Yellow);
            return;
        }

        int number = ReadInt($"Номер рейсу (1-{flights.Length}): ", 1, flights.Length);
        Console.WriteLine();
        PrintAirplane(flights[number - 1], number);
    }

    private static void DemonstrateConstructors()
    {
        Console.WriteLine("Конструктори класу Airplane (клас підключено з бібліотеки SimpleClassLibrary):");
        PrintCtor("new Airplane()", new Airplane());
        PrintCtor("new Airplane(\"Київ\", \"Львів\")", new Airplane("Київ", "Львів"));
        var london = new Airplane("Київ", "Лондон", new Date(2026, 12, 1, 7, 0), new Date(2026, 12, 1, 10, 30), 2134);
        PrintCtor("new Airplane(5 параметрів)", london);
        var copy = new Airplane(london);
        PrintCtor("new Airplane(london)", copy, "   (копіювання)");

        Console.WriteLine();
        Console.WriteLine("Зміна дальності копії в милях не впливає на оригінал:");
        copy.SetDistance(1000, DistanceUnit.Miles);
        Console.WriteLine($"   copy.SetDistance(1000, Miles) -> copy: {copy.DistanceKilometers:N1} км, " +
                          $"london: {london.DistanceKilometers:N1} км");

        Console.WriteLine();
        Console.WriteLine("Перевірка коректності даних:");
        TryShow("london.DistanceMeters = -500", () => london.DistanceMeters = -500);
        TryShow("new Date(2026, 2, 30)", () => new Date(2026, 2, 30));
    }

    private static void PrintCtor(string call, object created, string note = "") =>
        Console.WriteLine($"   {call,-31} -> {created}{note}");

    private static void TryShow(string description, Action action)
    {
        Console.Write($"   {description}: ");
        try
        {
            action();
            WriteColored("успішно", ConsoleColor.Green);
        }
        catch (ArgumentException ex)
        {
            WriteColored($"помилка - {ex.Message}", ConsoleColor.Red);
        }
    }

    private static string UnitName(DistanceUnit unit) => unit switch
    {
        DistanceUnit.Kilometers => "км",
        DistanceUnit.Meters => "м",
        DistanceUnit.Miles => "миль",
        _ => unit.ToString()
    };

    private static string FormatMinutes(int minutes) => $"{minutes} хв ({minutes / 60} год {minutes % 60:D2} хв)";

    private static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                return value;

            WriteInputError($"потрібне ціле число від {min} до {max}");
        }
    }

    private static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string text = (Console.ReadLine() ?? string.Empty).Trim();
            if (text.Length > 0 && text.Length <= 30)
                return text;

            WriteInputError("назва має містити від 1 до 30 символів");
        }
    }

    private static Date ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string text = (Console.ReadLine() ?? string.Empty).Trim();
            if (!DateTime.TryParseExact(text, ["dd.MM.yyyy HH:mm", "d.M.yyyy H:mm"], CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out DateTime value))
            {
                WriteInputError($"некоректна дата або час, потрібен формат {DateHint}");
                continue;
            }

            try
            {
                return new Date(value.Year, value.Month, value.Day, value.Hour, value.Minute);
            }
            catch (ArgumentException ex)
            {
                WriteInputError(ex.Message);
            }
        }
    }

    private static void WriteInputError(string message) =>
        WriteColored($"     Помилка введення: {message}. Повторіть введення.", ConsoleColor.Yellow);

    private static void WriteColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
