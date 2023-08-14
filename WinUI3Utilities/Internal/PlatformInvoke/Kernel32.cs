using System.Runtime.InteropServices;

namespace WinUI3Utilities.Internal.PlatformInvoke;

internal static class Kernel32
{
#pragma warning disable SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
    [DllImport(nameof(Kernel32))]
    internal static extern nint GetModuleHandle(string name);
#pragma warning restore SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
}
