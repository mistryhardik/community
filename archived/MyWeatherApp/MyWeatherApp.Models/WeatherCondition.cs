using System.Drawing;

namespace MyWeatherApp.Models
{
    public enum WeatherConditionType
    {
        Thunderstrom,
        Drizzle,
        Rain,
        Snow,
        Mist,
        Smoke,
        Haze,
        Sand,
        Dust_Whrils,
        Fog,
        Dust,
        Volcanic_Ash,
        Squalls,
        Tornado,
        Clear,
        Clouds
    }

    public class WeatherCondition
    {
        public Color BackgroundColor
        {
            get;
            set;
        }

        public Color TextColor
        {
            get;
            set;
        }

        public WeatherConditionType Type
        {
            get;
            set;
        }
    }
}
