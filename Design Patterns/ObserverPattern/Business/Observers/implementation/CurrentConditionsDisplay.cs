using ObserverPattern.Business.Observers.interfaces;
using ObserverPattern.Business.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern.Business.Observers.implementation
{
    public class CurrentConditionsDisplay : IObserver, IDisplayElement
    {
        private float temperature;
        private float humidity;
        private WeatherData? weatherData;

        public CurrentConditionsDisplay(WeatherData weatherData)
        {
            this.weatherData = weatherData;
            //subscribe to the publisher to get notification when the data chanaged
            weatherData.RegisterObserver(this);
        }
        public void Display()
        {
            Console.WriteLine($"Current conditions: {temperature}F degrees and {humidity}% humidity");
        }

        public void UpdateByPush(float temp, float humidity, float pressure)
        {
            this.temperature = temp;
            this.humidity = humidity;

            Display();
        }

        //the idea from the update method here is to pull only needed data not forced to take all data.
        public void UpdateByPull()
        {
            temperature = weatherData?.GetTemperature() ?? 0;
            humidity = weatherData?.GetHumidity() ?? 0;

            Display();
        }
    }
}
