using Fido2Net.Interop;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;


namespace Fido2Net.Util
{
    internal static class Init
    {
        #region Variables

        private static bool _called;

        #endregion

        #region Public Methods

        public static void Call()
        {
            if (_called) {
                return;
            }

            _called = true;

            // On Windows the native fido2/cbor DLLs live in native\ to avoid a name collision
            // with the managed Fido2.dll (fido2-net-lib) and CBOR.dll (PeterO.Cbor) assemblies.
            // Register a resolver so DllImport("fido2") always finds the correct native binary.
            if (OperatingSystem.IsWindows())
                RegisterNativeResolver();

            Native.fido_init((int)Fido2Settings.Flags);
        }

        #endregion

        #region Private Methods

        private static void RegisterNativeResolver()
        {
            var nativeDir = Path.Combine(AppContext.BaseDirectory, "native");
            if (!Directory.Exists(nativeDir))
                return;

            NativeLibrary.SetDllImportResolver(typeof(Init).Assembly,
                (name, _, _) =>
                {
                    var path = Path.Combine(nativeDir, name + ".dll");
                    return File.Exists(path) ? NativeLibrary.Load(path) : IntPtr.Zero;
                });
        }

        #endregion
    }
}
