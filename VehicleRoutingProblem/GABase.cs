using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRoutingProblem
{
    abstract public class GABase
    {
        //public static int maxL = 100;
        internal static Random ra = new Random(unchecked((int)DateTime.Now.Ticks));//时间种子
        public static int N;//地点数量
        public int selectPolicy;//0轮盘赌1精英轮盘赌2最优个体
        public int PcMethod;//0静态1自适应
                            /// <summary>
                            /// 初始化染色体，洗牌算法
                            /// </summary>
                            /// <returns>返回初始化后染色体</returns>
        public static int[] randGroup()
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


        public static string getNum(int[] x)
        {
            string res = x[1].ToString();
            for (int i = 2; i <= N; ++i)
                res += "-" + x[i].ToString();
            return res;
        }

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
        /// 随机交叉算子，随机取[ran1,ran2]区间交换，剩下依次排列，Ordered Cross
        /// </summary>
        /// <param name="F1"></param>
        /// <param name="F2"></param>
        internal int[] OXCross(ref int[] F1, ref int[] F2, double rate = 0.0)
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

            //剩余部分
            for (int i = 1, j1 = 1, j2 = 1; i <= N; i++)
            {
                if (i == ran1)
                {
                    i = ran2;
                    continue;
                }
                while (rep1[F1[j1]] > 0) ++j1;
                S1[i] = F1[j1++];
                while (rep2[F2[j2]] > 0) ++j2;
                S2[i] = F2[j2++];
            }
            if (this.GetType() == typeof(GA))
            {
                F1 = S1;
                F2 = S2;
            }
            //选择适应度更好的子代
            int[] _res = new int[N + 1];
            return (Evaluate(S1, out _res) > Evaluate(S2, out _res)) ? S1 : S2;
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
                S2[i] = F2[j2--];
            }
            if (this.GetType() == typeof(GA))
            {
                F1 = S1;
                F2 = S2;
            }
            //选择适应度更好的子代
            int[] _res = new int[N + 1];
            if (Evaluate(S1, out _res) > Evaluate(S2, out _res))
            {
                return S1;
            }
            else
            {
                return S2;
            }
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
                S2[i] = F2[j2--];
            }
            if (this.GetType() == typeof(GA))
            {
                F1 = S1;
                F2 = S2;
            }
            //选择适应度更好的子代
            int[] _res = new int[N + 1];
            return (Evaluate(S1, out _res) > Evaluate(S2, out _res)) ? S1 : S2;
        }

        /// <summary>
        /// 逆转变异算子，[ran1,ran2]逆转
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
        /// 恢复初始状态
        /// </summary>
        internal abstract void reset();

        /// <summary>
        /// 交换变异算子，贪婪策略，随机选择一对交换，和原染色体比较适应度，高于原有替换退出，进行30次后保留原有退出
        /// </summary>
        /// <param name="F"></param>
        internal void OnCVariation(ref int[] F)
        {
            int[] _res = new int[N + 1];
            double origin = Evaluate(F, out _res);
            int ran1, ran2;
            int count = 30;
            for (int i = 1; i <= count; ++i)
            {
                ran1 = 1 + ra.Next(0, 65535) % N;
                do
                {
                    ran2 = 1 + ra.Next(0, 65535) % N;
                } while (ran1 == ran2);
                swap(ref F[ran1], ref F[ran2]);
                if (Evaluate(F, out _res) > origin) return;
                swap(ref F[ran1], ref F[ran2]);
            }
        }
        public delegate void Variation(ref int[] F);
        public Variation variation;
        public delegate int[] Cross(ref int[] F1, ref int[]F2, double rate);
        public Cross cross;
        abstract internal double Evaluate(int[] Gen, out int[] res);
        abstract internal void run();
        internal abstract void initialize();
    }
}
