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

namespace BoxManage
{
    /// <summary>
    /// Interaction logic for Items.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        public Item(Item item)
        {
            InitializeComponent();
            this.CircleUi.Height = item.CircleUi.Height;
            this.CircleUi.Width = item.CircleUi.Width;
            this.CircleUi.Fill = item.CircleUi.Fill;
        }
    }
}