using System;
using System.Reflection;

namespace ManagedPowerManagement.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var powerManagement = new PowerManagement();
            var lastSleepTime = powerManagement.GetLastSleepTime();
            var lastWakeTime = powerManagement.GetLastWakeTime();
            var systemBatteryState = powerManagement.GetSystemBatteryState();
            var systemPowerInformation = powerManagement.GetSystemPowerInformation();

            Console.WriteLine($"lastSleepTime - {lastSleepTime}");
            Console.WriteLine();
            Console.WriteLine($"lastWakeTime - {lastWakeTime}");
            Console.WriteLine();


            var fieldsSystemBatteryState = typeof (SYSTEM_BATTERY_STATE).GetFields();
            WriteToConsole(fieldsSystemBatteryState, systemBatteryState);
            Console.WriteLine();

            var fieldsSystemPowerInformation = typeof(SYSTEM_POWER_INFORMATION).GetFields();
            WriteToConsole(fieldsSystemPowerInformation, systemPowerInformation);
            Console.WriteLine();

            Console.ReadKey();
        }

        static void WriteToConsole(FieldInfo[] fields, object src)
        {
            foreach (var field in fields)
            {
                Console.WriteLine(
                    $"{field.Name}: {field.GetValue(src)}");
            }
        }
    }
}
