using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Native;
using AtsEx.PluginHost.Plugins;
using BveTypes.ClassWrappers;

namespace PluginBridgeEX
{
    /// <summary>
    /// プラグインの本体
    /// Plugin() の第一引数でこのプラグインの仕様を指定
    /// Plugin() の第二引数でこのプラグインが必要とするAtsEX本体の最低バージョンを指定（オプション）
    /// </summary>
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
        }
        public void Initialize(StartedEventArgs e)//Initialize(int brake)
        {
            SendToBackEnd($"Initialize {brake}\n");
        }
        public void SetReverser(object sender,EventArgs e)//SetReverser(int notch) AtsEX RC9で実装
        {
            SendToBackEnd($"SetReverser {reverser}\n");
        }

        /// <summary>
        /// プラグインが解放されたときに呼ばれる
        /// 後処理を実装する
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// シナリオ読み込み中に毎フレーム呼び出される
        /// </summary>
        /// <param name="elapsed">前回フレームからの経過時間</param>
        public override TickResult Tick(TimeSpan elapsed)
        {
            brake = Native.Handles.Brake.Notch;
            power = Native.Handles.Power.Notch;
            switch(Native.Handles.Reverser.Position)
            {
                case ReverserPosition.B: reverser = -1; break;
                case ReverserPosition.F: reverser = 1; break;
                case ReverserPosition.N: reverser = 0; break;
            }

            oldBrake = Native.Handles.Brake.Notch;//以下は最後の方に記述
            oldPower = Native.Handles.Power.Notch;
            return new VehiclePluginTickResult();
        }
        static bool SendToBackEnd(string message)
        {
            return false;
        }
    }
}

