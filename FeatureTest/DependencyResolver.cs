using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class DependencyResolver
    {
        [Fact]
        public void ResolvePluginFromBaseDirectory()
        {
            string pluginPath = Path.Combine(DeploymentUtilities.ExecutableLocation, "PluginLibrary.dll");
            var resolver = new AssemblyDependencyResolver(pluginPath);
            Assert.Equal(pluginPath, resolver.ResolveAssemblyToPath(new AssemblyName("PluginLibrary")), ignoreCase: true);
            Assert.Null(resolver.ResolveAssemblyToPath(new AssemblyName("UtilitiesLibrary")));
        }

        [Fact]
        public void ResolvePluginFromTemp()
        {
            string subDirectory = Path.Combine(Path.GetTempPath(), "FeatureTest.Temp");
            Directory.CreateDirectory(subDirectory);
            File.Copy(
                Path.Combine(DeploymentUtilities.ExecutableLocation, "PluginLibrary.dll"), 
                Path.Combine(subDirectory, "PluginLibrary.dll"),
                overwrite: true);
            File.Copy(
                Path.Combine(DeploymentUtilities.ExecutableLocation, "PluginLibrary.deps.json"),
                Path.Combine(subDirectory, "PluginLibrary.deps.json"),
                overwrite: true);

            string pluginPath = Path.Combine(subDirectory, "PluginLibrary.dll");
            var resolver = new AssemblyDependencyResolver(pluginPath);

            Assert.Equal(pluginPath, resolver.ResolveAssemblyToPath(new AssemblyName("PluginLibrary")), ignoreCase: true);
            Assert.Null(resolver.ResolveAssemblyToPath(new AssemblyName("UtilitiesLibrary")));
        }
    }
}
