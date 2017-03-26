using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LumenWorks.Framework.IO.Csv;

namespace VehicleRoutingProblem
{
    public static class Map
    {
        //     public static MapData newRandMap(int N, )
        //     {
        //         MapData res = new MapData(N);
        //return res;
        //     }
        public static MapData initFileMap(System.IO.StreamReader sr)
        {
            CsvReader csv = new CsvReader(sr, false);
            //字段数量
            int fieldCount = csv.FieldCount;
            MapData res = new MapData(fieldCount - 1);
            //标题数组
            //string[] headers = csv.GetFieldHeaders();
            //只进的游标读取
            for(int row = 0; row <fieldCount;++row)
            {
                csv.ReadNextRecord();
                //遍历列
                //Console.Out.WriteLine(csv.ToString());

                for (int i = 0; i < fieldCount; i++)
                    res.Roads[row][i] = double.Parse(csv[i]);

            }
            csv.ReadNextRecord();
            for (int i = 0; i < fieldCount; i++)
                res.Goods[i] = double.Parse(csv[i]);
            csv.ReadNextRecord();
            res.MaxWeight = double.Parse(csv[0]);
            res.MaxDistance = double.Parse(csv[1]);

            //MessageBox.Show(N.ToString());
            return res;
        }
    }
}
