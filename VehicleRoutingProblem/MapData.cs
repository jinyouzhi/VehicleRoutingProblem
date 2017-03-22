using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRoutingProblem
{
    public class MapData
    {
        public int N;
        public double[][] Roads;
        public double[] Goods;
        //public double[] Weight;
        //public double[] Distance;
        public double MaxDistance = 10000.0;
        public double MaxWeight = 50.0;

        public MapData(int N)
        {
            this.N = N;
            this.Roads = new double[N+1][];
            for (int i = 0; i <= N; ++i)
                this.Roads[i] = new double[N+1];
            this.Goods = new double[N+1];
            //this.Weight = new double[N];
            //this.Distance = new double[N];
        }
    }
}
