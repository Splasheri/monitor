using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitor
{
    //Сhecker               - класс созданный для проверки введенных значений, не имеет конструктора 
    //
    //CheckInputCorrection  - проверяет количество аргументов, переданных командной строке
    //                        затем поочередно проверяет каждый из аргументов, передавая их в функции CheckProcessName и CheckTimeValues
    //                        возвращает структуру monitoringData, содержащую валидные данные  
    //
    //CheckProcessName      - проверяет аругмент содержащий название процесса на
    //                        строку нулевой длины
    //                        отсутствие процессов с соответствующим названием
    //                        соответствие названию процесса данной программы (monitor)
    //
    //CheckTimeValues      -  проверяет аругменты содержащие временные величины на
    //                        значение больше минимального
    //                        значение менше максимального
    //                        при ошибке указывает название первой невалидной величины      

    class Checker
    {
        public Program.monitoringData CheckInputCorrection(string[] arguments)
        {
            if (arguments.Length < 3)
            {
                throw new ArgumentOutOfRangeException(
                    "Number of parameters",
                    "Not enough parameters"
                );
            }
            if (arguments.Length > 3)
            {
                throw new ArgumentOutOfRangeException(
                    "Number of parameters",
                    "Too many parameters"
                );
            }

            string processName  = arguments[0];
            double lifeTime     = double.Parse(arguments[1]);
            double updatePeriod   = double.Parse(arguments[2]);

            CheckProcessName(processName);
            CheckTimeValues(lifeTime, updatePeriod);

            return new Program.monitoringData(processName, lifeTime, updatePeriod);
        }

        private void CheckProcessName(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
            {
                throw new ArgumentException(
                    "Empty process name",
                    "Process name"
                );
            }
            if (Process.GetProcessesByName(processName).Length == 0)
            {
                throw new ArgumentException(
                    "Wrong process name",
                    "Process name"
                );
            }
            if (processName == "monitor")
            {
                throw new ArgumentException(
                    "Wrong process name",
                    "Process name"
                );
            }
        }

        private void CheckTimeValues(double lifeTime, double updatePeriod)
        {
            if (lifeTime < Program.MIN_LIFETIME_NUMBER || lifeTime > Program.MAX_LIFETIME_NUMBER)
            {
                throw new ArgumentException(
                    lifeTime < Program.MIN_LIFETIME_NUMBER ? "Too small number" : "Too big number",
                    "Process lifetime value"
                );
            }

            if (updatePeriod < Program.MIN_UPDATE_PERIOD || updatePeriod > Program.MAX_UPDATE_PERIOD)
            {
                throw new ArgumentException(
                    lifeTime < Program.MIN_UPDATE_PERIOD ? "Too small number" : "Too big number",
                    "Process update period value"
                );
            }

        }
    }
}
