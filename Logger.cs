using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitor
{
    //Logger        - конструктор, сохраняющий в logPath путь к логу
    //                если не найден файл для резервного лога - создает его по пути emergencyLog
    //                проверяет возможность записи в лог, если при записи происходит ошибка - возвращает ApplicationException  
    //
    //WriteInLog    - добавляет в конец созданного файла строку logMessage, если файла нет - он будет создан
    //                при ошибке записи будет выведено сообщение в резервный лог и возвращено false  

    class Logger
    {
        private const string emergencyLog = "./emLog.txt";
        private string logPath;

        public Logger(string filePath)
        {
            logPath = filePath;
            if (!File.Exists(emergencyLog))
            {
                File.Create(emergencyLog);
            }
            if (!CheckWriting())
            {
                throw new ApplicationException("Writing error");
            }
        }

        public void WriteInLog(string logMessage)
        {
            File.AppendAllText(logPath, logMessage);
        }

        public bool CheckWriting()
        {
            try
            {
                WriteInLog("\r\n");
            }
            catch (Exception e)
            {
                File.AppendAllText(emergencyLog, DateTime.Now + e.Message + "\r\n");
                return false;
            }
            return true;
        }
    }
}
