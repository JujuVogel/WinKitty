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

        // On some Windows 10 configurations it lives under a WorkerW.
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
}