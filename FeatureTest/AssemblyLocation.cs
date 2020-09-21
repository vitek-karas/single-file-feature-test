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
        public void LocationOfCoreLib() => ValidateAssemblyLocation(typeof(object).Assembly);

        [Fact]
        public void LocationOfAppAssembly() => ValidateAssemblyLocation(typeof(AssemblyLocation).Assembly);

        [Fact]
        public void LocationOfProjectReferenceAssembly() => ValidateAssemblyLocation(typeof(DeploymentUtilities).Assembly);

        [Fact]
        public void LocationOfPackageReferenceAssembly() => ValidateAssemblyLocation(typeof(Assert).Assembly);

        [Fact]
        public void LocationOfExternalAssembly()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(LocationOfExternalAssembly));
            Assembly pluginAssembly = testAlc.LoadFromAssemblyPath(Path.Join(DeploymentUtilities.ExecutableLocation, "PluginLibrary.dll"));
            ValidateOnDiskAssemblyLocation(pluginAssembly);
        }

        [Fact]
        public void LocationOfAssemblyLoadedFromStream()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(LocationOfAssemblyLoadedFromStream));
            using var fileStream = File.OpenRead(Path.Join(DeploymentUtilities.ExecutableLocation, "PluginLibrary.dll"));
            Assembly assembly = testAlc.LoadFromStream(fileStream);
            Assert.Equal(string.Empty, assembly.Location);

#pragma warning disable SYSLIB0012
            Assert.Contains("System.Private.CoreLib", assembly.CodeBase);
            Assert.Contains("System.Private.CoreLib", assembly.EscapedCodeBase);
#pragma warning restore SYSLIB0012
            Assert.Contains("System.Private.CoreLib", assembly.GetName().CodeBase);
            Assert.Contains("System.Private.CoreLib", assembly.GetName().EscapedCodeBase);

            Assert.False(assembly.IsDynamic);
        }

        private void ValidateAssemblyLocation(Assembly assembly)
        {
            if (DeploymentUtilities.IsAssemblyInSingleFile(assembly.GetName().Name))
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

#pragma warning disable SYSLIB0012
            Assert.Throws<NotSupportedException>(() => assembly.CodeBase);
            Assert.Throws<NotSupportedException>(() => assembly.EscapedCodeBase);
#pragma warning restore SYSLIB0012
            Assert.Null(assembly.GetName().CodeBase);
            Assert.Null(assembly.GetName().EscapedCodeBase);
            Assert.False(assembly.IsDynamic);
        }

        private void ValidateOnDiskAssemblyLocation(Assembly assembly)
        {
            Assert.Contains(assembly.GetName().Name, assembly.Location);
#pragma warning disable SYSLIB0012
            Assert.Contains(assembly.GetName().Name, assembly.CodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.EscapedCodeBase);
#pragma warning restore SYSLIB0012
            Assert.Contains(assembly.GetName().Name, assembly.GetName().CodeBase);
            Assert.Contains(assembly.GetName().Name, assembly.GetName().EscapedCodeBase);
            Assert.False(assembly.IsDynamic);
        }
    }
}
