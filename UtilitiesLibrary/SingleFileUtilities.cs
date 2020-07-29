using System;

namespace UtilitiesLibrary
{
    public static class SingleFileUtilities
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
    }
}
