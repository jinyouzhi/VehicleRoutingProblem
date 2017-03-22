using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRoutingProblem
{
    internal static class simpleGA : GA
    {
        //Random ra;

        //static int maxN = 100;
        //int N;

        //int[] bestGen;//所有代数中最好的染色体
        //double bestEvaluation;//所有代数中最好的染色体的适应度
        //int bestT;//最好的染色体出现的代数

        //int maxV;//车辆最大值
        //int V;//车辆数
        //double Pw;//惩罚系数
        //double Pc, Pm;//交叉概率和变异概率

        //static int Scale = 100;//种群规模
        //public int maxT;//最大进化代数
        //int T;//代数

        //double decodedEvaluation;//解码后路程总和

        //double[,] vehicle = new double[maxN, 3];//K下标从1开始到K，0列表示车的最大载重量，1列表示车行驶的最大距离，2列表示速度
        //int[] decoded = new int[maxN];//染色体解码后表达的每辆车的服务的客户的顺序

        //int[][] oldGroup = new int[Scale][];//初始种群，父代种群，行数表示种群规模，一行代表一个个体，即染色体，列表示染色体基因片段
        //int[][] newGroup = new int[Scale][];//新的种群，子代种群
        //double[] Fitness = new double[maxN];//种群适应度，表示种群中各个个体的适应度
        //double[] Pi = new double[maxN];//种群中各个个体的累计概率

        void simpleGA(int N)
        {
            this.N = N;
        }

        //初始化
        void initData()
        {
            bestGen = new int[maxN];
            bestEvaluation = 0.0;
            bestT = 0;

            decodedEvaluation = 0;//解码后所有车辆所走路程总和

            Pw = 300;//车辆超额惩罚权重

            maxV = 5;//最大车数目
            Pc = 0.9;//交叉概率
            Pm = 0.9;//变异概率，实际为(1-Pc)*0.9=0.09
            //进化代数

            //初始化种群
            for(int i = 0; i < Scale; ++i)
            {
                oldGroup[i] = initGroup();
            }
        }

        public static bool swap(ref int i, ref int j)
        {
            i ^= j;
            j ^= i;
            i ^= j;
            return true;
        }

        int[] initGroup()
        {
            ra = new Random(unchecked((int)DateTime.Now.Ticks));//时间种子
            int[] res = new int[maxN];
            for (int i = 0; i < N; ++i)
                res[i] = i + 1;
            for (int i = 0; i < N - 1; ++i)
            {
                swap(ref res[i], ref res[ra.Next(i + 1, N - 1)]);
            }
            return res;
        }
        void initialize()
        {
        }

        void run()
        {
        }
    }
}
