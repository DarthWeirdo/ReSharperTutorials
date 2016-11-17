using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ReSharper20163
{
    class CodeGenerationIDisposable
    {
        class HandleWrapper : IDisposable
        {
            readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);
        }
    }
}