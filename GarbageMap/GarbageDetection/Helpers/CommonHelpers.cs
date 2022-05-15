using System.IO;

namespace GarbageMap.GarbageDetection.Helpers
{
    public static class CommonHelpers
    {
        public static string GetAbsolutePath(string relativePath)
        {
            var _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            var assemblyFolderPath = _dataRoot.Directory.FullName;

            var fullPath = Path.Combine(assemblyFolderPath, relativePath);
            return fullPath;
        }
    }
}
