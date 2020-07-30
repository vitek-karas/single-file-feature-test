using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Xunit;

namespace FeatureTest
{
    public class AssemblyLoad
    {
        [Fact]
        public void LoadCoreLib() => ValidateAssemblyLoad("System.Private.CoreLib");

        [Fact]
        public void LoadAppAssembly() => ValidateAssemblyLoad("FeatureTest");

        [Fact]
        public void LoadProjectReferenceAssembly() => ValidateAssemblyLoad("UtilitiesLibrary");

        [Fact]
        public void LoadPackageReferenceAssembly() => ValidateAssemblyLoad("xunit.assert");

        [Fact]
        public void LoadExternalAssemblyFails()
        {
            Assert.Throws<FileNotFoundException>(() => Assembly.Load("PluginLibrary"));
        }

        [Fact]
        public void LoadPreloadedExternalAssembly()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(LoadPreloadedExternalAssembly));
            Assembly pluginAssembly = testAlc.LoadFromAssemblyPath(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "PluginLibrary.dll"));
            using var scope = testAlc.EnterContextualReflection();
            ValidateAssemblyLoad("PluginLibrary");
        }

        [Fact]
        public void LoadPreloadedAssemblyFromStream()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(LoadPreloadedExternalAssembly));
            using var fileStream = File.OpenRead(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "PluginLibrary.dll"));
            Assembly assembly = testAlc.LoadFromStream(fileStream);
            using var scope = testAlc.EnterContextualReflection();
            ValidateAssemblyLoad("PluginLibrary");
        }

        void ValidateAssemblyLoad(string name)
        {
            var assembly = Assembly.Load(name);
            Assert.NotNull(assembly);
            Assert.Equal(name, assembly.GetName().Name);
        }
    }
}
