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
    /// <summary>
    /// 主窗体类
    /// </summary>
    partial class MainForm : Form
    {
        public static MainForm mainForm;
        public static MapData mapCur;
        public int[][] backupGroup;
        bool ok = false;//初始化标志
        bool load = false;//载入标志
        GABase solve;
        public MainForm()
        {
            InitializeComponent();
            mainForm = this;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            showInfo("导入数据...");
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.StreamReader sr = new
                   System.IO.StreamReader(openFileDialog1.FileName);
                mapCur = Map.initFileMap(sr);
                //MessageBox.Show(sr.ReadToEnd());
                sr.Close();
                ok = false;
                load = true;
                showInfo("数据已导入！");
            }
            else
            {
                showInfo("导入失败！");
                load = false;
            }
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        void init()
        {
            progressBar1.Value = 0;//进度条归零
                                   //Scale = int.Parse(textBox8.Text);
            //listBox1.Items.Clear();
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
            else if (comboBox2.Text.Trim() == "元胞遗传Moore型(cGA9)")
                solve = new cGA9();
            else
                solve = new cGA25();

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
                MessageBox.Show("请先导入数据!");
                showInfo("初始化失败，未导入数据！");
                return;
            }
            solve.initialize();
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //判断是否生成初始种群
            if (!ok)
            {
                //生成初始种群
                button3_Click(sender, e);
            }
            //else
            //{
            //    //仅恢复初始状态
            //    solve.reset();
            //}
            init();
            //运行
            showInfo("开始运行...");
            solve.run();
            //ok = false;
            showInfo("运行结束!");
            //solve.maxT = int.Parse(textBox8.Text);
        }

        /// <summary>
        /// 设定最大繁衍代数
        /// </summary>
        /// <param name="x"></param>
        internal void setTextBox8(int x)
        {
            textBox8.Text = x.ToString();
        }

        /// <summary>
        /// 初始化，生成初始种群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            int Scale = int.Parse(MainForm.mainForm.textBox12.Text.Trim());
            GABase.N = mapCur.N;
            backupGroup = new int[Scale + 1][];
            for (int i = 0; i < Scale; i++)
            {
                backupGroup[i] = GABase.randGroup();
            }
            showInfo("随机生成初始种群");
            ok = true;
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="txtInfo"></param>
        /// <param name="info"></param>
        public static void showInfo(string info)
        {
            System.Windows.Forms.TextBox txtInfo = mainForm.textBox13;
            txtInfo.AppendText(info);
            txtInfo.AppendText(Environment.NewLine);
            txtInfo.ScrollToCaret();

        }
    }
}
