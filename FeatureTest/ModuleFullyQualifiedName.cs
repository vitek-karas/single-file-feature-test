using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class ModuleFullyQualifiedName
    {
        [Fact]
        public void FullyQualifiedNameOfCoreLib() => ValidateFrameworkAssemblyFullyQualifiedName(typeof(object).Assembly);

        [Fact]
        public void FullyQualifiedNameOfAppAssembly() => ValidateApplicationAssemblyFullyQualifiedName(typeof(ModuleFullyQualifiedName).Assembly);

        [Fact]
        public void FullyQualifiedNameOfProjectReferenceAssembly() => ValidateApplicationAssemblyFullyQualifiedName(typeof(DeploymentUtilities).Assembly);

        [Fact]
        public void FullyQualifiedNameOfPackageReferenceAssembly() => ValidateApplicationAssemblyFullyQualifiedName(typeof(Assert).Assembly);

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

        void ValidateFrameworkAssemblyFullyQualifiedName(Assembly assembly)
        {
            var module = GetModule(assembly);
            if (DeploymentUtilities.IsSingleFile && DeploymentUtilities.IsSelfContained)
            {
                ValidateUnknownModuleFullyQualifiedName(module);
            }
            else
            {
                ValidateOnDiskModuleFullyQualifiedName(module);
            }
        }

        void ValidateApplicationAssemblyFullyQualifiedName(Assembly assembly)
        {
            var module = GetModule(assembly);
            if (DeploymentUtilities.IsSingleFile)
            {
                ValidateUnknownModuleFullyQualifiedName(module);
            }
            else
            {
                ValidateOnDiskModuleFullyQualifiedName(module);
            }
        }

        Module GetModule(Assembly assembly)
        {
            var modules = assembly.GetModules();
            Assert.Equal(1, modules.Length);
            return modules[0];
        }

        void ValidateUnknownModuleFullyQualifiedName(Module module)
        {
            // BUG https://github.com/dotnet/runtime/issues/40103
            if (DeploymentUtilities.IsSingleFile && RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && DeploymentUtilities.IsSelfContained)
            {
                Assert.Throws<FileNotFoundException>(() =>
                {
                    _ = module.Name;
                });
                Assert.Throws<FileNotFoundException>(() =>
                {
                    _ = module.FullyQualifiedName;
                });
            }
            else
            {
                Assert.Equal("<Unknown>", module.Name);
                Assert.Equal("<Unknown>", module.FullyQualifiedName);
            }

            Assert.Equal(module.Assembly.GetName().Name + ".dll", module.ScopeName);
        }

        void ValidateOnDiskModuleFullyQualifiedName(Module module)
        {
            Assert.NotEqual("<Unknown>", module.Name);
            Assert.Contains(module.Name, module.FullyQualifiedName);
            Assert.Equal(module.Assembly.GetName().Name + ".dll", module.ScopeName);
        }
    }
}
