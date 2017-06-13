using System;
using System.Runtime.InteropServices;

namespace ManagedPowerManagement
{
    public class PowerManagementInterop
    {
        [DllImport("PowrProf.dll")]
        public static extern uint CallNtPowerInformation(
            int informationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            IntPtr lpOutputBuffer,
            int nOutputBufferSize
            );

        [DllImport("powrprof.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetSuspendState(
            [In, MarshalAs(UnmanagedType.I1)] bool Hibernate,
            [In, MarshalAs(UnmanagedType.I1)] bool ForceCritical,
            [In, MarshalAs(UnmanagedType.I1)] bool DisableWakeEvent
            );
    }
}
