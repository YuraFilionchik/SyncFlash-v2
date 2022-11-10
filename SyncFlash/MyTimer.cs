using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SyncFlash
{
    class MyTimer
    {
        private DateTime start;
        public string Name;
        LogForm log;
        readonly double MinDuration = 5.5;//минимальная длительность события для отображения в логе
        struct StartID
        {
            public int id;
            public string name;
            public DateTime start;
        }
        List<StartID> startID;
        public MyTimer(LogForm Log)
        {
            log = Log;
            startID = new List<StartID>();
        }


        public void Start(string name)
        {
            start = DateTime.Now;
            Name = name;
        }
        public void Start(string name, int id)
        {if (!log.Visible) return;
            if (startID.Any(x => x.id == id)) return;
            startID.Add(new StartID
            {
                id = id,
                name = name,
                start = DateTime.Now
            });
        }

        //return name and duration of operation
        public string Stop()
        {
            if (!log.Visible) return "";
            var dt = DateTime.Now - start; //time of operation
            if (dt.TotalMilliseconds < MinDuration) return "";
            string result = ">" + Name + "\t\t-->\t" + PrepairString(dt);
            log.AddLine(result);
            return result;
        }
        public string Stop(int id)
        {
            if (!log.Visible) return "";
            if (!startID.Any(x => x.id == id)) return "Error ID timer";
            var START = startID.First(x => x.id == id);
            var dt = DateTime.Now - START.start; //time of operation
            if (dt.TotalMilliseconds < MinDuration) return "";
            string result = ">" + START.name + "\t\t-->\t" + PrepairString(dt);
            log.AddLine(result);
            startID.Remove(START);
            return result;
        }
        public void AddLine(string text)
        { 
            string result = ">" + text;
            log.AddLine(result);
        }
        /// <summary>
        /// перевод timespan в секунды и милисекунды с обозначениями
        /// в зависимости от длительности
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string PrepairString(TimeSpan dt)
        {
            string DT;
            if (dt.Seconds == 0 && dt.Minutes == 0) //if less 1sec
                DT = dt.TotalMilliseconds.ToString() + "мс";
            else if (dt.Minutes == 0) //less 1 min
                DT = "\t" + dt.Seconds.ToString() + "," + dt.Milliseconds.ToString() + "c";
            else DT = dt.Minutes.ToString() + "м " + dt.Seconds + "c";

            return DT;
        }
    }
}
