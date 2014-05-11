using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Threading
{
    public class Pool
    {
        private Thread[] thrds;
        private List<Task> tsks;
        private List<WaitCallback> witems;
        private List<Object> parameters;
        private Boolean finish;


        public Pool(int thread_number)
        {
            thrds = new Thread[thread_number];
            witems = new List<WaitCallback>();
            parameters = new List<Object>();
            finish = false;
            for (int i = 0; i < thread_number; i++)
            {
                thrds[i] = new Thread(PermRoutine);
                thrds[i].Name = i.ToString();
                thrds[i].Start();
            }
        }

        public int TasksInQueue()
        {
            Monitor.Enter(witems);
            int count = witems.Count;
            Monitor.Exit(witems);
            return count;
        }

        //Ждать завершения операций
        public void WaitForEveryone()
        {
            Monitor.Enter(witems);
            finish = true;
            Monitor.PulseAll(witems);
            Monitor.Exit(witems);
            foreach (Thread thrd in thrds)
            {
                thrd.Join();
            }
        }

        public void AbortAll()
        {
            Monitor.Enter(witems);
            witems.Clear();
            parameters.Clear();
            finish = true;
            Monitor.PulseAll(witems);
            Monitor.Exit(witems);
            foreach (Thread thrd in thrds)
            {
                thrd.Abort();
                thrd.Join();
            }
            return;
        }

        //Добавить процедуру в очередь
        public void AddRoutine(WaitCallback workitem, Object parameter)
        {
            Monitor.Enter(witems);
            parameters.Add(parameter);
            witems.Add(workitem);
            Monitor.Pulse(witems);
            Monitor.Exit(witems);
        }

        //Потоковая процедура 1
        private void PrimRoutine()
        {
            Console.WriteLine("Hello! I`m thread!" + Thread.CurrentThread.Name);
        }

        //Потоковая процедура 2
        private void PermRoutine()
        {
            WaitCallback mywc;
            Object myparam;
            while (true)
            {
                Monitor.Enter(witems);
                while (witems.Count == 0)
                {
                    if (!finish)
                        Monitor.Wait(witems);
                    else
                    {
                        Monitor.Exit(witems);
                        return;
                    }
                }
                mywc = witems[0];
                witems.RemoveAt(0);
                myparam = parameters[0];
                parameters.RemoveAt(0);
                Monitor.Exit(witems);
                mywc(myparam);
            }
        }
    }
}

