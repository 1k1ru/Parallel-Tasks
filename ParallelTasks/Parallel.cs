using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelTasks
{
    public class Parallel
    {
        private Form1 form; //окно программы
        private int n;      //некоторое число n - размеры обрабатываемых массивов данных

        private Dictionary<string, Task> tasks; //словарь запущенных/выполненных процессов
        private object locker;                  //объект-заглушка для синхронизации доступа к словарю

        private int[,] M;   //генерируемый массив M
        private bool[] R;   //генерируемый массив R

        private int[] f1;   //результат функции C
        private int f2;     //результат функции D
        private int f3;     //результат функции E
        private int f4;     //результат функции F
        private int f5;     //результат функции G
        private int f6;     //результат функции H
        private int f7;     //результат функции K

        public delegate void LogHandler(string message);

        public delegate void ProgressHandler();

        public Parallel(Form1 form, int n)
        {
            this.form = form;
            this.n = n;

            tasks = new Dictionary<string, Task>();
            locker = new object();
        }

        //запуск процессов
        public void Execute()
        {
            tasks.Clear();

            tasks.Add("A", Task.Run(A));
            tasks.Add("B", Task.Run(B));
        }

        //логгирование 
        public void Log(string message)
        {
            form.Log($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond}: {message}");
        }

        //сообщение о начале задачи
        public void TaskStarted(string invoker)
        {
            form.BeginInvoke(new LogHandler(Log), $"Task started: {invoker}");
        }

        //сообщение о выполнении задачи
        public void TaskCompleted(string invoker, int result)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Task completed: {invoker}");
            sb.Append($"Task result: {result}");

            form.BeginInvoke(new LogHandler(Log), sb.ToString());
            form.BeginInvoke(new ProgressHandler(Progress));
        }

        public void TaskCompleted(string invoker, int[] result)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Task completed: {invoker}");
            sb.AppendLine("Task result:");

            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i] + " ");
            }

            form.BeginInvoke(new LogHandler(Log), sb.ToString());
            form.BeginInvoke(new ProgressHandler(Progress));
        }

        public void TaskCompleted(string invoker, bool[] result)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Task completed: {invoker}");
            sb.AppendLine("Task result:");

            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i] + " ");
            }

            form.BeginInvoke(new LogHandler(Log), sb.ToString());

            form.BeginInvoke(new ProgressHandler(Progress));
        }

        public void TaskCompleted(string invoker, int[,] result)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Task completed: {invoker}");
            sb.AppendLine("Task result:");


            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    sb.Append(result[i, j] + " ");
                }

                sb.AppendLine();
            }

            sb.Remove(sb.Length - 1, 1);

            form.BeginInvoke(new LogHandler(Log), sb.ToString());
            form.BeginInvoke(new ProgressHandler(Progress));
        }

        //увеличение значения ProgressBar
        public void Progress()
        {
            form.Progress(1);
        }

        //сообщение о начале новой задачи с указанием имени задачи, которая ее запустила
        public void TaskStartsNewTask(string invoker, string startingTask)
        {
            form.BeginInvoke(new LogHandler(Log), $"Task {invoker} started task {startingTask}");
        }

        public void A()
        {
            TaskStarted("A");

            //генерация M
            M = new int[n, n];
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    M[i, j] = rand.Next(n);
                }
            }

            TaskCompleted("A", M);

            lock (locker)
            {
                try
                {
                    if (tasks["B"].IsCompleted)
                    {
                        tasks.Add("C", Task.Run(C));
                        TaskStartsNewTask("A", "C");
                    }
                }
                catch (Exception) { }
            }
        }

        public void B()
        {
            TaskStarted("B");

            //генерация R
            R = new bool[n];
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                R[i] = rand.Next(2) == 1;
            }

            TaskCompleted("A", R);

            lock (locker)
            {
                try
                {
                    if (tasks["A"].IsCompleted)
                    {
                        tasks.Add("C", Task.Run(C));
                        TaskStartsNewTask("B", "C");
                    }
                }
                catch (Exception) { }
            }
        }

        public void C()
        {
            TaskStarted("C");

            //некоторая функция f1
            f1 = new int[n];
            for (int i = 0; i < n; i++)
            {
                f1[i] = 0;
                for (int j = 0; j < n; j++)
                {
                    if (R[i])
                    {
                        f1[i] += M[i, j];
                    }
                }
            }

            TaskCompleted("C", f1);

            tasks.Add("D", Task.Run(D));
            TaskStartsNewTask("C", "D");
            tasks.Add("E", Task.Run(E));
            TaskStartsNewTask("C", "E");
            tasks.Add("F", Task.Run(F));
            TaskStartsNewTask("C", "F");
        }

        public void D()
        {
            TaskStarted("D");

            //некоторая функция f1
            f2 = 0;
            foreach (var i in f1)
            {
                f2 += i;
            }

            TaskCompleted("D", f2);

            tasks.Add("G", Task.Run(G));
            TaskStartsNewTask("D", "G");
        }

        public void E()
        {
            TaskStarted("E");

            //некоторая функция f1
            f3 = 1;
            foreach (var i in f1)
            {
                f3 *= i;
            }

            TaskCompleted("E", f3);

            tasks.Add("H", Task.Run(H));
            TaskStartsNewTask("E", "H");
        }

        public void F()
        {
            TaskStarted("F");

            //некоторая функция f1
            f4 = 1;
            foreach (var i in f1)
            {
                f4 *= i;
                f4 -= 10 * i;
            }

            TaskCompleted("F", f4);

            lock (locker)
            {
                try
                {
                    if (tasks["G"].IsCompleted && tasks["H"].IsCompleted)
                    {
                        tasks.Add("K", Task.Run(K));
                        TaskStartsNewTask("F", "K");
                    }
                }
                catch (Exception) { }
            }
        }

        public void G()
        {
            TaskStarted("G");

            //некоторая функция f1
            f5 = f2 * 2;

            TaskCompleted("G", f5);

            lock (locker)
            {
                try
                {
                    if (tasks["F"].IsCompleted && tasks["H"].IsCompleted)
                    {
                        tasks.Add("K", Task.Run(K));
                        TaskStartsNewTask("G", "K");
                    }
                }
                catch (Exception) { }
            }
        }

        public void H()
        {
            TaskStarted("H");

            //некоторая функция f1
            f6 = f3 + 100;

            TaskCompleted("H", f6);

            lock (locker)
            {
                try
                {
                    if (tasks["G"].IsCompleted && tasks["F"].IsCompleted)
                    {
                        tasks.Add("K", Task.Run(K));
                        TaskStartsNewTask("H", "K");
                    }
                }
                catch (Exception) { }
            }
        }

        public void K()
        {
            TaskStarted("K");

            //некоторая функция f1
            f7 = f5 + f6;

            TaskCompleted("K", f7);
        }
    }
}