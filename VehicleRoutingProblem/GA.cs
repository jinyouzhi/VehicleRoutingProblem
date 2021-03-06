﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRoutingProblem
{
    public class GA : GABase
    {
        //static int maxL = 100;

        //static Random ra;//随机
        //int N;//地点数量
        int Scale;//种群规模
        int maxT;//繁衍代数

        int maxV;//最大车辆数
        double Pw ;//车辆超额惩罚系数
        double Pc, Pm;//交叉概率和变异概率

        //int[][] backupGroup;//备份初代种群
        int[][] lastGroup;//父代种群
        int[][] newGroup;//子代种群

        int[] bestGen;//最好染色体
        double bestFitness;
        int bestT;//代数
        int[] bestplan;

        int[][] plan;

        double bestEvalution;

        double[] Fitness;//适应度
        double[] Pi;//累计概率


        /// <summary>
        /// 染色体解码函数，输入一个染色体，得到该染色体表达的每辆车的服务的客户的顺序
        /// </summary>
        /// <param name="Gen">染色体</param>
        /// <returns>每辆车的服务的客户的顺序</returns>
        //int[] Decoding(int[] Gen)
        //{
        //    int[] res = new int[N + 1];//每辆车的服务的客户的顺序
        //    double curWeight = Form1.mapCur.Goods[Gen[1]];
        //    double curDistance = Form1.mapCur.Roads[0][Gen[1]];
        //    res[1] = 1;
        //    double evalution = 0;
        //    //车辆数量
        //    int v = 1;

        //    for (int j = 2; j <= N; ++j)
        //    {
        //        curDistance += Form1.mapCur.Roads[Gen[j]][Gen[j - 1]];
        //        curWeight += Form1.mapCur.Goods[Gen[j]];

        //        //如果载重不足或者不够回程
        //        if (curWeight > Form1.mapCur.MaxWeight || curDistance + Form1.mapCur.Roads[Gen[j]][0] > Form1.mapCur.MaxDistance)
        //        {
        //            ++v;//起用下一辆车
        //            res[v] = res[v - 1] + 1;
        //            evalution += curDistance - Form1.mapCur.Roads[Gen[j]][Gen[j - 1]] + Form1.mapCur.Roads[Gen[j - 1]][0];
        //            curDistance = Form1.mapCur.Roads[0][Gen[j]];
        //            curWeight = Form1.mapCur.Goods[Gen[j]];
        //        }
        //        else
        //        {
        //            ++res[v];
        //        }
        //    }
        //    //加上最后一段回程
        //    evalution += curDistance + Form1.mapCur.Roads[Gen[N]][0];

        //    res[0] = v;
        //    return res;
        //}


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
                    evalution += curDistance + MainForm.mapCur.Roads[Gen[j - 1]][0] - MainForm.mapCur.Roads[Gen[j]][Gen[j - 1]] ;
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
            for (int i = 0; i < Scale; ++i)
            {
                Fitness[i] = Evaluate(lastGroup[i], out plan[i]);
                if (Fitness[i] > Fitness[index])
                    index = i;
                if (plan[i][0] <= maxV && Fitness[i] > bestFitness)
                {
                    bestFitness = Fitness[i];
                    Array.Copy(lastGroup[i], bestGen, lastGroup[i].Length);
                    bestT = t;
                    Array.Copy(plan[i], bestplan, plan[i].Length);
                }
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

        /// <summary>
        /// 进化函数
        /// </summary>
        void Evolution()
        {
            int k;
            double rand;
            Pi = CountRate(ref Fitness);
            for (k = 1; k < Scale; ++k)
                Array.Copy(lastGroup[select()], newGroup[k], lastGroup[select()].Length);

            for (k = 1; k + 1 < Scale; k += 2)
            {
                rand = ra.Next(0, 65535) % 1000 / 1000.0;
                double p = (PcMethod == 1) ? (Pc + (1 - Pc) * ((Fitness[k + 1] - Fitness[k]) / (Fitness[k + 1] - Fitness[k]* Pc))) : (Pc);
                if (rand < p)
                {
                    cross(ref newGroup[k], ref newGroup[k + 1], p);
                }
                else
                {
                    rand = ra.Next(0, 65535) % 1000 / 1000.0;
                    if (rand < Pm)
                        variation(ref newGroup[k]);

                    rand = ra.Next(0, 65535) % 1000 / 1000.0;
                    if (rand < Pm)
                        variation(ref newGroup[k + 1]);

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
            Scale = int.Parse(MainForm.mainForm.textBox12.Text.Trim());
            maxT = int.Parse(MainForm.mainForm.textBox8.Text.Trim());
            maxV = int.Parse(MainForm.mainForm.textBox10.Text.Trim());
            Pc = double.Parse(MainForm.mainForm.textBox5.Text.Trim());
            Pm = double.Parse(MainForm.mainForm.textBox9.Text.Trim());
            Pw = double.Parse(MainForm.mainForm.textBox11.Text.Trim());
            newGroup = new int[Scale + 1][];
            for (int i = 0; i < Scale; ++i)
                newGroup[i] = new int[N + 1];
            bestFitness = -1;
            bestT = -1;
            bestGen = new int[N + 1];

            //产生备份初代种群
            //backupGroup = new int[Scale + 1][];
            //for (int i = 0; i < Scale; ++i)
            //{
            //    backupGroup[i] = randGroup();
            //    Fitness[i] = Evaluate(lastGroup[i], out plan[i]);
            //}

            lastGroup = new int[Scale + 1][];
            for (int i = 0; i < Scale; ++i)
            {
                lastGroup[i] = new int[N + 1];
            }

            MainForm.mainForm.listBox1.Items.Clear();
            for (int i = 0; i < Scale; ++i)
            {
                Array.Copy(MainForm.mainForm.backupGroup[i], lastGroup[i], MainForm.mainForm.backupGroup[i].Length);
                //Fitness[i] = Evaluate(lastGroup[i], out plan[i]);
                MainForm.mainForm.listBox1.Items.Add(getNum(lastGroup[i]));
            }

            Fitness = new double[Scale + 1];
            Pi = new double[Scale + 1];

            plan = new int[Scale][];
            for (int i = 0; i < Scale; ++i)
                plan[i] = new int[N + 1];
            bestplan = new int[N + 1];
        }

        /// <summary>
        /// 恢复初始状态
        /// </summary>
        internal override void reset()
        {
            //bestFitness = -1;
            //bestT = -1;
            //bestplan = new int[N + 1];
            //bestGen = new int[N + 1];
            //Array.Copy(backupGroup, lastGroup, backupGroup.Length);
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="form1"></param>
        internal override void run()
        {
            TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
            for (int t = 0; t < maxT; ++t)
            {
                //最好的个体保留到子代
                Array.Copy(updateBestGen(t), newGroup[0], newGroup[0].Length);
                Evolution();

                Array.Copy(lastGroup, newGroup, lastGroup.Length);
                
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
            for (int i = 0; i < Scale; ++i)
                MainForm.mainForm.listBox2.Items.Add(getNum(lastGroup[i]));

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

            //bestGen[1] = 1;
            //bestGen[2] = 7;
            //bestGen[3] = 3;
            //bestGen[4] = 8;
            //bestGen[5] = 5;
            //bestGen[6] = 2;
            //bestGen[7] = 4;
            //bestGen[8] = 6;

            //bestGen[1] = 4;
            //bestGen[2] = 7;
            //bestGen[3] = 6;
            //bestGen[4] = 2;
            //bestGen[5] = 1;
            //bestGen[6] = 3;
            //bestGen[7] = 5;
            //bestGen[8] = 8;

            //bestFitness = Evaluate(bestGen, out bestplan);

            GraphView.GenerateGraph(N, bestplan, bestGen);
            MainForm.mainForm.progressBar1.Value = 100;

        }

    }
}
