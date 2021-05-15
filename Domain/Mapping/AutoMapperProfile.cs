using AutoMapper;
using Domain.Models;

namespace Domain.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<WeatherForecast, WeatherForecastDTO>();
        }
    }
}