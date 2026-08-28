namespace WinKitty.Platform;

internal static class DesktopManager
{
    public static IntPtr FindDesktopOwner()
    {
        // Usually SHELLDLL_DefView is directly under Progman.
        IntPtr progman = NativeMethods.FindWindow("Progman", null);

        if (progman != IntPtr.Zero)
        {
            IntPtr shellView = NativeMethods.FindWindowEx(
                progman,
                IntPtr.Zero,
                "SHELLDLL_DefView",
                null);

            if (shellView != IntPtr.Zero)
                return shellView;
        }

        // Explorer may host SHELLDLL_DefView under a WorkerW
        // instead of directly under Progman.
        IntPtr desktopOwner = IntPtr.Zero;

        NativeMethods.EnumWindows((window, _) =>
        {
            IntPtr shellView = NativeMethods.FindWindowEx(
                window,
                IntPtr.Zero,
                "SHELLDLL_DefView",
                null);

            if (shellView == IntPtr.Zero)
                return true;

            desktopOwner = shellView;
            return false;
        }, IntPtr.Zero);

        return desktopOwner;
    }
    
    public static void ConfigureAsToolWindow(IntPtr windowHandle)
    {
        int style = NativeMethods.GetWindowLong(
            windowHandle,
            NativeMethods.GWL_EXSTYLE);

        NativeMethods.SetWindowLong(
            windowHandle,
            NativeMethods.GWL_EXSTYLE,
            style | NativeMethods.WS_EX_TOOLWINDOW);
    }
}
