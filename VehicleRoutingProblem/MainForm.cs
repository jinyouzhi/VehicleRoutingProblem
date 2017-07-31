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
    partial class MainForm : Form
    {
        public static MainForm mainForm;
        public static MapData mapCur;
        bool ok = false;//初始化标志
        bool load = false;//载入标志
        GABase solve;
        public MainForm()
        {
            InitializeComponent();
            mainForm = this;
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
                ok = false;
                load = true;
            }
            else
            {
                load = false;
            }
        }
        

        void init()
        {
            progressBar1.Value = 0;//进度条归零
                                   //Scale = int.Parse(textBox8.Text);
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";

            //初始化
            if (comboBox2.Text.Trim() == "遗传算法(GA)")
                solve = new GA();
            else
                solve = new cGA();

            if (comboBox3.Text.Trim() == "改进型变长逆转交叉算子")
                solve.cross = solve.NewOXCROSS;
            else if (comboBox3.Text.Trim() == "随机交叉算子")
                solve.cross = solve.OXCross;
            else
                solve.cross = solve.ORXCross;

            if (comboBox1.Text.Trim() == "随机变异算子")
                solve.variation = solve.OnCVariation;
            else
                solve.variation = solve.RevVariation;

            if (comboBox4.Text.Trim() == "轮盘赌(GA)")
                solve.selectPolicy = 0;
            else if (comboBox4.Text.Trim() == "精英轮盘赌(cGA)")
                solve.selectPolicy = 1;
            else
                solve.selectPolicy = 2;

            if (comboBox5.Text.Trim() == "静态概率")
                solve.PcMethod = 0;
            else
                solve.PcMethod = 1;

            if (!load)
            {
                MessageBox.Show("请先载入数据!");
                return;
            }
            solve.initialize();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //判断是否初始化
            if (!ok)
            { 
                init();
                ok = true;
            }
            solve.run();
            ok = false;
            //solve.maxT = int.Parse(textBox8.Text);
        }
        internal void setTextBox8(int x)
        {
            textBox8.Text = x.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            init();
            ok = true;
        }

    }
}
