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
using static utauPlugin.Note;

namespace NoteConnector
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //本程序用于修改脚本文件的对象
        UtauPlugin plugin;
        //存放用于拼字的两个音符
        Note note1;
        Note note2;
        //两个音符默认的重叠长度
        float overlap = 60;

        public MainWindow()
        {
            //获取启动参数
            string[] args = App.args;
            if (args.Count() != 0)
            {
                plugin = new UtauPlugin(args[0]);
            }
            else
            {
                Application.Current.Shutdown();
            }
            InitializeComponent();
            Connect();
            Application.Current.Shutdown();
        }
        private void Connect()
        {
            //载入插件
            plugin.Input();
            List<Note> notes = plugin.note;
            //根据有无Prev来决定从第几个音符开始取
            int i = plugin.hasPrev ? 1 : 0;
#warning 这里需要检测最少两个音符
            //取出前两个音符用于拼字
            note1 = notes[i];
            note2 = notes[i + 1];
            //缩短音符得到R后Modulation(移调)默认为空声音会很奇怪，这里自动设置一下
            note1.SetMod(note1.GetMod());
            note2.SetMod(note2.GetMod());
            //得到第二个音的先行
            //通常先行会完全覆盖辅音
            //使用包络线遮蔽先行即可仅发出元音
            //如果音源oto设置不准确，也提供了自定义遮蔽长度的选择
            float discardedLength = note2.GetAtPre();
            //得到音符的包络线
            note1.InitEnvelope(note1.GetEnvelope());
            note2.InitEnvelope(note2.GetEnvelope());
            Envelope envelope1 = note1.envelope;
            Envelope envelope2 = note2.envelope;
            //如果第二个音符前端已使用包络线遮蔽，则认为应不使用p1之前的声音
            try
            {
                float p1_note2=envelope2.GetP(1);
                if (p1_note2 > 0) discardedLength = p1_note2;
            }
            catch { }
            //设置第一个音符的P3和第二个音符的P2为重叠长度（应用淡入淡出）
            envelope1.SetP(overlap, 3);
            envelope2.SetP(overlap, 2);
            //设置第二个音符的P1为遮蔽长度（遮蔽第二个音符的辅音）
            envelope2.SetP(discardedLength, 1);
            //设置第二个音符重叠为遮蔽长度与两音预计重叠长度之和
            note2.SetOve(discardedLength + overlap);
            //设置第一个音长为60，第二个音拉长到总长度不变
            int l = note1.GetLength() + note2.GetLength();
            note1.SetLength(60);
            note2.SetLength(l - 60);
            //将第二个音发音开始的位置对齐到第一个音符的开始位置（遮蔽第一个音符的元音）
            note2.SetPre(discardedLength + 60);
            //保存更改
            plugin.Output();
        }
    }
}
