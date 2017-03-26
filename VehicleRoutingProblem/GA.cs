using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRoutingProblem
{
    public class GA
    {
        static int maxL = 100;

        static Random ra;//随机
        int N;//地点数量
        int Scale;//种群规模
        int maxT;//繁衍代数

        int maxV;//最大车辆数
        double Pw = 300;//车辆超额惩罚系数
        double Pc = 0.9, Pm = 0.9;//交叉概率和变异概率

        int[][] lastGroup;//父代种群
        int[][] newGroup;//子代种群

        int[] bestGen;//最好染色体
        double bestFitness;
        int bestT;//代数

        double bestEvalution;

        double[] Fitness;//适应度
        double[] Pi;//累计概率

        /// <summary>
        /// 交换函数，交换两个int
        /// </summary>
        /// <param name="i">A</param>
        /// <param name="j">B</param>
        /// <returns></returns>
        public static bool swap(ref int i, ref int j)
        {
            i ^= j;
            j ^= i;
            i ^= j;
            return true;
        }

        /// <summary>
        /// 初始化染色体，洗牌算法
        /// </summary>
        /// <returns>返回初始化后染色体</returns>
        int[] randGroup()
        {
            //ra = new Random(unchecked((int)DateTime.Now.Ticks));//时间种子
            int[] res = new int[maxL];
            for (int i = 0; i < N; ++i)
                res[i] = i + 1;
            for (int i = 0; i < N - 1; ++i)
            {
                //swap(ref res[i], ref res[ra.Next(i + 1, N - 1)]);
                swap(ref res[i], ref res[ra.Next(0, 65535) % ((N - 1) - (i)) + i + 1]);
            }
            return res;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="form1">窗体，用于读取更改窗体数据</param>
        internal void initialize(Form1 form1)
        {
            ra = new Random(unchecked((int)DateTime.Now.Ticks));//时间种子
            N = Form1.mapCur.N;
            maxT = form1.getTextBox8();
            Scale = 100;
            maxV = 2;
            lastGroup = new int[Scale + 1][];
            newGroup = new int[Scale + 1][];

            Fitness = new double[Scale + 1];
            Pi = new double[Scale + 1];

            form1.listBox1.Items.Clear();
            for (int i = 0; i <= Scale; ++i)
            {
                lastGroup[i] = randGroup();
                Fitness[i] = Evaluate(ref lastGroup[i]);
                form1.listBox1.Items.Add(getNum(lastGroup[i]));
            }
            updateBestGen(0);
        }

        string getNum(int[] x)
        {
            string res = x[0].ToString();
            for (int i = 1; i < N; ++i)
                res += "-" + x[i].ToString();
            return res;
        }

        /// <summary>
        /// 染色体解码函数，输入一个染色体，得到该染色体表达的每辆车的服务的客户的顺序
        /// </summary>
        /// <param name="Gen">染色体</param>
        /// <returns>每辆车的服务的客户的顺序</returns>
        int[] Decoding(int[] Gen)
        {
            int[] res = new int[maxL];//每辆车的服务的客户的顺序
            double curWeight = Form1.mapCur.Goods[Gen[0]];
            double curDistance = Form1.mapCur.Roads[0][Gen[0]];
            res[1] = 1;
            double evalution = 0;
            //车辆数量
            int v = 1;

            for (int j = 1; j < N; ++j)
            {
                curDistance += Form1.mapCur.Roads[Gen[j]][Gen[j - 1]];
                curWeight += Form1.mapCur.Goods[Gen[j]];

                //如果载重不足或者不够回程
                if (curWeight > Form1.mapCur.MaxWeight || curDistance + Form1.mapCur.Roads[Gen[j]][0] > Form1.mapCur.MaxDistance)
                {
                    ++v;//起用下一辆车
                    res[v] = res[v - 1] + 1;
                    evalution += curDistance - Form1.mapCur.Roads[Gen[j]][Gen[j - 1]] + Form1.mapCur.Roads[Gen[j - 1]][0];
                    curDistance = Form1.mapCur.Roads[0][Gen[j]];
                    curWeight = Form1.mapCur.Goods[Gen[j]];
                }
                else
                {
                    ++res[v];
                }
            }
            //加上最后一段回程
            bestEvalution = evalution += curDistance + Form1.mapCur.Roads[Gen[N - 1]][0];

            res[0] = v;
            return res;
        }


        /// <summary>
        /// 染色体评价函数，输入一个染色体得到该染色体适应度
        /// </summary>
        /// <param name="Gen">被评价染色体 引用，下标0~L-1</param>
        /// <returns>适应度</returns>
        double Evaluate(ref int[] Gen)
        {
            double curWeight = Form1.mapCur.Goods[Gen[0]];
            double curDistance = Form1.mapCur.Roads[0][Gen[0]];
            double evalution = 0;
            //车辆数量
            int v = 1;
            //车辆超额数量
            int flag = 0;
            for (int j = 1; j < N; ++j)
            {
                curDistance += Form1.mapCur.Roads[Gen[j]][Gen[j - 1]];
                curWeight += Form1.mapCur.Goods[Gen[j]];

                //如果载重不足或者不够回程
                if (curWeight > Form1.mapCur.MaxWeight || curDistance + Form1.mapCur.Roads[Gen[j]][0] > Form1.mapCur.MaxDistance)
                {
                    ++v;//起用下一辆车
                    evalution += curDistance - Form1.mapCur.Roads[Gen[j]][Gen[j - 1]] + Form1.mapCur.Roads[Gen[j - 1]][0];
                    curDistance = Form1.mapCur.Roads[0][Gen[j]];
                    curWeight = Form1.mapCur.Goods[Gen[j]];
                }
            }
            //加上最后一段回程
            evalution += curDistance + Form1.mapCur.Roads[Gen[N - 1]][0];

            flag = v - maxV;//超额车辆
            if (flag < 0)//如果车辆不超额
                flag = 0;

            evalution += curDistance + flag * Pw;
            return 10.0 / evalution;//压缩适应度
        }

        /// <summary>
        /// 计算累积概率
        /// </summary>
        /// <param name="val">适应度</param>
        /// <returns>累积概率</returns>
        double[] CountRate(ref double[] val)
        {
            double sumVal = 0;
            for (int k = 0; k < Scale; ++k)
                sumVal += val[k];

            double[] res = new double[Scale];

            res[0] = val[0] / sumVal;
            for (int k = 1; k < Scale; ++k)
            {
                res[k] = res[k - 1] + val[k] / sumVal;
            }
            return res;
        }
        /// <summary>
        /// 更新最优个体
        /// </summary>
        int[] updateBestGen(int t)
        {
            int index = 0;
            for (int i = 1; i < Scale; ++i)
                if (Fitness[i] > Fitness[index])
                    index = i;

            if (Fitness[index] > bestFitness)
            {
                bestFitness = Fitness[index];
                bestGen = lastGroup[index];
                bestT = t;
            }
            return lastGroup[index];
        }

        int select()
        {
            double rand = ra.Next(0, 65535) % 1000 / 1000.0;

            for (int k = 0; k < Scale; ++k)
            {
                if (rand < Pi[k])
                    return k;
            }
            return ra.Next(0, 65535) % Scale;
        }
        ///
        void OXCross(ref int[] F1, ref int[] F2)
        {
            int ran1, ran2;
            int flag;
            int[] S1 = new int[N], S2 = new int[N];
            ran1 = ra.Next(0, 65535) % N;
            do
            {
                ran2 = ra.Next(0, 65535) % N;
            } while (ran2 == ran1);

            if (ran1 > ran2)
            {
                swap(ref ran1, ref ran2);
            }


            flag = ran2 - ran1 + 1;//删除重复基因前染色体长度

            for (int i = 0, j = ran1; i < flag; i++, j++)
            {
                S1[i] = F2[j];
                S2[i] = F1[j];
            }
            //已近赋值i=ran2-ran1个基因


            for (int k = 0, j = flag; j < N; j++)//染色体长度
            {
            Lab3:
                S1[j] = F1[k++];
                for (int i = 0; i < flag; i++)
                { if (S1[i] == S1[j]) goto Lab3; }
            }

            for (int k = 0, j = flag; j < N; j++)//染色体长度
            {
            Lab4:
                S2[j] = F2[k++];
                for (int i = 0; i < flag; i++)
                { if (S2[i] == S2[j]) goto Lab4; }
            }
            F1 = S1;
            F2 = S2;
        }

        void OnCVariation(ref int[] F)
        {
            int ran1, ran2;
            int count = ra.Next(0, 65535) % N;
            for (int i = 0; i < count; ++i)
            {
                ran1 = ra.Next(0, 65535) % N;
                do
                {
                    ran2 = ra.Next(0, 65535) % N;
                } while (ran1 == ran2);
                swap(ref F[ran1], ref F[ran2]);
            }
        }

        /// <summary>
        /// 进化函数
        /// </summary>
        void Evolution()
        {
            int k;
            double rand;
            Pi = CountRate(ref Fitness);
            for (k = 1; k < Scale; ++k)
                newGroup[k] = lastGroup[select()];

            for (k = 1; k + 1 < Scale; k += 2)
            {
                rand = ra.Next(0, 65535) % 1000 / 1000.0;
                if (rand < Pc)
                    OXCross(ref newGroup[k], ref newGroup[k + 1]);
                else
                {
                    rand = ra.Next(0, 65535) % 1000 / 1000.0;
                    if (rand < Pm)
                        OnCVariation(ref newGroup[k]);

                    rand = ra.Next(0, 65535) % 1000 / 1000.0;
                    if (rand < Pm)
                        OnCVariation(ref newGroup[k + 1]);

                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form1"></param>
        internal void run(Form1 form1)
        {
            TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
            for (int t = 0; t < maxT; ++t)
            {
                //最好的个体保留到子代
                newGroup[0] = updateBestGen(t);
                Evolution();

                lastGroup = newGroup;

                for (int i = 0; i < Scale; ++i)
                {
                    Fitness[i] = Evaluate(ref lastGroup[i]);
                }
                form1.progressBar1.Value = t * 100 / maxT;
            }

            TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts = ts2.Subtract(ts1).Duration();
            //时间差的绝对值 
            form1.textBox7.Text = "运行时间：" + ts.TotalMilliseconds.ToString();

            //出现代数
            form1.textBox1.Text = bestT.ToString() + " 代";
            //染色体评价值
            form1.textBox2.Text = bestFitness.ToString();

            //最好的染色体
            string s11 = "";
            for (int i = 0; i < N; i++)
                s11 = s11 + " " + bestGen[i].ToString();
            form1.textBox3.Text = s11;

            int[] assign = Decoding(bestGen);

            form1.listBox2.Items.Clear();
            for (int i = 0; i < Scale; ++i)
                form1.listBox2.Items.Add(getNum(lastGroup[i]));

            form1.textBox4.Text = assign[0].ToString();
            form1.textBox5.Text = "";
            for (int i = 1; i <= assign[0]; ++i)
                form1.textBox5.Text += " " + assign[i].ToString();

            form1.textBox6.Text = bestEvalution.ToString();

            form1.progressBar1.Value = 100;

        }
    }
}
