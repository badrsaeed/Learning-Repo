using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern.Business.Observers.interfaces
{
    public interface IObserver
    {
        void UpdateByPush(float temp, float humidity, float pressure);
        void UpdateByPull();
    }
}
