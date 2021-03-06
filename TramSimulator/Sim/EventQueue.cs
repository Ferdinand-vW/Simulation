﻿using System.Collections.Generic;
using System.Linq;

using TramSimulator.Events;

namespace TramSimulator.Sim
{
    //Priority queue for events
    //super lelijk O(n) maar n is altijd constant dus het valt mee
    // SortedList en dergelijke zorgden voor problemen
    public class EventQueue
    {
        public List<Event> EventList { get; private set; }

        public EventQueue()
        {
            this.EventList = new List<Event>();
        }

        public bool HasEvent()
        {
            return EventList.Count > 0;
        }

        public void AddEvent(Event e)
        {
            EventList.Add(e);
        }

        //Get the next event
        public Event Next()
        {
            //Retrieves the index of the event with the lowest starttime
            int index = EventList.Select((val, i) => new { Val = val, Index = i })
                                 .Aggregate((min,next) => min.Val.StartTime < next.Val.StartTime ? min : next)
                                 .Index;
            Event e = EventList[index];
            EventList.RemoveAt(index);
            return e;
        }
    }
}
