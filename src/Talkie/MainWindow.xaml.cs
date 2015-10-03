using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Talkie
{
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Ctrl = 2,
        Shift = 4,
        WindowsKey = 8
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const Int32 MY_HOTKEYID = 0x9999;

        public MainWindow()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hwnd, string lPstring);

        [DllImport("user32")]
        public extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr handle = new WindowInteropHelper(this).Handle;
            RegisterHotKey(handle, MY_HOTKEYID, (uint)KeyModifiers.Alt, 48);

            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;

            source.AddHook(WndProc);
        }     

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handle)
        {
            if (wParam.ToInt32() == MY_HOTKEYID)
            {
                Console.WriteLine("he");
                //全局快捷键要执行的命令
                Say();
            }
            return IntPtr.Zero;
        }

        void Say()
        {
            IntPtr ptr = FindWindow(null, "SAPI5 TTSAPP");
            ptr = FindWindowEx(ptr, IntPtr.Zero, "Button", "Speak");
            const int BM_CLICK = 0xF5;
            SendMessage(ptr, BM_CLICK, 0, 0);     //需要管理员权限，发送点击按钮的消息  
        }
    }
}
