using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelTasks
{
    public partial class Form1 : Form
    {
        private Parallel p;

        public Form1()
        {
            InitializeComponent();
            p = new Parallel(this, 5);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            t.Start();

            richTextBox1.Text = "";
            p.Execute();
        }

        public void Progress(int value)
        {
            progressBar1.Value += value;
        }

        public void Log(string text)
        {
            richTextBox1.AppendText(text + '\n');
        }

        private void t_Tick(object sender, EventArgs e)
        {
            t.Stop();
            button1.Enabled = true;
        }
    }
}
