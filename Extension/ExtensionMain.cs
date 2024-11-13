﻿using System;
using System.Runtime.InteropServices;

using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;

namespace PluginBridgeEX
{
    /// <summary>
    /// プラグインの本体
    /// Plugin() の第一引数でこのプラグインの仕様を指定
    /// Plugin() の第二引数でこのプラグインが必要とするAtsEX本体の最低バージョンを指定（オプション）
    /// Togglable を付加するとユーザーがAtsEXのバージョン一覧から有効・無効を切換できる
    /// </summary>
    [Plugin(PluginType.Extension)]
    [Togglable]
    internal class ExtensionMain : AssemblyPluginBase, ITogglableExtension, IExtension
    {
        /// プラグインの有効・無効状態
        private bool status = true;
        //DLLExportを使うためのdll(末尾にベタ移植ゾーンがある)
        private const string dllPath = "YourDLLName.dll";
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        /// <inheritdoc/>
        public bool IsEnabled
        {
            get { return status; }
            set { status = value; }
        }
        /// <summary>
        /// プラグインが読み込まれた時に呼ばれる
        /// 初期化を実装する
        /// </summary>
        /// <param name="builder"></param>
        public ExtensionMain(PluginBuilder builder) : base(builder)
        {
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
            if (status)
            {
                // 処理を実装
            }
            return new ExtensionTickResult();
        }

        //ここからベタ移植ゾーン(with ChatGPT)
        public static void Initialize()
        {
            IntPtr libraryHandle = LoadLibrary(dllPath);
            if (libraryHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to load DLL.");
            }
        }

        public static void Cleanup()
        {
            if (!FreeLibrary(LoadLibrary(dllPath)))
            {
                throw new Exception("Failed to unload DLL.");
            }
        }
    }
}