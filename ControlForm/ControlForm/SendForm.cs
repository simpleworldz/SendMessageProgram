using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlForm
{
    public partial class SendForm : Form
    {
        public SendForm()
        {
            InitializeComponent();
        }
        //ReceiveForm
        private IntPtr receiveFormIP = new IntPtr(0);
        //ReceiveForm的sendBtn
        private IntPtr reShowBtnIP = new IntPtr(0);
        //保存子控件
        private List<IntPtr> listWnd = new List<IntPtr>();
        private void BeginBtn_Click(object sender, EventArgs e)
        {
            if (receiveFormIP != new IntPtr(0))
            {
                MessageBox.Show("ReceiveForm已打开");
                return;
            }
            //不用应用ReceiveForm的方式打开  
            System.Diagnostics.Process.Start(@"..\..\..\ReceiveForm\bin\Debug\ReceiveForm.exe");
            //防止还没打开ReceiveForm就执行后面语句 
            //获取receiveForm
            int n = 0;
            while (n < 25 && receiveFormIP == new IntPtr(0))
            {
                receiveFormIP = FindWindow(null, "ReceiveForm");
                System.Threading.Thread.Sleep(100);
                n++;
            }

            label1.Text = "已获取ReceiveForm句柄";
            label1.ForeColor = Color.Red;

            //获取receiveForm的所有子控件
            //CallBack函数(传入的参数）可以为以下三种形式
            //1.new CallBack()
            //2.degegate
            //3.lambda表达式
            // return true 是因为CallBack函数设定的返回值为bool类型
            EnumChildWindows(receiveFormIP, /*new CallBack(*//*delegate */(IntPtr hwnd, int lParam) =>
            {
                listWnd.Add(hwnd);
                return true;
            }/*)*/, 0);
            //获取receiveForm的按钮
            reShowBtnIP = FindWindowEx(receiveFormIP, new IntPtr(0), null, "Show");

        }
        /// <summary>
        /// 校验是否获取ReveiveFrom句柄 有空把它升级为 AOP 或者特性
        /// </summary>
        public void Verify()
        {
            if (receiveFormIP == new IntPtr(0))
            {
                MessageBox.Show("还未获取ReceiveForm句柄,请点击Begin按钮！");
                return;
            }
        }
        public IntPtr GetTextBoxIP(List<IntPtr> listWnd)
        {
            IntPtr textBoxIP = FindWindowEx(receiveFormIP, new IntPtr(0), null, "");
            return textBoxIP;
        }
        //方法一
        /// <summary>
        /// 获取ReceiveForm的textBox
        /// </summary>
        /// <param name="btn">ReceiveForm中的button</param>
        /// <param name="listWnd">ReceiveForm中的控件列表</param>
        /// <returns></returns>
        public IntPtr GetTextBoxIP(IntPtr btn, List<IntPtr> listWnd)
        {
            IntPtr textBoxIP = new IntPtr(0);
            foreach (IntPtr item in listWnd)
            {
                if (item != btn)
                {
                    textBoxIP = item;
                }

            }
            return textBoxIP;
        }
        /// <summary>
        /// 向textBoxIP中发送字符串
        /// </summary>
        /// <param name="textBoxIP">ReceiveForm的textBox控件</param>
        public void Send(IntPtr textBoxIP)
        {
            //char[] message = textBox1.Text.ToArray();
            string message = textBox1.Text;
            SendMessage(textBoxIP, WM_SETTEXT, IntPtr.Zero, message);
        }
        private void SendBtn_Click(object sender, EventArgs e)
        {
            Verify();
            //将textbox1中信息发送到ReceiveForm中
            IntPtr reTextBoxIp = GetTextBoxIP(listWnd);
            Send(reTextBoxIp);//改参数最好为 textbox空间
        }
        private void ShowBtn_Click(object sender, EventArgs e)
        {
            Verify();
            //点击ReceiveForm的sendBtn  弹出textbox中的信息
            SendMessage(reShowBtnIP, WM_CLICK, IntPtr.Zero, "0");
        }
        private void ClearBtn_Click(object sender, EventArgs e)
        {
            Verify();
            IntPtr reTextBoxIp = GetTextBoxIP(listWnd);
            //(IntPtr)8 表示删除操作
            //SendMessage(reTextBoxIp, WM_CHAR, (IntPtr)8, "0");
            //发送个""过去，表示清空
            SendMessage(reTextBoxIp, WM_SETTEXT, IntPtr.Zero, "");
        }
        //处理的消息种类
        //按下按键
        public static int WM_CLICK = 0x00F5;
        //WM_CHAR消息是俘获某一个字符的消息
        public static int WM_CHAR = 0x102;
        //支持发送中文
        private const int WM_SETTEXT = 0x000C;

        //向指定窗口Send Message（判断接收成功后继续执行后续函数）
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lparam);
        //向指定窗口Post Message（不用判定是否成功接收）
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        //FindWindowEx是在窗口列表中寻找与指定条件相符的第一个子窗口
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(IntPtr hWndParent, CallBack lpfn, int lParam);
        //获取窗口
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    }
    public delegate bool CallBack(IntPtr hwnd, int lParam);
}
