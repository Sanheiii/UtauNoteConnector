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
using utauPlugin;

namespace NoteConnector
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        UtauPlugin plugin;
        Note note1;
        Note note2;
        public MainWindow()
        {
            string[] args = App.args;
            if (args.Count() != 0)
            {
                plugin = new UtauPlugin(args[0]);
            }
            //this.Loaded += MainWindow_Loaded;
            InitializeComponent();
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //载入插件
            plugin.Input();
            List<Note> notes = plugin.note;
            //根据有无Prev来决定从第几个音符开始取
            int i = plugin.hasPrev ? 1 : 0;
#warning 这里需要检测最少两个音符
            //取出前两个note用于拼字
            note1 = notes[i];
            note2 = notes[i+1];
            //缩短音符得到R后Modulation(移调)默认为空会出问题，这里设置一下
            note1.SetMod(note1.GetMod());
            note2.SetMod(note2.GetMod());
            plugin.Output();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow_Loaded(sender, e);
        }
    }
}
