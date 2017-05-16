using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehicleRoutingProblem
{
    partial class Form1 : Form
    {
        public static MapData mapCur;
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.StreamReader sr = new
                   System.IO.StreamReader(openFileDialog1.FileName);
                mapCur = Map.initFileMap(sr);
                //MessageBox.Show(sr.ReadToEnd());
                sr.Close();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }


        private void button4_Click(object sender, EventArgs e)
        {
        }

        void init()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            GABase solve;
            init();
            progressBar1.Value = 0;//进度条归零
                                   //Scale = int.Parse(textBox8.Text);

            //初始化
            if (comboBox2.Text.Trim() == "遗传算法(GA)")
                solve = new GA();
            else
                solve = new cGA();

            if (comboBox1.Text.Trim() == "随机变异算子")
                solve.variation = solve.OnCVariation;
            else
                solve.variation = solve.RevVariation;
            solve.initialize(this);
            solve.run(this);
            //solve.maxT = int.Parse(textBox8.Text);
        }
        internal void setTextBox8(int x)
        {
            textBox8.Text = x.ToString();
        }
    }
}
