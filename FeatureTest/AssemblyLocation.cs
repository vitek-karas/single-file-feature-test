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
        public void LocationOfCoreLib() => ValidateFrameworkAssemblyLocation(typeof(object).Assembly);

        [Fact]
        public void LocationOfAppAssembly() => ValidateApplicationAssemblyLocation(typeof(AssemblyLocation).Assembly);

        [Fact]
        public void LocationOfProjectReferenceAssembly() => ValidateApplicationAssemblyLocation(typeof(DeploymentUtilities).Assembly);

        [Fact]
        public void LocationOfPackageReferenceAssembly() => ValidateApplicationAssemblyLocation(typeof(Assert).Assembly);

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

            Assert.False(assembly.IsDynamic);
        }

        private void ValidateFrameworkAssemblyLocation(Assembly assembly)
        {
            if (DeploymentUtilities.IsSingleFile && DeploymentUtilities.IsSelfContained)
            {
                ValidateInMemoryAssemblyLocation(assembly);
            }
            else
            {
                ValidateOnDiskAssemblyLocation(assembly);
            }
        }

        private void ValidateApplicationAssemblyLocation(Assembly assembly)
        {
            if (DeploymentUtilities.IsSingleFile)
            {
                ValidateInMemoryAssemblyLocation(assembly);
            }
            else
            {
                ValidateOnDiskAssemblyLocation(assembly);
            }
        }

        private void ValidateInMemoryAssemblyLocation(Assembly assembly)
        {
            Assert.Equal(string.Empty, assembly.Location);

            Assert.Throws<NotSupportedException>(() => assembly.CodeBase);
            Assert.Throws<NotSupportedException>(() => assembly.EscapedCodeBase);
            Assert.Null(assembly.GetName().CodeBase);
            Assert.Null(assembly.GetName().EscapedCodeBase);
            Assert.False(assembly.IsDynamic);
        }

        private void ValidateOnDiskAssemblyLocation(Assembly assembly)
        {
            Assert.Contains(assembly.GetName().Name, assembly.Location);
            Assert.Contains(assembly.GetName().Name, assembly.CodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.EscapedCodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.GetName().CodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.GetName().EscapedCodeBase);
            Assert.False(assembly.IsDynamic);
        }
    }
}
