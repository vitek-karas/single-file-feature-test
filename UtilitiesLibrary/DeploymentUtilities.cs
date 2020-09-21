using System;
using System.Diagnostics;
using System.IO;

namespace UtilitiesLibrary
{
    public static class DeploymentUtilities
    {
        private static bool? _isSingleFile;

        public static bool IsSingleFile
        {
            get
            {
                if (!_isSingleFile.HasValue)
                {
                    _isSingleFile = AppContext.GetData("BUNDLE_PROBE") != null;
                }

                return _isSingleFile.Value;
            }
        }

        private static bool? _isSelfContained;

        public static bool IsSelfContained
        {
            get
            {
                if (!_isSelfContained.HasValue)
                {
                    _isSelfContained = string.IsNullOrEmpty((string)AppContext.GetData("FX_DEPS_FILE"));
                }

                return _isSelfContained.Value;
            }
        }

        private static string _executableLocation;

        public static string ExecutableLocation
        {
            get
            {
                if (_executableLocation == null)
                {
                    _executableLocation = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                }

                return _executableLocation;
            }
        }

        private static string _trustedPlatformAssemblies;

        public static bool IsAssemblyInSingleFile(string assemblyName)
        {
            if (_trustedPlatformAssemblies == null)
            {
                _trustedPlatformAssemblies = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
            }

            return !_trustedPlatformAssemblies.Contains(assemblyName + ".dll");
        }
    }
}
