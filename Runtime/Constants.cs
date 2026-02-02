
namespace Constants
{
    public static class BuildInfo
    {
        public const string buildPath = "Builds\\VEX MAGE"; // This gets appended with the application version and a timestamp
        public const string executableName =
#if UNITY_SERVER
            "VEX MAGE SERVER";
#else
            "VEX MAGE";
#endif
        public const string executableExtensionWindows = "exe";
        public const string executableExtensionAndroid = "apk";
    }
}
