using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PluginBridgeEX
{
    public struct AtsVehicleSpec
    {
        public int BrakeNotches;
        public int PowerNotches;
        public int AtsNotch;
        public int B67Notch;
        public int Cars;
    }

    public struct AtsVehicleState
    {
        public int Location;
        public float Speed;
        public int Time;
        public int BcPressure;
        public int MrPressure;
        public int ErPressure;
        public int BpPressure;
        public int SapPressure;
        public int Current;
    }

    public struct AtsHandles
    {
        public int Brake;
        public int Power;
        public int Reverser;
        public int ConstantSpeed;
    }

    public struct AtsBeaconData
    {
        public int Type;
        public int Signal;
        public float Distance;
        public int Optional;
    }

    public static class Encoder
    {
        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void Load();

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void Dispose();

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetVehicleSpec(AtsVehicleSpec spec);

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void Initialize(int brake);

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern AtsHandles Elapse(AtsVehicleState state, int[] panel, int[] sound);

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetPower(int notch);

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetBrake(int notch);

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetReverser(int notch);

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void KeyDown(int key);

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void KeyUp(int key);

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void HornBlow(int type);

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void DoorOpen();

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void DoorClose();

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetSignal(int aspect);

        [DllImport("YourDLLName.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetBeaconData(AtsBeaconData beacon);
    }
}
