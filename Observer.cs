using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace monitor
{
    //StartObserving    - запускает CheckInstances метод с помощью Threading.Timer с периодичностью updatePeriod
    //                    когда выполнение CheckInstances закончится, stopMarker вернет управление в главный поток и таймер остановится
    //
    //UpdateProcessList - создает или обновляет список процессов с именем procesName
    //
    //CheckInstances    - если список процессов пуст - возвращает управление в главный поток
    //                    иначе проверяет каждый процесс, завершая его, если время работы процесса превышает lifeTime
    //                    при уничтожении процесса выполняется запись в лог с помощью класса Logger
    //
    //Dispose           - так как Timer и AutoResetEvent  реализуют IDisposable, удобно освободить память в одном методе.
    //
    //Observer          - в конструкторе перезаписывается monitoringData и создается новый экземпляр класса Logger

    class Observer : IDisposable
    {
        private Logger logger;
        private AutoResetEvent stopMarker;
        private List<Process> processInstances;
        private Timer updateTimer;
        private readonly Program.monitoringData data;

        public Observer(Program.monitoringData _data)
        {
            data = _data;
            logger = new Logger(Program.OUTPUT_PATH);
        }

        public void Dispose()
        {
            stopMarker.Dispose();
            updateTimer.Dispose();
        }

        public void StartObserving()
        {
            stopMarker = new AutoResetEvent(false);

            updateTimer = new Timer(
                CheckInstances, stopMarker as Object,
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromMinutes(data.updatePeriod)
            );

            stopMarker.WaitOne();
            updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void CheckInstances(Object _stopMarker)
        {
            UpdateProcessList();
            AutoResetEvent resetEvent = _stopMarker as AutoResetEvent;
            if (processInstances.Count == 0)
            {
                resetEvent.Set();
                return;
            }
            foreach (var instance in processInstances)
            {
                if ((DateTime.Now - instance.StartTime).TotalMinutes > data.lifeTime)
                {
                    logger.WriteInLog(
                        "" + DateTime.Now +
                        "\t " + instance.ProcessName +
                        "\tid " + instance.Id +
                        "\thas been killed" +
                        "\r\n"
                    );
                    instance.Kill();
                }
            }
        }

        private void UpdateProcessList()
        {
            processInstances = new List<Process>(
                Process.GetProcessesByName(data.processName)
            );
        }
    }
}
