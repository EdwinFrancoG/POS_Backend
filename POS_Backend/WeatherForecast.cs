namespace POS_Backend
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }
        //hola mundo

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}
