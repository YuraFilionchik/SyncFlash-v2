namespace SyncFlash
{
    public class Queue
    {
        /// <summary>
        /// Номер записи внутри текущей очереди.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Нужно ли копировать файл.
        /// </summary>
        public bool Active { get; set; }

        public string SourceFile { get; set; } = string.Empty;
        public string TargetFile { get; set; } = string.Empty;
        public string SourceFileProjectDir { get; set; } = string.Empty;
        public string TargetFileProjectDir { get; set; } = string.Empty;

        /// <summary>
        /// Относительный путь файла внутри папки проекта — ключ дедупликации.
        /// </summary>
        public string RelativePath { get; set; } = string.Empty;

        public DateTime DateSource { get; set; }
        public DateTime DateTarget { get; set; }
        public bool isNewFile { get; set; }

        public Queue()
        {
            Active = true;
        }

        public Queue(bool active, string source, string target, string srcProjDir, string targProjDir, DateTime dateS, DateTime dateT, bool isnew)
        {
            Active = active;
            SourceFile = source;
            TargetFile = target;
            SourceFileProjectDir = srcProjDir;
            TargetFileProjectDir = targProjDir;
            DateSource = dateS;
            DateTarget = dateT;
            isNewFile = isnew;
        }
    }
}
