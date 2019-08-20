using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace monitor
{
    //  При запуске программа будет отслеживать все работающие процессы с указанным названием 
    //  как только все процессы завершат свою работу, завершит свою работу и эта программа
    //  main вернет 0 при успешном завершении и 1 при возникновении ошибки
    //  чтобы при работе не отображалось окно, приложение отмечено как приложение Windows в настройках проекта, а не как консольное приложение
    //  для вывода в консоль используется метод AttachConsole(-1), где параметр -1 обозначает ту консоль, через которую запущено приложение
    //
    //Program               - основной класс программы
    //monitoringData        - структура, созданная для удобного представления основной информации(названия процесса, время жизни, период оббновления)
    //
    //В Program заданы все константы
    //
    // MAX_LIFETIME_NUMBER  - максимально допустимое время жизни процесса, проверяется Checker 
    // MIN_LIFETIME_NUMBER  - минимально допустимое время жизни процесса, проверяется Checker 
    // MAX_UPDATE_PERIOD    - максимально  допустимое время обновления, проверяется Checker 
    // MIN_UPDATE_PERIOD    - минимально  допустимое время обновления, проверяется Checker 
    // OUTPUT_PATH          - путь к логу
    //
    //Main                  - точка входа приложения, в котором
    //                        создается экземпляр Checker, выполняется проверка переданных через командную строку аргументов
    //                        создается экземпляр Observer, запускается мониторинг процесса
    //                        при возникновении ошибки, она будет передана в командную строку

    class Program
    {
        //импорт WINAPI функции для подключения консоли
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);

        public struct monitoringData
        {
            public string processName { get; private set; }
            public double lifeTime { get; private set; }
            public double updatePeriod { get; private set; }

            public monitoringData(string _processName, double _lifeTime, double _updatePeriod)
            {
                processName = _processName;
                lifeTime    = _lifeTime;
                updatePeriod  = _updatePeriod;
            }
        }

        public const double MAX_LIFETIME_NUMBER = 25.0;
        public const double MIN_LIFETIME_NUMBER = 1.0 / 60.0;

        public const double MAX_UPDATE_PERIOD   = 25.0;
        public const double MIN_UPDATE_PERIOD   = 1.0 / 60.0;

        public const string OUTPUT_PATH = "./Log.txt";

        private static Checker checker;
        private static Observer observer;

        private static monitoringData data;


        static int Main(string[] args)
        {
            try
            {
                checker = new Checker();
                data = checker.CheckInputCorrection(args);                
                observer = new Observer(data);
            }
            catch (Exception exception)
            {
                AttachConsole(-1);
                Console.WriteLine("\n" + exception.Message);
                return 1;
            }
            observer.StartObserving();
            observer.Dispose();
            return 0;
        }
    }
}
