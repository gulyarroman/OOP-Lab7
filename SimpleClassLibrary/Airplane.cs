namespace SimpleClassLibrary;

/// <summary>Рейс літака: міста та дати відправлення і прибуття, дальність польоту.</summary>
public class Airplane
{
    public const double MetersPerKilometer = 1000;
    public const double KilometersPerMile = 1.609344;

    private string departureCity;
    private string arrivalCity;
    private Date departureDate;
    private Date arrivalDate;
    private double distanceKm;

    /// <summary>Конструктор за замовчуванням: невідомий маршрут, відправлення і прибуття - зараз.</summary>
    public Airplane()
        : this("Невідомо", "Невідомо")
    {
    }

    /// <summary>Маршрут без розкладу: відправлення і прибуття - поточний момент.</summary>
    public Airplane(string departureCity, string arrivalCity)
        : this(departureCity, arrivalCity, new Date(), new Date())
    {
    }

    public Airplane(string departureCity, string arrivalCity, Date departureDate, Date arrivalDate)
        : this(departureCity, arrivalCity, departureDate, arrivalDate, 0)
    {
    }

    public Airplane(string departureCity, string arrivalCity, Date departureDate, Date arrivalDate, double distanceKm)
    {
        this.departureCity = ValidateCity(departureCity);
        this.arrivalCity = ValidateCity(arrivalCity);
        this.departureDate = departureDate ?? throw new ArgumentException("дата відправлення не задана");
        this.arrivalDate = arrivalDate ?? throw new ArgumentException("дата прибуття не задана");
        this.distanceKm = ValidateDistance(distanceKm);

        if (arrivalDate.ToDateTime() < departureDate.ToDateTime())
            throw new ArgumentException("прибуття не може бути раніше за відправлення");
    }

    /// <summary>Конструктор копіювання (глибоке копіювання: дати теж копіюються).</summary>
    public Airplane(Airplane other)
        : this(other.departureCity, other.arrivalCity, new Date(other.departureDate), new Date(other.arrivalDate),
               other.distanceKm)
    {
    }

    public string DepartureCity
    {
        get => departureCity;
        set => departureCity = ValidateCity(value);
    }

    public string ArrivalCity
    {
        get => arrivalCity;
        set => arrivalCity = ValidateCity(value);
    }

    public Date DepartureDate
    {
        get => departureDate;
        set => departureDate = value ?? throw new ArgumentException("дата відправлення не задана");
    }

    public Date ArrivalDate
    {
        get => arrivalDate;
        set => arrivalDate = value ?? throw new ArgumentException("дата прибуття не задана");
    }

    /// <summary>Дальність польоту в кілометрах (саме в них дальність зберігається).</summary>
    public double DistanceKilometers
    {
        get => distanceKm;
        set => distanceKm = ValidateDistance(value);
    }

    /// <summary>Дальність польоту в метрах.</summary>
    public double DistanceMeters
    {
        get => distanceKm * MetersPerKilometer;
        set => distanceKm = ValidateDistance(value / MetersPerKilometer);
    }

    /// <summary>Дальність польоту в милях.</summary>
    public double DistanceMiles
    {
        get => distanceKm / KilometersPerMile;
        set => distanceKm = ValidateDistance(value * KilometersPerMile);
    }

    /// <summary>Задає дальність польоту в обраних одиницях вимірювання.</summary>
    public void SetDistance(double value, DistanceUnit unit)
    {
        switch (unit)
        {
            case DistanceUnit.Kilometers: DistanceKilometers = value; break;
            case DistanceUnit.Meters: DistanceMeters = value; break;
            case DistanceUnit.Miles: DistanceMiles = value; break;
            default: throw new ArgumentException($"невідома одиниця вимірювання {unit}");
        }
    }

    /// <summary>Повертає дальність польоту в обраних одиницях вимірювання.</summary>
    public double GetDistance(DistanceUnit unit) => unit switch
    {
        DistanceUnit.Kilometers => DistanceKilometers,
        DistanceUnit.Meters => DistanceMeters,
        DistanceUnit.Miles => DistanceMiles,
        _ => throw new ArgumentException($"невідома одиниця вимірювання {unit}")
    };

    /// <summary>Сумарний час подорожі у хвилинах.</summary>
    public int GetTotalTime() => (int)(arrivalDate.ToDateTime() - departureDate.ToDateTime()).TotalMinutes;

    /// <summary>true, якщо відправлення і прибуття відбуваються в той самий день.</summary>
    public bool IsArrivingToday() => departureDate.IsSameDay(arrivalDate);

    public override string ToString() =>
        $"{departureCity} -> {arrivalCity}, {departureDate} - {arrivalDate}, {distanceKm:0.###} км";

    private static string ValidateCity(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("назва міста не може бути порожньою");
        return value.Trim();
    }

    private static double ValidateDistance(double kilometers)
    {
        if (double.IsNaN(kilometers) || kilometers < 0 || kilometers > 50_000)
            throw new ArgumentException("дальність польоту має бути від 0 до 50 000 км");
        return kilometers;
    }
}
