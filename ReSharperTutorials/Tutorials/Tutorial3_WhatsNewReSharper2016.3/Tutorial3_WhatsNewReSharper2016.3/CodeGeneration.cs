using System;
using System.Collections.Generic;
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

    class CodeGenerationIComparable
    {        
        class ShoeSize
        {
            private int _sizeCm;

            public int SizeCm => _sizeCm;

            public int SizeEu
            {
                get { return (int) (1.5 * _sizeCm); }
                set { _sizeCm = (int) (value / 1.5); }
            }            
        }


        class Account
        {
            private string _name;
            private string _email;

            public string Name => _name;
            public string EMail => _email;            
        }        

    }
}