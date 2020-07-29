using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class AssemblyLocation
    {
        [Fact]
        public void LocationOfCoreLib() => ValidateBundledAssemblyLocation(typeof(object).Assembly);

        [Fact]
        public void LocationOfAppAssembly() => ValidateBundledAssemblyLocation(typeof(AssemblyLocation).Assembly);

        [Fact]
        public void LocationOfProjectReferenceAssembly() => ValidateBundledAssemblyLocation(typeof(SingleFileUtilities).Assembly);

        [Fact]
        public void LocationOfPackageReferenceAssembly() => ValidateBundledAssemblyLocation(typeof(Assert).Assembly);

        [Fact]
        public void LocationOfExternalAssembly()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(LocationOfExternalAssembly));
            Assembly pluginAssembly = testAlc.LoadFromAssemblyPath(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "PluginLibrary.dll"));
            ValidateOnDiskAssemblyLocation(pluginAssembly);
        }

        [Fact]
        public void LocationOfAssemblyLoadedFromStream()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(LocationOfAssemblyLoadedFromStream));
            using var fileStream = File.OpenRead(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "PluginLibrary.dll"));
            Assembly assembly = testAlc.LoadFromStream(fileStream);
            Assert.Equal(string.Empty, assembly.Location);

            // BUG: https://github.com/dotnet/runtime/issues/40087
            Assert.Contains("System.Private.CoreLib", assembly.CodeBase);
            Assert.Contains("System.Private.CoreLib", assembly.EscapedCodeBase);
            Assert.Contains("System.Private.CoreLib", assembly.GetName().CodeBase);
            Assert.Contains("System.Private.CoreLib", assembly.GetName().EscapedCodeBase);
        }

        private void ValidateBundledAssemblyLocation(Assembly assembly)
        {
            if (SingleFileUtilities.IsSingleFile)
            {
                ValidateEmptyAssemblyLocation(assembly);
            }
            else
            {
                ValidateOnDiskAssemblyLocation(assembly);
            }
        }

        private void ValidateEmptyAssemblyLocation(Assembly assembly)
        {
            Assert.Equal(string.Empty, assembly.Location);

            // BUG: https://github.com/dotnet/runtime/issues/40087
            //Assert.Equal(string.Empty, assembly.CodeBase);
            //Assert.Equal(string.Empty, assembly.EscapedCodeBase);
            //Assert.Equal(string.Empty, assembly.GetName().CodeBase);
            //Assert.Equal(string.Empty, assembly.GetName().EscapedCodeBase);

            Assert.Contains(assembly.GetName().Name, assembly.CodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.EscapedCodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.GetName().CodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.GetName().EscapedCodeBase);
        }

        private void ValidateOnDiskAssemblyLocation(Assembly assembly)
        {
            Assert.Contains(assembly.GetName().Name, assembly.Location);
            Assert.Contains(assembly.GetName().Name, assembly.CodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.EscapedCodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.GetName().CodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.GetName().EscapedCodeBase);
        }
    }
}
