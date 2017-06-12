using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ManagedPowerManagement
{
    [ComVisible(true)]
    [Guid("BEB8478C-4A1D-4A62-AA2A-23F45A8124B5")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPowerManagement
    {
        long GetLastSleepTime();

        long GetLastWakeTime();

        SYSTEM_BATTERY_STATE GetSystemBatteryState();

        SYSTEM_POWER_INFORMATION GetSystemPowerInformation();

        void ReserveSystemHiberFile(bool reserve);

        bool SetSuspendState(bool hibernate = false);
    }
}
