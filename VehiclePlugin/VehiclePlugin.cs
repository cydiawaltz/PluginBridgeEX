using System;
using AtsEx.PluginHost.Native;
using AtsEx.PluginHost.Plugins;
using System.Windows.Forms;
using AtsEx.PluginHost.Input.Native;
using System.IO.Pipes;
using System.Text;
using System.IO;

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
        private static IntPtr dllModuleHandle;
        private static NamedPipeClientStream pipeToBackend, pipeFromBackend;
        public VehiclePluginMain(PluginBuilder builder) : base(builder)
        {
            //Load関数開始
            if(string.IsNullOrEmpty(BackendPath()))
            {
                MessageBox.Show("バックエンドアプリのパスが見つかりませんでした。");
                return;
            }
            pipeToBackend = new NamedPipeClientStream(".", "pipeToBackend", PipeDirection.Out, PipeOptions.Asynchronous);
            pipeFromBackend = new NamedPipeClientStream(".", "pipeFromBackend", PipeDirection.In, PipeOptions.Asynchronous);
            pipeToBackend.Connect();
            pipeFromBackend.Connect();
            SendToBackEnd("Load\n");
            //Load関数終了
            Native.Started += Initialize;
            SendToBackEnd($"SetVehicleSpec {Native.VehicleSpec}");//SetvehicleSpec(AtsVehicleSpec spec)
            Native.Handles.Reverser.PositionChanged += SetReverser;
            Native.NativeKeys.AnyKeyPressed += OnKeyDown;
            Native.NativeKeys.AnyKeyReleased += OnKeyUp;
            Native.HornBlown += HornBlown;
            Native.DoorOpened += DoorOpen;
            Native.DoorClosed += DoorClose;
            Native.SignalUpdated += SetSignal;
            Native.BeaconPassed += SetBeaconData;
        }
        
        string BackendPath()//暫定的にexeファイルの位置は直下のsetting.txtで指定
        {
            string path = File.ReadAllText(Location + "setting.txt");
            return path;
        }
        void Initialize(StartedEventArgs e)//Initialize(int brake)
        {
            SendToBackEnd($"Initialize {(int)e.DefaultBrakePosition}\n");
        }
        void SetReverser(object sender, EventArgs e)//SetReverser(int) AtsEX RC9で実装
        {
            SendToBackEnd($"SetReverser {(int)Native.Handles.Reverser.Position}\n");
        }
        void OnKeyUp(object sender, NativeKeyEventArgs e)//KeyDown(int)
        {
            SendToBackEnd($"KeyDown {(int)e.KeyName}\n");
        }
        void OnKeyDown(object sender, NativeKeyEventArgs e)//KeyUp(int)
        {
            SendToBackEnd($"KeyDown {(int)e.KeyName}\n");
        }
        void HornBlown(HornBlownEventArgs e)//HornBrow(int)
        {
            SendToBackEnd($"HornBlown {(int)e.HornType}\n");
        }
        void DoorClose(DoorEventArgs e)//DoorOpen()
        {
            SendToBackEnd($"DoorOpen\n");
        }

        void DoorOpen(DoorEventArgs e)//DoorClose()
        {
            SendToBackEnd($"DoorClose\n");
        }
        void SetSignal(SignalUpdatedEventArgs e)//SetSignal(int)
        {
            SendToBackEnd($"SetSignal {e.SignalIndex}\n");
        }
        void SetBeaconData(BeaconPassedEventArgs e)//SetBeaconData(ATS_BEACONDATA)
        {
            //AtsBeaconDataにあたる型がないので、配列でまとめてexe側で解凍する
            var datas = new float[4] { e.Distance, e.Optional, e.SignalIndex, e.Type };
            SendToBackEnd($"SetBeaconData {datas}");
        }

        public override void Dispose()
        {
            SendToBackEnd("Dispose\n");
            pipeFromBackend?.Dispose();
            pipeFromBackend?.Dispose();
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
            //Elapse内処理
            VehicleState s = Native.VehicleState;
            double[] vehicleStates = new double[9] {s.BcPressure,s.BcPressure,s.Current,s.ErPressure,s.Location,s.MrPressure,s.SapPressure,s.Speed,s.Time.TotalMilliseconds};
            //↑最後はTimeSpan型なので、exe側でもう一回変換を噛ませる
            //panelvaluesとsoundvaluesがわからん
            var message = $"Elapse {vehicleStates} {}";
            SendToBackEnd(message);
            var response = ReceiveFromBackEnd();
            if (response != null)
            {
                var values = response.Split(' ');
                // Parse response and update `panelValues`, `soundStates`, and `AtsHandles`
            }
            //Elapse内処理終了
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
        static string ReceiveFromBackEnd()
        {
            if (pipeFromBackend != null && pipeFromBackend.IsConnected)
            {
                using (var reader = new StreamReader(pipeFromBackend, Encoding.UTF8, false, 8192, true))
                {
                    return reader.ReadLine();
                }
            }
            MessageBox.Show("バックエンドアプリからの値の受信に失敗しました");
            return "error";
        }
    }
}

