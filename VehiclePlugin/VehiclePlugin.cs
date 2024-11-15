using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Native;
using AtsEx.PluginHost.Plugins;
using BveTypes.ClassWrappers;
using System.Windows.Forms;
using AtsEx.PluginHost.Input.Native;

namespace PluginBridgeEX
{
    /// プラグインの本体
    /// Plugin() の第一引数でこのプラグインの仕様を指定
    /// Plugin() の第二引数でこのプラグインが必要とするAtsEX本体の最低バージョンを指定（オプション）
    [Plugin(PluginType.VehiclePlugin)]
    internal class VehiclePluginMain : AssemblyPluginBase
    {
        int brake;
        int oldBrake;//SetBrake()の判定
        int power;
        int oldPower;//SetPower()の判定
        int reverser;
        public VehiclePluginMain(PluginBuilder builder) : base(builder)
        {
            Native.Started += Initialize;
            SendToBackEnd($"SetVehicleSpec {Native.VehicleSpec}");//SetvehicleSpec(AtsVehicleSpec spec)
            Native.Handles.Reverser.PositionChanged += SetReverser;
            Native.NativeKeys.AnyKeyPressed += OnKeyDown;
            Native.NativeKeys.AnyKeyReleased += OnKeyUp;
            Native.HornBlown += HornBlown;
        }

        void Initialize(StartedEventArgs e)//Initialize(int brake)
        {
            SendToBackEnd($"Initialize {e.DefaultBrakePosition}\n");
        }
        void SetReverser(object sender, EventArgs e)//SetReverser(int) AtsEX RC9で実装
        {
            SendToBackEnd($"SetReverser {Native.Handles.Reverser.Position}\n");
        }
        void OnKeyUp(object sender, NativeKeyEventArgs e)//KeyDown(int)
        {
            SendToBackEnd($"KeyDown {(int)e.KeyName}\n");
        }
        void OnKeyDown(object sender, NativeKeyEventArgs e)//KeyUp(int)
        {
            SendToBackEnd($"KeyDown {(int)e.KeyName}\n");
        }
        void HornBlown(HornBlownEventArgs e)
        {
            SendToBackEnd($"HornBlown {(int)e.HornType}");
        }

        public override void Dispose()
        {

        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            brake = Native.Handles.Brake.Notch;
            power = Native.Handles.Power.Notch;
            if (!(brake == oldBrake))
            {
                SendToBackEnd($"SetBrake {brake}");
            }
            if (!(power == oldPower))
            {
                SendToBackEnd($"SetPower {power}");
            }
            oldBrake = Native.Handles.Brake.Notch;//以下は最後の方に記述
            oldPower = Native.Handles.Power.Notch;
            return new VehiclePluginTickResult();
        }
        static void SendToBackEnd(string message)
        {
            if (SendMain(message) == false)
            {
                MessageBox.Show("名前付きパイプの通信でエラーが発生しました。\n発生番地:" + message);
            }
        }
        static bool SendMain(string message)
        {
            return true;
        }
    }
}

