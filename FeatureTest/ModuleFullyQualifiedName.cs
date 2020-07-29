using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class ModuleFullyQualifiedName
    {
        [Fact]
        public void FullyQualifiedNameOfCoreLib() => ValidateBundledFullyQualifiedName(typeof(object).Assembly);

        [Fact]
        public void FullyQualifiedNameOfAppAssembly() => ValidateBundledFullyQualifiedName(typeof(ModuleFullyQualifiedName).Assembly);

        [Fact]
        public void FullyQualifiedNameOfProjectReferenceAssembly() => ValidateBundledFullyQualifiedName(typeof(SingleFileUtilities).Assembly);

        [Fact]
        public void FullyQualifiedNameOfPackageReferenceAssembly() => ValidateBundledFullyQualifiedName(typeof(Assert).Assembly);

        [Fact]
        public void FullyQualifiedNameOfExternalAssembly()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(FullyQualifiedNameOfExternalAssembly));
            Assembly pluginAssembly = testAlc.LoadFromAssemblyPath(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "PluginLibrary.dll"));
            ValidateOnDiskModuleFullyQualifiedName(pluginAssembly.GetModules()[0]);
        }

        [Fact]
        public void FullyQualifiedNameOfAssemblyLoadedFromStream()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(FullyQualifiedNameOfAssemblyLoadedFromStream));
            using var fileStream = File.OpenRead(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "PluginLibrary.dll"));
            Assembly assembly = testAlc.LoadFromStream(fileStream);
            ValidateUnknownModuleFullyQualifiedName(assembly.GetModules()[0]);
        }

        void ValidateBundledFullyQualifiedName(Assembly assembly)
        {
            var modules = assembly.GetModules();
            Assert.Equal(1, modules.Length);
            var module = modules[0];

            if (SingleFileUtilities.IsSingleFile)
            {
                ValidateUnknownModuleFullyQualifiedName(module);
            }
            else
            {
                ValidateOnDiskModuleFullyQualifiedName(module);
            }
        }

        void ValidateUnknownModuleFullyQualifiedName(Module module)
        {
            // BUG https://github.com/dotnet/runtime/issues/40103
            if (SingleFileUtilities.IsSingleFile)
            {
                Assert.Throws<FileNotFoundException>(() =>
                {
                    _ = module.FullyQualifiedName;
                });
            }
            else
            {
                Assert.Equal("<Unknown>", module.FullyQualifiedName);
            }
        }

        void ValidateOnDiskModuleFullyQualifiedName(Module module)
        {
            Assert.Contains(module.Name, module.FullyQualifiedName);
        }
    }
}
