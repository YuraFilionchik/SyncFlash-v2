using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SyncFlash
{
    public class Queue
    {
        /// <summary>
        /// Number of queue
        /// </summary>
        public int Number;
        /// <summary>
        /// Need for Copying
        /// </summary>
        public bool Active;
        public string SourceFile;
        public string TargetFile;
        public string SourceFileProjectDir;
        public string TargetFileProjectDir;
        public DateTime DateSource;
        public DateTime DateTarget;
        public bool isNewFile;
        /// <summary>
        /// Count of created objects
        /// </summary>
        public static int Count = 0;
        public Queue()
        {
            Count++;
            this.Active = true;
        }
        public Queue(bool active, string source, string target, string srcProjDir, string targProjDir, DateTime dateS, DateTime dateT, bool isnew)
        {
            Count++;
            this.Active = active;
            Number = Count;
            SourceFile = source;
            TargetFile = target;
            DateSource = dateS;
            DateTarget = dateT;
            SourceFileProjectDir = srcProjDir;
            TargetFileProjectDir = targProjDir;
            isNewFile = isnew;
        }
    }
}
