using System;

namespace VehicleRoutingProblem
{
    public class cGA5 : GABase
    {
        //static int maxL = 100;

        //static Random ra;//随机
        //int N;//地点数量
        int ScaleN;//种群规模-棋盘边长
        int maxT;//繁衍代数

        int maxV;//最大车辆数
        double Pw;//车辆超额惩罚系数
        double Pc, Pm;//交叉概率和变异概率

        //int[][][] backupGroup;//备份初代种群
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
        /// 染色体评价函数，输入一个染色体得到该染色体适应度
        /// </summary>
        /// <param name="Gen">被评价染色体 引用，下标0~L-1</param>
        /// <returns>适应度</returns>
        override internal double Evaluate(int[] Gen, out int[] res)
        {
            res = new int[N + 1];
            double curWeight = MainForm.mapCur.Goods[Gen[1]];
            double curDistance = MainForm.mapCur.Roads[0][Gen[1]];
            double evalution = 0;
            res[1] = 1;
            //车辆数量
            int v = 1;
            //车辆超额数量
            int flag = 0;
            for (int j = 2; j <= N; ++j)
            {
                curDistance += MainForm.mapCur.Roads[Gen[j]][Gen[j - 1]];
                curWeight += MainForm.mapCur.Goods[Gen[j]];

                //如果载重不足或者不够回程
                if (curWeight > MainForm.mapCur.MaxWeight || curDistance + MainForm.mapCur.Roads[Gen[j]][0] > MainForm.mapCur.MaxDistance)
                {
                    ++v;//起用下一辆车
                    res[v] = res[v - 1] + 1;
                    evalution += curDistance + MainForm.mapCur.Roads[Gen[j - 1]][0] - MainForm.mapCur.Roads[Gen[j]][Gen[j - 1]];
                    curDistance = MainForm.mapCur.Roads[0][Gen[j]];
                    curWeight = MainForm.mapCur.Goods[Gen[j]];
                }
                else
                {
                    ++res[v];
                }
            }
            //加上最后一段回程
            evalution += curDistance + MainForm.mapCur.Roads[Gen[N]][0];

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

        static int[,] dir = { { 0,1}, { 0,-1}, { 1,0}, { -1, 0 }, { 0, 0 } };
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
            double[] Pi = new double[4];
            int[] newS = new int[N + 1];
            int[] newPlan = new int[N + 1];
            double newFitness;
            int hit;
            for (int i = 1; i < ScaleN - 1; i++)
            {
                for (int j = 1; j < ScaleN - 1; j++)
                {
                    if (selectPolicy == 2)
                    {
                        //最优个体
                        hit = 0;
                        for (int k = 1; k < 4; ++k)
                            if (Fitness[i + dir[k, 0]][j + dir[k, 1]] >= Fitness[i + dir[hit, 0]][j + dir[hit, 1]])
                                hit = k;
                    }
                    else
                    {
                        sumVal = 0;
                        //精英轮盘赌
                        for (int k = 0; k < 4; ++k)
                            if (Fitness[i + dir[k, 0]][j + dir[k, 1]] >= Fitness[i][j])
                                sumVal += Fitness[i + dir[k, 0]][j + dir[k, 1]];

                        //for (int k = 0; k < 4; k++)
                        //    sumVal += Fitness[i + dir[k, 0]][j + dir[k, 1]];
                        //Pi[0] = Fitness[i + dir[0, 0]][j + dir[0, 1]] / sumVal;
                        //for (int k = 1; k < 4 k++)
                        //    Pi[k] = Pi[k - 1] + Fitness[i + dir[k, 0]][j + dir[k, 1]] / sumVal;
                        if (Fitness[i + dir[0, 0]][j + dir[0, 1]] >= Fitness[i][j])
                            Pi[0] = Fitness[i + dir[0, 0]][j + dir[0, 1]] / sumVal;
                        else
                            Pi[0] = 0.0;
                        for (int k = 1; k < 4; k++)
                            if (Fitness[i + dir[k, 0]][j + dir[k, 1]] >= Fitness[i][j])
                                Pi[k] = Pi[k - 1] + Fitness[i + dir[k, 0]][j + dir[k, 1]] / sumVal;
                            else
                                Pi[k] = Pi[k - 1];

                        //轮盘赌
                        //rand = ra.Next(0, 65535) % 1000 / 1000.0;
                        //hit = ra.Next(0, 65535) % 4;//随机作为默认，无法命中时作为结果
                        //for (int k = 0; k < 4; ++k)
                        //{
                        //    if (rand < Pi[k])
                        //    {
                        //        hit = k;
                        //        break;
                        //    }
                        //}
                        //精英轮盘赌
                        rand = ra.Next(0, 65535) % 1000 / 1000.0;
                        hit = 4;
                        for (int k = 0; k < 4; ++k)
                        {
                            if (rand < Pi[k])
                            {
                                hit = k;
                                break;
                            }
                        }
                    }
                    double p = (PcMethod == 1) ? (Pc + (1 - Pc) * ((Fitness[i + dir[hit, 0]][j + dir[hit, 1]] - Fitness[i][j]) / (Fitness[i + dir[hit, 0]][j + dir[hit, 1]] - Fitness[i][j] * Pc))) : (Pc);
					rand = ra.Next(0, 65535) % 1000 / 1000.0;
					if (rand < p)
					{
						newS = cross(ref curGroup[i][j], ref curGroup[i + dir[hit, 0]][j + dir[hit, 1]], p);
						newFitness = Evaluate(newS, out newPlan);
						if (newFitness > Fitness[i][j])//新子代如果更优替代中心元胞
						{
							Array.Copy(newS, curGroup[i][j], newS.Length);
							Array.Copy(newPlan, plan[i][j], newPlan.Length);
						}
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
        internal override void initialize()
        {
            //ra = new Random(unchecked((int)DateTime.Now.Ticks));//时间种子
            //N = MainForm.mapCur.N;
            ScaleN = (int)(Math.Ceiling(Math.Sqrt(double.Parse(MainForm.mainForm.textBox12.Text.Trim()))));
            maxT = int.Parse(MainForm.mainForm.textBox8.Text.Trim());
            maxV = int.Parse(MainForm.mainForm.textBox10.Text.Trim());
            Pc = double.Parse(MainForm.mainForm.textBox5.Text.Trim());
            Pm = double.Parse(MainForm.mainForm.textBox9.Text.Trim());
            Pw = double.Parse(MainForm.mainForm.textBox11.Text.Trim());
            bestFitness = -1;
            bestT = -1;
            bestGen = new int[N + 1];

            Fitness = new double[ScaleN][];
            for (int i = 0; i < ScaleN; ++i)
                Fitness[i] = new double[ScaleN];

            curGroup = new int[ScaleN][][];
            for (int i = 0; i < ScaleN; ++i)
            {
                curGroup[i] = new int[ScaleN][];
                for (int j = 0; j < ScaleN; ++j)
                    curGroup[i][j] = new int[N + 1];
            }
            //backupGroup = new int[ScaleN][][];
            //for (int i = 0; i < ScaleN; ++i)
            //{
            //    backupGroup[i] = new int[ScaleN][];
            //    for (int j = 0; j < ScaleN; ++j)
            //        backupGroup[i][j] = new int[N + 1];
            //}
            //生成初始种群
            MainForm.mainForm.listBox1.Items.Clear();
            for (int i = 0; i < ScaleN; ++i)
                for (int j = 0; j < ScaleN; j++)
                {
                    Array.Copy(MainForm.mainForm.backupGroup[i * ScaleN + j], curGroup[i][j], MainForm.mainForm.backupGroup[i * ScaleN + j].Length);
                    //curGroup[i][j] = randGroup();
                    MainForm.mainForm.listBox1.Items.Add(getNum(curGroup[i][j]));
                }
            ////备份初代种群
            //Array.Copy(curGroup, backupGroup, curGroup.Length);

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
        }
        /// <summary>
        /// 恢复初始状态
        /// </summary>
        internal override void reset()
        {
            //bestFitness = -1;
            //bestT = -1;
            //bestplan = new int[maxL];
            //bestGen = new int[maxL];
            //Array.Copy(backupGroup, curGroup, backupGroup.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        internal override void run()
        {
            TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
            for (int t = 0; t < maxT; ++t)
            {
                //最好的个体保留到子代
                //Array.Copy(updateBestGen(t), newGroup[0], updateBestGen(t).Length);
                updateBestGen(t);
                Evolution();

                //Array.Copy(lastGroup, newGroup, lastGroup.Length);

                MainForm.mainForm.progressBar1.Value = t * 100 / maxT;
            }
            updateBestGen(maxT);

            TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts = ts2.Subtract(ts1).Duration();
            //时间差的绝对值 
            MainForm.mainForm.textBox7.Text = "运行时间：" + ts.TotalMilliseconds.ToString();

            //出现代数
            MainForm.mainForm.textBox1.Text = bestT.ToString() + " 代";
            //染色体评价值
            MainForm.mainForm.textBox2.Text = bestFitness.ToString();

            //最好的染色体
            string s11 = "";
            for (int i = 1; i <= N; i++)
                s11 = s11 + " " + bestGen[i].ToString();
            MainForm.mainForm.textBox3.Text = s11;

            MainForm.mainForm.listBox2.Items.Clear();
            for (int i = 0; i < ScaleN; ++i)
                for (int j = 0; j < ScaleN; ++j)
                    MainForm.mainForm.listBox2.Items.Add(getNum(curGroup[i][j]));

            bestEvalution = 10.0 / bestFitness;// Evaluate(bestGen, out bestplan);
            MainForm.mainForm.textBox4.Text = bestplan[0].ToString();
            MainForm.mainForm.listBox3.Items.Clear();
            for (int i = 1, j = 1; i <= bestplan[0]; ++i)
            {
                string tmp = "";
                for (; j <= bestplan[i]; ++j)
                    tmp += bestGen[j].ToString() + " ";
                MainForm.mainForm.listBox3.Items.Add(tmp);
            }
            MainForm.mainForm.textBox6.Text = bestEvalution.ToString();


            GraphView.GenerateGraph(N, bestplan, bestGen);
            MainForm.mainForm.progressBar1.Value = 100;

        }
    }
}
