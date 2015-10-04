using System;
using System.Collections.Generic;
using System.Configuration;
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

    public enum WND_MSG
    {
        BM_CLICK = 0xF5
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const Int32 MY_HOTKEYID = 0x9999;  // Speak
        private const Int32 HOTKEY_PAUSERESUME = 0x10000;  // Pause/Resume

        private IntPtr NeoWnd { get; set; }

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
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr handle = new WindowInteropHelper(this).Handle;

            uint modifiers = uint.Parse(ConfigurationManager.AppSettings["KeyModifiers"]);
            uint key = uint.Parse(ConfigurationManager.AppSettings["Key"]);

            RegisterHotKey(handle, MY_HOTKEYID, modifiers, key);

            // Alt + 4 Pause/Resume
            RegisterHotKey(handle, HOTKEY_PAUSERESUME, (uint) KeyModifiers.Alt, 52 /* ascii for 4*/);

            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;

            source.AddHook(WndProc);
        }     

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handle)
        {
            int v = 0;
            try
            {
                v = wParam.ToInt32();
            }
            catch (OverflowException)
            {

            }
            switch (v)
            {
                case MY_HOTKEYID:
                    Console.WriteLine("he");
                    //全局快捷键要执行的命令
                    Say();
                    break;
                case HOTKEY_PAUSERESUME:
                    PauseOrResume();
                    break;
            }                                                                          
            return IntPtr.Zero;
        }

        void Say()
        {
            IntPtr ptr = FindWindow(null, "SAPI5 TTSAPP");
            NeoWnd = ptr;

            SetText();

            ptr = FindWindowEx(ptr, IntPtr.Zero, "Button", "Speak");
            const int BM_CLICK = 0xF5;
            SendMessage(ptr, BM_CLICK, 0, IntPtr.Zero);     //需要管理员权限，发送点击按钮的消息  
        }

        void SetText()
        {
            const int WM_SETTEXT                      =0x000C;
            string txt = Clipboard.GetText();

            IntPtr txtWnd = FindWindowEx(NeoWnd, IntPtr.Zero, "RichEdit20A", null);
            IntPtr lparam = new IntPtr();
            lparam = Marshal.StringToHGlobalAnsi(txt);
            SendMessage(txtWnd, WM_SETTEXT, 0, lparam);
            Marshal.FreeHGlobal(lparam);
        }

        void PauseOrResume()
        {
            if (NeoWnd !=null)
            {
                IntPtr ptr = FindWindowEx(NeoWnd, IntPtr.Zero, "Button", "Pause");
                if (ptr != IntPtr.Zero)
                {
                    SendMessage(ptr, (int)WND_MSG.BM_CLICK, 0, IntPtr.Zero);     //需要管理员权限，发送点击按钮的消息  
                }
                else
                {
                    ptr = FindWindowEx(NeoWnd, IntPtr.Zero, "Button", "Resume");
                    if (ptr != IntPtr.Zero)
                    {
                        SendMessage(ptr, (int)WND_MSG.BM_CLICK, 0, IntPtr.Zero);     //需要管理员权限，发送点击按钮的消息  
                    }
                }
            }
        }
    }
}
