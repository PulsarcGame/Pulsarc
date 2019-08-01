using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.Beatmaps
{
    abstract public class Event
    {
        public int time;
        public string type;
        public List<string> parameters; 

        public Event(string line)
        {
            parameters = new List<string>();
            Queue<string> parts = new Queue<string>(line.Split(','));

            time = int.Parse(parts.Dequeue());
            type = parts.Dequeue();
            parameters.AddRange(parts);            
        }

        public override string ToString()
        {
            return time + "," + type + "," + string.Join(',',parameters.ToArray());
        }

        public abstract void Handle(GameplayEngine gameplayEngine);
    }
}
