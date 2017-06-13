using System.Runtime.InteropServices;

namespace ManagedPowerManagement
{
    public struct SYSTEM_BATTERY_STATE
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool AcOnLine;

        [MarshalAs(UnmanagedType.I1)]
        public bool BatteryPresent;

        [MarshalAs(UnmanagedType.I1)]
        public bool Charging;

        [MarshalAs(UnmanagedType.I1)]
        public bool Discharging;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
        public bool[] Spare1;

        public uint MaxCapacity;
        public uint RemainingCapacity;
        public uint Rate;
        public uint EstimatedTime;
        public uint DefaultAlert1;
        public uint DefaultAlert2;
    }
}