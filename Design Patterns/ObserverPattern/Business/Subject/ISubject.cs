using ObserverPattern.Business.Observers.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern.Business.Subject
{
    public interface ISubject
    {
        void RegisterObserver(IObserver o);
        void RemoveObserver(IObserver o);
        void NotifyObserversByPushingDesign();
        void NotifyObserversByPullingDesign();
    }
}
