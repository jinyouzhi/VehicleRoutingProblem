using System;

namespace VehicleRoutingProblem
{
    public class cGA : GABase
    {
        static int maxL = 100;

        //static Random ra;//随机
        //int N;//地点数量
        int ScaleN;//种群规模-棋盘边长
        int maxT;//繁衍代数

        int maxV;//最大车辆数
        double Pw;//车辆超额惩罚系数
        double Pc, Pm;//交叉概率和变异概率

        int[][][] curGroup;//种群

        int[] bestGen;//最好染色体
        double bestFitness;
        int bestT;//代数
        int[] bestplan;

        int[][][] plan;

        double bestEvalution;

        double[][] Fitness;//适应度
        //double[][] Pi;//累计概率

        /// <summary>
        /// 初始化染色体，洗牌算法
        /// </summary>
        /// <returns>返回初始化后染色体</returns>
        int[] randGroup()
        {
            int tmp;
            //ra = new Random(unchecked((int)DateTime.Now.Ticks));//时间种子
            int[] res = new int[N + 1];
            for (int i = 0; i <= N; ++i)
                res[i] = i;
            for (int i = 1; i < N; ++i)
            {
                //swap(ref res[i], ref res[ra.Next(i + 1, N - 1)]);
                //洗牌算法，依次将i跟i~N随机交换
                tmp = ra.Next(0, 65535) % ((N) - (i)) + i + 1;
                swap(ref res[i], ref res[tmp]);
            }
            return res;
        }


        string getNum(int[] x)
        {
            string res = x[1].ToString();
            for (int i = 2; i <= N; ++i)
                res += "-" + x[i].ToString();
            return res;
        }


        /// <summary>
        /// 染色体评价函数，输入一个染色体得到该染色体适应度
        /// </summary>
        /// <param name="Gen">被评价染色体 引用，下标0~L-1</param>
        /// <returns>适应度</returns>
        double Evaluate(int[] Gen, out int[] res)
        {
            res = new int[N + 1];
            double curWeight = Form1.mapCur.Goods[Gen[1]];
            double curDistance = Form1.mapCur.Roads[0][Gen[1]];
            double evalution = 0;
            res[1] = 1;
            //车辆数量
            int v = 1;
            //车辆超额数量
            int flag = 0;
            for (int j = 2; j <= N; ++j)
            {
                curDistance += Form1.mapCur.Roads[Gen[j]][Gen[j - 1]];
                curWeight += Form1.mapCur.Goods[Gen[j]];

                //如果载重不足或者不够回程
                if (curWeight > Form1.mapCur.MaxWeight || curDistance + Form1.mapCur.Roads[Gen[j]][0] > Form1.mapCur.MaxDistance)
                {
                    ++v;//起用下一辆车
                    res[v] = res[v - 1] + 1;
                    evalution += curDistance + Form1.mapCur.Roads[Gen[j - 1]][0] - Form1.mapCur.Roads[Gen[j]][Gen[j - 1]];
                    curDistance = Form1.mapCur.Roads[0][Gen[j]];
                    curWeight = Form1.mapCur.Goods[Gen[j]];
                }
                else
                {
                    ++res[v];
                }
            }
            //加上最后一段回程
            evalution += curDistance + Form1.mapCur.Roads[Gen[N]][0];

            res[0] = v;
            flag = v - maxV;//超额车辆
            if (flag < 0)//如果车辆不超额
                flag = 0;

            evalution += flag * Pw;
            return 10.0 / evalution;//压缩适应度
        }

        /// <summary>
        /// 更新最优个体
        /// </summary>
        void updateBestGen(int t)
        {
            for (int i = 0; i < ScaleN; ++i)
                for (int j = 0; j < ScaleN; j++)
                {
                    Fitness[i][j] = Evaluate(curGroup[i][j], out plan[i][j]);
                    if (plan[i][j][0] <= maxV && Fitness[i][j] > bestFitness)
                    {
                        bestFitness = Fitness[i][j];
                        Array.Copy(curGroup[i][j], bestGen, curGroup[i][j].Length);
                        bestT = t;
                        Array.Copy(plan[i][j], bestplan, plan[i][j].Length);
                    }
                }
        }

        static int[,] dir = { { -1,1}, { 0,1}, { 1,1}, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 } , { 0, 0 } };
        /// <summary>
        /// 进化函数
        /// </summary>
        void Evolution()
        {
            //int k;
            //double rand;
            //Pi = CountRate(ref Fitness);
            //for (k = 1; k < Scale; ++k)
            //    Array.Copy(lastGroup[select()], newGroup[k], lastGroup[select()].Length);

            double rand;
            double sumVal = 0;
            double[] Pi = new double[8];
            int[] newS = new int[N + 1];
            int[] newPlan = new int[N + 1];
            double newFitness;
            int hit;
            for (int i = 1; i < ScaleN - 1; i++)
            {
                for (int j = 1; j < ScaleN - 1; j++)
                {
                    sumVal = 0;
                    //精英轮盘赌
                    for (int k = 0; k < 8; ++k)
                        if (Fitness[i + dir[k, 0]][j + dir[k, 1]] >= Fitness[i][j])
                            sumVal += Fitness[i + dir[k, 0]][j + dir[k, 1]];

                    //for (int k = 0; k < 8; k++)
                    //    sumVal += Fitness[i + dir[k, 0]][j + dir[k, 1]];
                    //Pi[0] = Fitness[i + dir[0, 0]][j + dir[0, 1]] / sumVal;
                    //for (int k = 1; k < 8; k++)
                    //    Pi[k] = Pi[k - 1] + Fitness[i + dir[k, 0]][j + dir[k, 1]] / sumVal;
                    if (Fitness[i + dir[0, 0]][j + dir[0, 1]] >= Fitness[i][j])
                        Pi[0] = Fitness[i + dir[0, 0]][j + dir[0, 1]] / sumVal;
                    else
                        Pi[0] = 0.0;
                    for (int k = 1; k < 8; k++)
                        if (Fitness[i + dir[k, 0]][j + dir[k, 1]] >= Fitness[i][j])
                            Pi[k] = Pi[k - 1] + Fitness[i + dir[k, 0]][j + dir[k, 1]] / sumVal;
                        else
                            Pi[k] = 0.0;

                    //轮盘赌
                    //rand = ra.Next(0, 65535) % 1000 / 1000.0;
                    //hit = ra.Next(0, 65535) % 8;//随机作为默认，无法命中时作为结果
                    //for (int k = 0; k < 8; ++k)
                    //{
                    //    if (rand < Pi[k])
                    //    {
                    //        hit = k;
                    //        break;
                    //    }
                    //}
                    //精英轮盘赌
                    rand = ra.Next(0, 65535) % 1000 / 1000.0;
                    hit = 8;
                    for (int k = 0; k < 8; ++k)
                    {
                        if (rand < Pi[k])
                        {
                            hit = k;
                            break;
                        }
                    }
                    double p = (PcMethod == 1) ? (Pc + (1 - Pc)*((Fitness[i + dir[hit, 0]][j + dir[hit, 1]] - Fitness[i][j]) /(Fitness[i + dir[hit, 0]][j + dir[hit,1]] - Fitness[i][j]*Pc))) : (Pc);
                    newS = cross(ref curGroup[i][j], ref curGroup[i + dir[hit, 0]][j + dir[hit, 1]], p);
                    newFitness = Evaluate(newS, out newPlan);
                    if (newFitness > Fitness[i][j])//新子代如果更优替代中心元胞
                    {
                        Array.Copy(newS, curGroup[i][j], newS.Length);
                        Array.Copy(newPlan, plan[i][j], newPlan.Length);
                    }
                    else
                    {
                        //否则按一定概率变异
                        rand = ra.Next(0, 65535) % 1000 / 1000.0;
                        if (rand < Pm)
                            variation(ref curGroup[i][j]);
                    }
                }
            }

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="form1">窗体，用于读取更改窗体数据</param>
        internal override void initialize(Form1 form1)
        {
            ra = new Random(unchecked((int)DateTime.Now.Ticks));//时间种子
            N = Form1.mapCur.N;
            ScaleN = (int)(Math.Ceiling(Math.Sqrt(double.Parse(form1.textBox12.Text.Trim()))));
            maxT = int.Parse(form1.textBox8.Text.Trim());
            maxV = int.Parse(form1.textBox10.Text.Trim());
            Pc = double.Parse(form1.textBox5.Text.Trim());
            Pm = double.Parse(form1.textBox9.Text.Trim());
            Pw = double.Parse(form1.textBox11.Text.Trim());
            curGroup = new int[ScaleN][][];
            for (int i = 0; i < ScaleN; ++i)
            {
                curGroup[i] = new int[ScaleN][];
                for (int j = 0; j < ScaleN; ++j)
                    curGroup[i][j] = new int[N + 1];
            }
            bestFitness = -1;
            bestGen = new int[N + 1];
            bestT = -1;

            Fitness = new double[ScaleN][];
            for (int i = 0; i < ScaleN; ++i)
                Fitness[i] = new double[ScaleN];

            plan = new int[ScaleN][][];
            for (int i = 0; i < ScaleN; ++i)
            {
                plan[i] = new int[ScaleN][];
                for (int j = 0; j < ScaleN; j++)
                {
                    plan[i][j] = new int[N + 1];
                }
            }
            bestplan = new int[N + 1];

            form1.listBox1.Items.Clear();
            for (int i = 0; i < ScaleN; ++i)//生成初始种群
                for (int j = 0; j < ScaleN; j++)      
            {
                curGroup[i][j] = randGroup();
                form1.listBox1.Items.Add(getNum(curGroup[i][j]));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form1"></param>
        internal override void run(Form1 form1)
        {
            TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
            for (int t = 0; t < maxT; ++t)
            {
                //最好的个体保留到子代
                //Array.Copy(updateBestGen(t), newGroup[0], updateBestGen(t).Length);
                updateBestGen(t);
                Evolution();

                //Array.Copy(lastGroup, newGroup, lastGroup.Length);

                form1.progressBar1.Value = t * 100 / maxT;
            }
            updateBestGen(maxT);

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
            for (int i = 1; i <= N; i++)
                s11 = s11 + " " + bestGen[i].ToString();
            form1.textBox3.Text = s11;

            form1.listBox2.Items.Clear();
            for (int i = 0; i < ScaleN; ++i)
                for (int j = 0; j < ScaleN; ++j)
                    form1.listBox2.Items.Add(getNum(curGroup[i][j]));

            bestEvalution = 10.0 / bestFitness;// Evaluate(bestGen, out bestplan);
            form1.textBox4.Text = bestplan[0].ToString();
            form1.listBox4.Items.Clear();
            for (int i = 1, j = 1; i <= bestplan[0]; ++i)
            {
                string tmp = "";
                for (; j <= bestplan[i]; ++j)
                    tmp += bestGen[j].ToString() + " ";
                form1.listBox4.Items.Add(tmp);
            }
            form1.textBox6.Text = bestEvalution.ToString();



            form1.progressBar1.Value = 100;

        }
    }
}
