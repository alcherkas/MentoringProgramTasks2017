using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ManagedPowerManagement
{
    [ComVisible(true)]
    [Guid("4903BE5B-D53F-4A18-B32C-37A5DFAB3568")]
    [ClassInterface(ClassInterfaceType.None)]
    public class PowerManagement : IPowerManagement
    {
        public long GetLastSleepTime()
        {
            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(long)));
            PowerManagementInterop.CallNtPowerInformation(
                (int)POWER_INFORMATION_LEVEL.LastSleepTime,
                IntPtr.Zero,
                0,
                ptr,
                Marshal.SizeOf(typeof(long))
            );
            var result = Marshal.PtrToStructure(ptr, typeof(long));
            return (long)result;
        }

        public long GetLastWakeTime()
        {
            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(long)));
            PowerManagementInterop.CallNtPowerInformation(
                 (int)POWER_INFORMATION_LEVEL.LastWakeTime,
                 IntPtr.Zero,
                 0,
                 ptr,
                 Marshal.SizeOf(typeof(long))
             );

            var result = Marshal.PtrToStructure(ptr, typeof(long));
            return (long)result;
        }

        public SYSTEM_BATTERY_STATE GetSystemBatteryState()
        {
            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE)));
            PowerManagementInterop.CallNtPowerInformation(
                 (int)POWER_INFORMATION_LEVEL.SystemBatteryState,
                 IntPtr.Zero,
                 0,
                 ptr,
                 Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE))
             );

            var result = Marshal.PtrToStructure(ptr, typeof(SYSTEM_BATTERY_STATE));
            return (SYSTEM_BATTERY_STATE)result;
        }

        public SYSTEM_POWER_INFORMATION GetSystemPowerInformation()
        {
            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION)));
            PowerManagementInterop.CallNtPowerInformation(
                 (int)POWER_INFORMATION_LEVEL.SystemPowerInformation,
                 IntPtr.Zero,
                 0,
                 ptr,
                 Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION))
             );

            var result = Marshal.PtrToStructure(ptr, typeof(SYSTEM_POWER_INFORMATION));
            return (SYSTEM_POWER_INFORMATION)result;
        }

        public void ReserveSystemHiberFile(bool reserve)
        {
            int val = reserve ? 1 : 0;
            var pBool = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(pBool, 0, val);


            PowerManagementInterop.CallNtPowerInformation(
                 (int)POWER_INFORMATION_LEVEL.SystemReserveHiberFile,
                 pBool,
                 Marshal.SizeOf(typeof(bool)),
                 IntPtr.Zero,
                 0
             );

            Marshal.FreeCoTaskMem(pBool);
        }

        public bool SetSuspendState(bool hibernate = false)
        {
            return PowerManagementInterop.SetSuspendState(hibernate, false, false);
        }
    }
}
