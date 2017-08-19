using GraphSharp.Controls;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VehicleRoutingProblem
{
    /// <summary>
    /// 单一节点
    /// </summary>
    [DebuggerDisplay("{ID}-{Position}")]
    public class VVertex
    {
        public int ID { get; private set; }
        public int Position { get; set; }

        public VVertex(int id, int position)
        {
            ID = id;
            Position = position;
        }

        public override string ToString()
        {
            if (ID == 0)
                return string.Format("地点【{0}】-起点", ID, Position);
            return string.Format("地点【{0}】-路线【{1}】", ID, Position);
        }
    }

    /// <summary>
    /// 单一边
    /// </summary>
    [DebuggerDisplay("{Source.ID} -> {Target.ID} : {Length}")]
    public class VEdge : Edge<VVertex>
    {
        static Color[] colorCode = { Colors.Aqua, Colors.Black, Colors.CornflowerBlue, Colors.DarkGreen, Colors.Fuchsia, Colors.Gold, Colors.Honeydew, Colors.Indigo, Colors.LightSeaGreen, Colors.Maroon, Colors.Navy, Colors.Olive, Colors.PaleGoldenrod, Colors.Red, Colors.Sienna, Colors.Tomato, Colors.Violet, Colors.Wheat, Colors.Yellow };
        public string ID
        {
            get;
            private set;
        }

        public Color EdgeColor
        {
            get;
            set;
        }

        public VEdge(string id, VVertex source, VVertex target)
            : base(source, target)
        {
            ID = id;
            EdgeColor = colorCode[Math.Max(source.Position, target.Position) % colorCode.Length];
        }

        public override string ToString()
        {
            return ID;
        }
    }

    /// <summary>
    /// 图
    /// </summary>
    public class VGraph : BidirectionalGraph<VVertex, VEdge>
    {
        public VGraph() { }

        public VGraph(bool allowParallelEdges)
            : base(allowParallelEdges) { }

        public VGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) { }
    }

    public class VGraphLayout : GraphLayout<VVertex, VEdge, VGraph> { }

    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class GraphView : UserControl
    {
        public static GraphView gv;
        private VGraph _graph;

        public GraphView()
        {
            InitializeComponent();
            gv = this;
            //用于测试
            //OnNewClick(this, new RoutedEventArgs());
        }

        static public void GenerateGraph(int N, int[] bestplan, int[] bestGen)
        {
            gv._graph = new VGraph();

            //加入各节点
            List<VVertex> existingVerrtics = new List<VVertex>();
            for (int i = 0; i <= N; ++i)
            {
                existingVerrtics.Add(new VVertex(i, 0));
            }
            //标记各节点位置（Position），即是第几条路径上的
            for (int i = 1, j = 1; i <= bestplan[0]; ++i)
            {
                for (; j <= bestplan[i]; ++j)
                {
                    existingVerrtics[bestGen[j]].Position = i;
                }
            }
            //加入各节点
            foreach (VVertex vertex in existingVerrtics)
                gv._graph.AddVertex(vertex);
            //加边
            for (int i = 1, j = 1; i <= bestplan[0]; ++i)
            {
                int v1 = 0, v2 = 0;
                for (v1 = 0; j <= bestplan[i]; ++j, v1 = v2)
                {
                    v2 = bestGen[j];
                    gv._graph.AddEdge(new VEdge(string.Format("{0}->{1} 距离:{2}", v1, v2, MainForm.mapCur.Roads[v1][v2]), existingVerrtics[v1], existingVerrtics[v2]));
                }
                gv._graph.AddEdge(new VEdge(string.Format("{0}->{1} 距离:{2}", v1, 0, MainForm.mapCur.Roads[v1][0]), existingVerrtics[v1], existingVerrtics[0]));
            }
            gv.DataContext = gv._graph;
        }

    }

    public class EdgeColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}