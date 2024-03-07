using ObserverPattern.Business.Observers.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern.Business.Subject
{
    public class WeatherData : ISubject
    {
        private List<IObserver> observers = new List<IObserver>();
        private float temperature;
        private float humidity;
        private float pressure;


        public void RegisterObserver(IObserver o)
        {
            observers.Add(o);
        }

        public void RemoveObserver(IObserver o)
        {
            observers.Remove(o);
        }
        public void NotifyObserversByPushingDesign()
        {
            foreach (var observer in observers)
                observer.UpdateByPush(temperature, humidity, pressure);
        }
        public void NotifyObserversByPullingDesign()
        {
            foreach (var observer in observers)
                observer.UpdateByPull();
        }
        public void MeasurementsChanged()
        {
            //NotifyObserversByPushingDesign();
            NotifyObserversByPullingDesign();
        }
        public void SetMeasurements(float temperature, float humidity, float pressure)
        {
            this.temperature = temperature;
            this.humidity = humidity;
            this.pressure = pressure;
            // fire event to notify all observers with changes
            MeasurementsChanged();
        }

        public float GetTemperature() { return temperature; }
        public float GetHumidity() {  return humidity; }
        public float GetPressure() { return pressure;}
    }
}
