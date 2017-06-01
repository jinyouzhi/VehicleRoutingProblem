using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRoutingProblem
{
    abstract public class GABase
    {
        internal static Random ra;//随机
        internal int N;//地点数量
        public int selectPolicy;//0轮盘赌1精英轮盘赌2最优个体
        public int PcMethod;//0静态1自适应

        /// <summary>
        /// 交换函数，交换两个int
        /// </summary>
        /// <param name="i">A</param>
        /// <param name="j">B</param>
        /// <returns></returns>
        internal static bool swap(ref int i, ref int j)
        {
            i ^= j;
            j ^= i;
            i ^= j;
            return true;
        }

        /// <summary>
        /// 随机交叉算子，随机取[ran1,ran2]区间交换到新染色体最前，剩下依次排列
        /// </summary>
        /// <param name="F1"></param>
        /// <param name="F2"></param>
        internal int[] OXCross(ref int[] F1, ref int[] F2, double rate = 0.0)
        {
            int ran1, ran2;
            int flag;
            int[] S1 = new int[N + 1], S2 = new int[N + 1];
            ran1 = 1 + ra.Next(0, 65535) % N;
            do
            {
                ran2 = 1 + ra.Next(0, 65535) % N;
            } while (ran2 == ran1);

            if (ran1 > ran2)
            {
                swap(ref ran1, ref ran2);
            }

            flag = ran2 - ran1 + 1;//删除重复基因前染色体长度

            for (int i = 1, j = ran1; i <= flag; i++, j++)
            {
                S1[i] = F2[j];
                S2[i] = F1[j];
            }
            //已近赋值i=ran2-ran1个基因


            for (int k = 1, j = flag + 1; j <= N; j++)//染色体长度
            {
                Lab3:
                S1[j] = F1[k++];
                for (int i = 1; i <= flag; i++)
                { if (S1[i] == S1[j]) goto Lab3; }
            }

            for (int k = 1, j = flag + 1; j <= N; j++)//染色体长度
            {
                Lab4:
                S2[j] = F2[k++];
                for (int i = 1; i <= flag; i++)
                { if (S2[i] == S2[j]) goto Lab4; }
            }
            if (this.GetType() == typeof(GA))
            {
                F1 = S1;
                F2 = S2;
            }
            return S1;
        }
   
        /// <summary>
        /// 改进型变长逆转交叉算子，随机交换L比例长度
        /// </summary>
        /// <param name="F1">父本1</param>
        /// <param name="F2">父本2</param>
        /// <param name="L">交换长度比例</param>
        /// <returns>生成的子代</returns>
        internal int[] NewOXCROSS(ref int[] F1, ref int[] F2, double rate = 0.0)
        {
            int L = (int)(rate * N);

            int ran1, ran2;
            int[] rep1 = new int[N + 1], rep2 = new int[N + 1];
            int[] S1 = new int[N + 1], S2 = new int[N + 1];
            ran1 = 1 + ra.Next(0, 65535) % (N - L + 1);
            ran2 = ran1 + L - 1;
            //交叉部分
            for (int i = ran1; i <= ran2; ++i)
            {
                S1[i] = F2[i];
                rep1[S1[i]] = 1;
                S2[i] = F1[i];
                rep2[S2[i]] = 1;
            }

            //逆转部分
            for (int i = 1, j1 = N, j2 = N; i <= N; i++)
            {
                if (i == ran1)
                {
                    i = ran2;
                    continue;
                }
                while (rep1[F1[j1]] > 0) --j1;
                S1[i] = F1[j1--];
                while (rep2[F2[j2]] > 0) --j2;
                S2[i] = F1[j2--];
            }
            if (this.GetType() == typeof(GA))
            {
                F1 = S1;
                F2 = S2;
            }
            return S1;
        }

        /// <summary>
        /// 顺序逆转交叉算子(小生境技术——朱大林文）
        /// </summary>
        /// <param name="F1">父代1</param>
        /// <param name="F2">父代2</param>
        /// <returns>子代</returns>
        internal int[] ORXCross(ref int[] F1, ref int[] F2, double rate = 0.0)
        {
            int ran1, ran2;
            int[] rep1 = new int[N + 1], rep2 = new int[N + 1];
            int[] S1 = new int[N + 1], S2 = new int[N + 1];
            ran1 = 1 + ra.Next(0, 65535) % N;
            do
            {
                ran2 = 1 + ra.Next(0, 65535) % N;
            } while (ran2 == ran1);

            if (ran1 > ran2)
            {
                swap(ref ran1, ref ran2);
            }

            //交叉部分
            for (int i = ran1; i <= ran2; ++i)
            {
                S1[i] = F2[i];
                rep1[S1[i]] = 1;
                S2[i] = F1[i];
                rep2[S2[i]] = 1;
            }

            //逆转部分
            for (int i = 1, j1 = N, j2 = N; i <= N; i++)
            {
                if (i == ran1)
                {
                    i = ran2;
                    continue;
                }
                while (rep1[F1[j1]] > 0) --j1;
                S1[i] = F1[j1--];
                while (rep2[F2[j2]] > 0) --j2;
                S2[i] = F1[j2--];
            }
            if (this.GetType() == typeof(GA))
            {
                F1 = S1;
                F2 = S2;
            }
            return S1;
        }

        /// <summary>
        /// 逆转变异算子，[ran1,ran2]随机打乱，其余逆转
        /// </summary>
        /// <param name="F"></param>
        internal void RevVariation(ref int[] F)
        {
            int ran1, ran2;
            ran1 = 1 + ra.Next(0, 65535) % N;
            do
            {
                ran2 = 1 + ra.Next(0, 65535) % N;
            } while (ran2 == ran1);

            if (ran1 > ran2)
            {
                swap(ref ran1, ref ran2);
            }
            int flag = ran2 - ran1 + 1;//逆转部分长度

            for (int i = 0; i < flag / 2; i++)
                swap(ref F[ran1 + i], ref F[ran2 - i]);
        }

        /// <summary>
        /// 随机变异算子，[ran1,ran2]随机打乱
        /// </summary>
        /// <param name="F"></param>
        internal void OnCVariation(ref int[] F)
        {
            int ran1, ran2;
            int count = 1 + ra.Next(0, 65535) % N;
            for (int i = 1; i <= count; ++i)
            {
                ran1 = 1 + ra.Next(0, 65535) % N;
                do
                {
                    ran2 = 1 + ra.Next(0, 65535) % N;
                } while (ran1 == ran2);
                swap(ref F[ran1], ref F[ran2]);
            }
        }
        public delegate void Variation(ref int[] F);
        public Variation variation;
        public delegate int[] Cross(ref int[] F1, ref int[]F2, double rate);
        public Cross cross;
        abstract internal void run(Form1 form1);
        internal abstract void initialize(Form1 form1);
    }
}
