using System;

namespace Domain.Models
{
    public class WeatherForecastDTO
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }
    }
}
