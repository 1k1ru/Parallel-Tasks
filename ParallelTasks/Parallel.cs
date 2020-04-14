using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelTasks
{
    public class Parallel
    {
        private Form1 form;
        private int n;

        private Dictionary<string, Task> tasks;
        private object locker;

        private int[,] M;
        private bool[] R;

        private int[,] resC;
        private int resD;
        private int resE;
        private int resF;
        private int resG;
        private int resH;
        private int resK;

        public delegate void TaskHandler(string invoker);

        public delegate void TaskStarting(string invoker, string startingTask);

        public Parallel(Form1 form, int n)
        {
            this.form = form;
            this.n = n;

            tasks = new Dictionary<string, Task>();
            locker = new object();
        }

        public void Execute()
        {
            tasks.Clear();
            form.progressBar1.Value = 0;

            tasks.Add("A", Task.Run(A));
            tasks.Add("B", Task.Run(B));
        }

        public void TaskStarted(string invoker)
        {
            form.Log($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond}: Task started: {invoker}");
        }

        public void TaskCompleted(string invoker)
        {
            form.Log($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond}: Task completed: {invoker}");
            form.Progress(1);
        }

        public void TaskStartsNewTask(string invoker, string startingTask)
        {
            form.Log($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond}: Task {invoker} started task {startingTask}");
        }

        public void A()
        {
            form.BeginInvoke(new TaskHandler(TaskStarted), "A");

            M = new int[n, n];
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    M[i, j] = rand.Next(n);
                }
            }

            form.BeginInvoke(new TaskHandler(TaskCompleted), "A");

            lock (locker)
            {
                try
                {
                    if (tasks["B"].IsCompleted)
                    {
                        tasks.Add("C", Task.Run(C));
                        form.BeginInvoke(new TaskStarting(TaskStartsNewTask), "A", "C");
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }

        public void B()
        {
            form.BeginInvoke(new TaskHandler(TaskStarted), "B");

            R = new bool[n];
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                R[i] = rand.Next(2) == 1;
            }

            form.BeginInvoke(new TaskHandler(TaskCompleted), "B");

            lock (locker)
            {
                try
                {
                    if (tasks["A"].IsCompleted)
                    {
                        tasks.Add("C", Task.Run(C));
                        form.BeginInvoke(new TaskStarting(TaskStartsNewTask), "B", "C");
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }

        public void C()
        {
            form.BeginInvoke(new TaskHandler(TaskStarted), "C");

            resC = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    //some func
                    if (R[i])
                    {
                        resC[i, j] = M[i, j];
                    }
                }
            }

            form.BeginInvoke(new TaskHandler(TaskCompleted), "C");

            tasks.Add("D", Task.Run(D));
            form.BeginInvoke(new TaskStarting(TaskStartsNewTask), "C", "D");
            tasks.Add("E", Task.Run(E));
            form.BeginInvoke(new TaskStarting(TaskStartsNewTask), "C", "E");
            tasks.Add("F", Task.Run(F));
            form.BeginInvoke(new TaskStarting(TaskStartsNewTask), "C", "F");
        }

        public void D()
        {
            form.BeginInvoke(new TaskHandler(TaskStarted), "D");

            resD = 0;
            foreach (var i in resC)
            {
                //some func
                resD += i;
            }

            form.BeginInvoke(new TaskHandler(TaskCompleted), "D");

            tasks.Add("G", Task.Run(G));
            form.BeginInvoke(new TaskStarting(TaskStartsNewTask), "D", "G");
        }

        public void E()
        {
            form.BeginInvoke(new TaskHandler(TaskStarted), "E");

            resE = 1;
            foreach (var i in resC)
            {
                //some func
                resE *= i;
            }

            form.BeginInvoke(new TaskHandler(TaskCompleted), "E");

            tasks.Add("H", Task.Run(H));
            form.BeginInvoke(new TaskStarting(TaskStartsNewTask), "E", "H");
        }

        public void F()
        {
            form.BeginInvoke(new TaskHandler(TaskStarted), "F");

            resE = 1;
            foreach (var i in resC)
            {
                //some func
                resE *= i;
                resE -= 10 * i;
            }

            form.BeginInvoke(new TaskHandler(TaskCompleted), "F");

            lock (locker)
            {
                try
                {
                    if (tasks["G"].IsCompleted && tasks["H"].IsCompleted)
                    {
                        tasks.Add("K", Task.Run(K));
                        form.BeginInvoke(new TaskStarting(TaskStartsNewTask), "F", "K");
                    }
                }
                catch (Exception e)
                {
                    //ignored
                }
            }
        }

        public void G()
        {
            form.BeginInvoke(new TaskHandler(TaskStarted), "G");

            //some func
            resG = resD * 2;

            form.BeginInvoke(new TaskHandler(TaskCompleted), "G");

            lock (locker)
            {
                try
                {
                    if (tasks["F"].IsCompleted && tasks["H"].IsCompleted)
                    {
                        tasks.Add("K", Task.Run(K));
                        form.BeginInvoke(new TaskStarting(TaskStartsNewTask), "G", "K");
                    }
                }
                catch (Exception e)
                {
                    //ignored
                }
            }
        }

        public void H()
        {
            form.BeginInvoke(new TaskHandler(TaskStarted), "H");

            //some func
            resH = resE + 100;

            form.BeginInvoke(new TaskHandler(TaskCompleted), "H");

            lock (locker)
            {
                try
                {
                    if (tasks["G"].IsCompleted && tasks["F"].IsCompleted)
                    {
                        tasks.Add("K", Task.Run(K));
                        form.BeginInvoke(new TaskStarting(TaskStartsNewTask), "H", "K");
                    }
                }
                catch (Exception e)
                {
                    //ignored
                }
            }
        }

        public void K()
        {
            form.BeginInvoke(new TaskHandler(TaskStarted), "K");

            //some func
            resK = resG + resH;

            form.BeginInvoke(new TaskHandler(TaskCompleted), "K");
        }
    }
}