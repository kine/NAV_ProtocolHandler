using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Win32Helper
{
    #region ### User32 Definitions ###
    public class Win32
    {
        #region Structures
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public int fErase;
            public Rectangle rcPaint;
            public int fRestore;
            public int fIncUpdate;
            public int Reserved1;
            public int Reserved2;
            public int Reserved3;
            public int Reserved4;
            public int Reserved5;
            public int Reserved6;
            public int Reserved7;
            public int Reserved8;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx;
            public int cy;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TRACKMOUSEEVENTS
        {
            public uint cbSize;
            public uint dwFlags;
            public IntPtr hWnd;
            public uint dwHoverTime;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public int time;
            public int pt_x;
            public int pt_y;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct LOGBRUSH
        {
            public uint lbStyle;
            public uint lbColor;
            public uint lbHatch;
        }
        #endregion

        public class User32
        {
            private User32()
            {
            }

            #region ### Constants ###
            public const uint WS_POPUP = 0x80000000;
            public const int WS_EX_TOPMOST = 0x8;
            public const int WS_EX_TOOLWINDOW = 0x80;
            public const int WS_EX_LAYERED = 0x80000;
            public const int WS_EX_TRANSPARENT = 0x20;
            public const int WS_EX_NOACTIVATE = 0x08000000;
            public const int SW_HIDE = 0;
            public const int SW_SHOWNORMAL = 1;
            public const int SW_NORMAL = 1;
            public const int SW_SHOWMINIMIZED = 2;
            public const int SW_SHOWMAXIMIZED = 3;
            public const int SW_MAXIMIZE = 3;
            public const int SW_SHOWNOACTIVATE = 4;
            public const int SW_SHOW = 5;
            public const int SW_MINIMIZE = 6;
            public const int SW_SHOWMINNOACTIVE = 7;
            public const int SW_SHOWNA = 8;
            public const int SW_RESTORE = 9;
            public const int SW_SHOWDEFAULT = 10;
            public const int SW_FORCEMINIMIZE = 11;
            public const int SW_MAX = 11;
            public const uint AW_HOR_POSITIVE = 0x1;
            public const uint AW_HOR_NEGATIVE = 0x2;
            public const uint AW_VER_POSITIVE = 0x4;
            public const uint AW_VER_NEGATIVE = 0x8;
            public const uint AW_CENTER = 0x10;
            public const uint AW_HIDE = 0x10000;
            public const uint AW_ACTIVATE = 0x20000;
            public const uint AW_SLIDE = 0x40000;
            public const uint AW_BLEND = 0x80000;
            public const int WM_CLOSE = 0x0010;
            public const int WM_QUIT = 0x0012;
            public const int WM_DESTROY = 0x0002;
            public const int WM_MOVE = 0x0003;
            public const int WM_SIZE = 0x0005;
            public const int WM_SYSCOMMAND = 0x0112;
            public const int WM_SETICON = 0x0080;
            public const int WM_HOTKEY = 0x0312;
            public const int WM_NCPAINT = 0x0085;
            public const int WM_PAINT = 0x000F;
            public const int WM_ACTIVATEAPP = 0x001C;
            public const int ICON_BIG = 1;
            public const int ICON_SMALL = 0;

            public const int MOD_ALT = 0x0001;
            public const int MOD_CONTROL = 0x0002;
            public const int MOD_SHIFT = 0x0004;
            public const int MOD_WIN = 0x0008;

            public const int VK_F1 = 0x70;
            public const int VK_F2 = 0x71;
            public const int VK_F3 = 0x72;
            public const int VK_F4 = 0x73;
            public const int VK_F5 = 0x74;
            public const int VK_F6 = 0x75;
            public const int VK_F7 = 0x76;
            public const int VK_F8 = 0x77;
            public const int VK_F9 = 0x78;
            public const int VK_F10 = 0x79;
            public const int VK_F11 = 0x7A;
            public const int VK_F12 = 0x7B;

            public const int WM_LBUTTONDOWN = 0x0201;
            public const int WM_LBUTTONUP = 0x0202;
            public const int WM_DRAWITEM = 0x002B;

            public const int SC_RESTORE = 0xF120;
            public const ushort KEYEVENTF_KEYUP = 0x0002;

            public enum VK : ushort
            {
                SHIFT = 0x10,
                CONTROL = 0x11,
                MENU = 0x12,
                ESCAPE = 0x1B,
                BACK = 0x08,
                TAB = 0x09,
                RETURN = 0x0D,
                PRIOR = 0x21,
                NEXT = 0x22,
                END = 0x23,
                HOME = 0x24,
                LEFT = 0x25,
                UP = 0x26,
                RIGHT = 0x27,
                DOWN = 0x28,
                SELECT = 0x29,
                PRINT = 0x2A,
                EXECUTE = 0x2B,
                SNAPSHOT = 0x2C,
                INSERT = 0x2D,
                DELETE = 0x2E,
                HELP = 0x2F,
                NUMPAD0 = 0x60,
                NUMPAD1 = 0x61,
                NUMPAD2 = 0x62,
                NUMPAD3 = 0x63,
                NUMPAD4 = 0x64,
                NUMPAD5 = 0x65,
                NUMPAD6 = 0x66,
                NUMPAD7 = 0x67,
                NUMPAD8 = 0x68,
                NUMPAD9 = 0x69,
                MULTIPLY = 0x6A,
                ADD = 0x6B,
                SEPARATOR = 0x6C,
                SUBTRACT = 0x6D,
                DECIMAL = 0x6E,
                DIVIDE = 0x6F,
                F1 = 0x70,
                F2 = 0x71,
                F3 = 0x72,
                F4 = 0x73,
                F5 = 0x74,
                F6 = 0x75,
                F7 = 0x76,
                F8 = 0x77,
                F9 = 0x78,
                F10 = 0x79,
                F11 = 0x7A,
                F12 = 0x7B,
                OEM_1 = 0xBA,   // ',:' for US
                OEM_PLUS = 0xBB,   // '+' any country
                OEM_COMMA = 0xBC,   // ',' any country
                OEM_MINUS = 0xBD,   // '-' any country
                OEM_PERIOD = 0xBE,   // '.' any country
                OEM_2 = 0xBF,   // '/?' for US
                OEM_3 = 0xC0,   // '`~' for US
                MEDIA_NEXT_TRACK = 0xB0,
                MEDIA_PREV_TRACK = 0xB1,
                MEDIA_STOP = 0xB2,
                MEDIA_PLAY_PAUSE = 0xB3,
                LWIN = 0x5B,
                RWIN = 0x5C
            }


            public struct KEYBDINPUT
            {
                public ushort wVk;
                public ushort wScan;
                public uint dwFlags;
                public long time;
                public uint dwExtraInfo;
            };

            [StructLayout(LayoutKind.Explicit, Size = 28)]
            public struct INPUT
            {
                [FieldOffset(0)]
                public uint type;
                [FieldOffset(4)]
                public KEYBDINPUT ki;
            };

            [StructLayout(LayoutKind.Sequential)]
            public struct DRAWITEMSTRUCT
            {
                public uint CtlType;
                public uint CtlID;
                public uint itemID;
                public uint itemAction;
                public uint itemState;
                public IntPtr hwndItem;
                public IntPtr hDC;
                public RECT rcItem;
                public IntPtr itemData;
            };

            [StructLayout(LayoutKind.Sequential)]
            public struct tagGUITHREADINFO
            {
                public uint cbSize;
                public uint flags;
                public IntPtr hwndActive;
                public IntPtr hwndFocus;
                public IntPtr hwndCapture;
                public IntPtr hwndMenuOwner;
                public IntPtr hwndMoveSize;
                public IntPtr hwndCaret;
                RECT rcCaret;
            };


            #endregion

            #region ### Delegates ###
            public delegate bool WindowEnumDelegate(IntPtr hwnd, int lParam);
            #endregion

            #region ### Methods ###
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int capacity);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool AnimateWindow(IntPtr hWnd, uint dwTime, uint dwFlags);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool ClientToScreen(IntPtr hWnd, ref POINT pt);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool DispatchMessage(ref MSG msg);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool DrawFocusRect(IntPtr hWnd, ref RECT rect);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetDC(IntPtr hWnd);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetFocus();
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern ushort GetKeyState(int virtKey);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool GetMessage(ref MSG msg, int hWnd, uint wFilterMin, uint wFilterMax);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetParent(IntPtr hWnd);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SetParent(IntPtr hWnd, IntPtr parhWnd);
            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern bool GetClientRect(IntPtr hWnd, [In, Out] ref RECT rect);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetWindow(IntPtr hWnd, int cmd);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool HideCaret(IntPtr hWnd);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool InvalidateRect(IntPtr hWnd, ref RECT rect, bool erase);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr LoadCursor(IntPtr hInstance, uint cursor);
            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref RECT rect, int cPoints);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool PeekMessage(ref MSG msg, int hWnd, uint wFilterMin, uint wFilterMax, uint wFlag);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool ReleaseCapture();
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool ScreenToClient(IntPtr hWnd, ref POINT pt);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern uint SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SetCursor(IntPtr hCursor);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SetFocus(IntPtr hWnd);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int newLong);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int X, int Y, int Width, int Height, uint flags);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool ShowCaret(IntPtr hWnd);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool SetCapture(IntPtr hWnd);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern int ShowWindow(IntPtr hWnd, short cmdShow);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int bRetValue, uint fWinINI);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENTS tme);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool TranslateMessage(ref MSG msg);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool UpdateWindow(IntPtr hwnd);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool WaitMessage();
            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern bool AdjustWindowRectEx(ref RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);
            [DllImport("user32.dll")]
            public static extern int EnumChildWindows(IntPtr hwnd, WindowEnumDelegate del, int lParam);
            [DllImport("user32.dll")]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
            [DllImport("user32.dll")]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
            [DllImport("user32.dll")]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();
            [DllImport("User32.dll", SetLastError = true)]
            public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);
            [DllImport("user32.dll")]
            public static extern IntPtr GetKeyboardLayout(uint idThread);
            [DllImport("user32.dll")]
            public static extern short VkKeyScanEx(char ch, IntPtr dwhkl);
            [DllImport("user32.dll")]
            public static extern bool GetGUIThreadInfo(uint idThread, out tagGUITHREADINFO lpgui);
            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

   #endregion
        }
    }
    #endregion

    class SendKeysClass
    {
        public String keys;
        public String caption;
        public String className;
        public int counter;
        public int maxCounter;
        public int parentLevel;

        public SendKeysClass()
        {
            this.counter = 0;
        }

    }

    public static class SendKeys
    {
        public static void SendToClass(string className, string keys, int timeout)
        {
            if (WaitForClass(className, timeout, 0))
            {
                System.Windows.Forms.SendKeys.SendWait(keys);
            }

        }

        public static bool WaitForClass(string waitForClassName, int timeout, int level)
        {
            try
            {
                for (int retry = timeout; retry > 0; retry--)
                {
                    Thread.Sleep(500);
                    timeout--;

                    IntPtr topWindow = Win32.User32.GetForegroundWindow();
                    for (int i = 0; i < level; i++)
                    {
                        topWindow = Win32.User32.GetParent(topWindow);
                    }
                    StringBuilder className = new StringBuilder(256);
                    Win32.User32.GetClassName(topWindow, className, className.Capacity);
                    if (String.Compare(className.ToString(), waitForClassName) != 0) //Handle not found, LOGO displayed? Create new thread for asynchronous execution
                    {
                        if (timeout <= 0)
                        {
                            return false;
                        }
                        //run next check
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

    }
}
