using System;

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
    }
}
