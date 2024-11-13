using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBridgeEX
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DllMain.Initialize();
                Console.WriteLine("DLL loaded successfully.");

                string input;
                while ((input = Console.ReadLine()) != null)
                {
                    string[] parts = input.Split(' ');
                    string command = parts[0];

                    switch (command)
                    {
                        case "Load":
                            Encoder.Load();
                            break;
                        case "Dispose":
                            Encoder.Dispose();
                            break;
                        case "SetVehicleSpec":
                            AtsVehicleSpec spec = new AtsVehicleSpec
                            {
                                BrakeNotches = int.Parse(parts[1]),
                                PowerNotches = int.Parse(parts[2]),
                                AtsNotch = int.Parse(parts[3]),
                                B67Notch = int.Parse(parts[4]),
                                Cars = int.Parse(parts[5])
                            };
                            Encoder.SetVehicleSpec(spec);
                            break;
                        case "Initialize":
                            int brake = int.Parse(parts[1]);
                            Encoder.Initialize(brake);
                            break;
                        case "Elapse":
                            AtsVehicleState state = new AtsVehicleState
                            {
                                Location = int.Parse(parts[1]),
                                Speed = float.Parse(parts[2]),
                                Time = int.Parse(parts[3]),
                                BcPressure = int.Parse(parts[4]),
                                MrPressure = int.Parse(parts[5]),
                                ErPressure = int.Parse(parts[6]),
                                BpPressure = int.Parse(parts[7]),
                                SapPressure = int.Parse(parts[8]),
                                Current = int.Parse(parts[9])
                            };
                            int[] panels = new int[256];
                            int[] sounds = new int[256];
                            AtsHandles handles = Encoder.Elapse(state, panels, sounds);
                            Console.WriteLine($"Handles: {handles}, Panels: {panels}, Sounds: {sounds}");
                            break;
                        case "SetPower":
                            Encoder.SetPower(int.Parse(parts[1]));
                            break;
                        case "SetBrake":
                            Encoder.SetBrake(int.Parse(parts[1]));
                            break;
                        case "SetReverser":
                            Encoder.SetReverser(int.Parse(parts[1]));
                            break;
                        case "KeyDown":
                            Encoder.KeyDown(int.Parse(parts[1]));
                            break;
                        case "KeyUp":
                            Encoder.KeyUp(int.Parse(parts[1]));
                            break;
                        case "HornBlow":
                            Encoder.HornBlow(int.Parse(parts[1]));
                            break;
                        case "DoorOpen":
                            Encoder.DoorOpen();
                            break;
                        case "DoorClose":
                            Encoder.DoorClose();
                            break;
                        case "SetSignal":
                            Encoder.SetSignal(int.Parse(parts[1]));
                            break;
                        case "SetBeaconData":
                            AtsBeaconData beacon = new AtsBeaconData
                            {
                                Type = int.Parse(parts[1]),
                                Signal = int.Parse(parts[2]),
                                Distance = float.Parse(parts[3]),
                                Optional = int.Parse(parts[4])
                            };
                            Encoder.SetBeaconData(beacon);
                            break;
                        default:
                            Console.WriteLine($"Unrecognized command: {command}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                DllMain.Cleanup();
            }
        }
    }
}
