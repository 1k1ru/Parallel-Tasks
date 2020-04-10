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

        public delegate void TaskHandler(string s);
        //private event TaskHandler TaskComplete;

        public Parallel(Form1 form, int n)
        {
            this.form = form;
            this.n = n;

            tasks = new Dictionary<string, Task>();
            locker = new object();

            //TaskComplete += Log;
        }

        public void Execute()
        {
            tasks.Clear();
            form.progressBar1.Value = 0;

            tasks.Add("A", Task.Run(A));
            tasks.Add("B", Task.Run(B));
        }

        public void Log(string invoker)
        {
            form.Log($"Task complete: {invoker}");
            form.Progress(1);
        }

        public void A()
        {
            M = new int[n, n];
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    M[i, j] = rand.Next(n);
                }
            }

            form.BeginInvoke(new TaskHandler(Log), "A");

            lock (locker)
            {
                try
                {
                    if (tasks["B"].IsCompleted)
                    {
                        tasks.Add("C", Task.Run(C));
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
            R = new bool[n];
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                R[i] = rand.Next(2) == 1;
            }

            form.BeginInvoke(new TaskHandler(Log), "B");

            lock (locker)
            {
                try
                {
                    if (tasks["A"].IsCompleted)
                    {
                        tasks.Add("C", Task.Run(C));
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

            form.BeginInvoke(new TaskHandler(Log), "C");

            tasks.Add("D", Task.Run(D));
            tasks.Add("E", Task.Run(E));
            tasks.Add("F", Task.Run(F));
        }

        public void D()
        {
            resD = 0;
            foreach (var i in resC)
            {
                //some func
                resD += i;
            }

            form.BeginInvoke(new TaskHandler(Log), "D");

            tasks.Add("G", Task.Run(G));
        }

        public void E()
        {
            resE = 1;
            foreach (var i in resC)
            {
                //some func
                resE *= i;
            }

            form.BeginInvoke(new TaskHandler(Log), "E");

            tasks.Add("H", Task.Run(H));
        }

        public void F()
        {
            resE = 1;
            foreach (var i in resC)
            {
                //some func
                resE *= i;
                resE -= 10 * i;
            }

            form.BeginInvoke(new TaskHandler(Log), "F");

            lock (locker)
            {
                try
                {
                    if (tasks["G"].IsCompleted && tasks["H"].IsCompleted)
                    {
                        tasks.Add("K", Task.Run(K));
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
            //some func
            resG = resD * 2;

            form.BeginInvoke(new TaskHandler(Log), "G");

            lock (locker)
            {
                try
                {
                    if (tasks["F"].IsCompleted && tasks["H"].IsCompleted)
                    {
                        tasks.Add("K", Task.Run(K));
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
            //some func
            resH = resE + 100;

            form.BeginInvoke(new TaskHandler(Log), "H");

            lock (locker)
            {
                try
                {
                    if (tasks["G"].IsCompleted && tasks["F"].IsCompleted)
                    {
                        tasks.Add("K", Task.Run(K));
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
            //some func
            resK = resG + resH;

            form.BeginInvoke(new TaskHandler(Log), "K");
        }
    }
}