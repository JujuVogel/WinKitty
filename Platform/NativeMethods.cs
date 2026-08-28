using System;
using System.Runtime.InteropServices;

namespace WinKitty.Platform;

internal static class NativeMethods
{
    [DllImport("user32.dll")] public static extern int GetWindowLong(IntPtr hWnd, int index);
    [DllImport("user32.dll")] public static extern int SetWindowLong(IntPtr hWnd, int index, int newStyle);
    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_TOOLWINDOW = 0x80;
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string cls, string? win);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindowEx(
        IntPtr parent,
        IntPtr childAfter,
        string cls,
        string? win);
    [DllImport("user32.dll")] public static extern bool EnumWindows(EnumWindowsProc cb, IntPtr lParam);
     public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
}
