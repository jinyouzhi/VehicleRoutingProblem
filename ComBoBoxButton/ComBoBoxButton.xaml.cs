using System;
using System.Collections.Generic;
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

namespace ComBoBoxButton
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ComBoBoxButton : UserControl
    {
        public ComBoBoxButton()
        {
            InitializeComponent();

            // 添加测试数据  
            for (int ix = 0; ix < 10; ix++)
                _comBox.Items.Add("abcdefg" + ix.ToString());
        }
    }
}
