using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class LoadFromAssemblyPath
    {
        [Fact]
        public void LoadExternalAssembly()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(LoadExternalAssembly));
            string filePath = Path.Join(DeploymentUtilities.ExecutableLocation, "PluginLibrary.dll");
            Assembly pluginAssembly = testAlc.LoadFromAssemblyPath(filePath);
            Assembly pluginAssemblySecondLoad = testAlc.LoadFromAssemblyPath(filePath);
            Assert.Same(pluginAssembly, pluginAssemblySecondLoad);
        }

        [Fact]
        public void LoadFromNonExistentPath()
        {
            var exception = Assert.Throws<FileNotFoundException>(() =>
                AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath("nonexistent.dll")));

            if (DeploymentUtilities.IsSelfContained && DeploymentUtilities.IsSingleFile && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Empty(exception.Message);
            }
            else
            {
                Assert.Contains("nonexistent.dll", exception.Message);
            }
        }
    }
}
