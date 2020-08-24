using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class AssemblyGetFile
    {
        [Fact]
        public void GetFilesOnCoreLib() => ValidateFrameworkAssemblyGetFiles(typeof(object).Assembly);

        [Fact]
        public void GetFilesOnAppAssembly() => ValidateApplicationAssemblyGetFiles(typeof(ModuleFullyQualifiedName).Assembly);

        [Fact]
        public void GetFilesOnProjectReferenceAssembly() => ValidateApplicationAssemblyGetFiles(typeof(DeploymentUtilities).Assembly);

        [Fact]
        public void GetFilesOnPackageReferenceAssembly() => ValidateApplicationAssemblyGetFiles(typeof(Assert).Assembly);

        [Fact]
        public void GetFilesOnExternalAssembly()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(GetFilesOnExternalAssembly));
            Assembly pluginAssembly = testAlc.LoadFromAssemblyPath(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "PluginLibrary.dll"));
            ValidateOnDiskGetFiles(pluginAssembly);
        }

        [Fact]
        public void GetFilesOnAssemblyLoadedFromStream()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(GetFilesOnAssemblyLoadedFromStream));
            using var fileStream = File.OpenRead(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "PluginLibrary.dll"));
            Assembly assembly = testAlc.LoadFromStream(fileStream);
            ValidateInMemoryGetFiles(assembly);
        }

        void ValidateFrameworkAssemblyGetFiles(Assembly assembly)
        {
            if (DeploymentUtilities.IsSingleFile && DeploymentUtilities.IsSelfContained)
            {
                ValidateInMemoryGetFiles(assembly);
            }
            else
            {
                ValidateOnDiskGetFiles(assembly);
            }
        }

        void ValidateApplicationAssemblyGetFiles(Assembly assembly)
        {
            if (DeploymentUtilities.IsSingleFile)
            {
                ValidateInMemoryGetFiles(assembly);
            }
            else
            {
                ValidateOnDiskGetFiles(assembly);
            }
        }

        void ValidateInMemoryGetFiles(Assembly assembly)
        {
            // BUG https://github.com/dotnet/runtime/issues/40103
            if (DeploymentUtilities.IsSingleFile || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Throws<FileNotFoundException>(() => assembly.GetFiles());
                Assert.Throws<FileNotFoundException>(() => assembly.GetFile(assembly.GetName().Name + ".dll"));
            }
            else
            {
                Assert.Throws<IOException>(() => assembly.GetFiles());
                Assert.Throws<IOException>(() => assembly.GetFile(assembly.GetName().Name + ".dll"));
            }
        }

        void ValidateOnDiskGetFiles(Assembly assembly)
        {
            var files = assembly.GetFiles();
            Assert.Equal(1, files.Length);
            var file = files[0];
            Assert.Contains(assembly.GetName().Name, file.Name);

            file = assembly.GetFile(assembly.GetName().Name + ".dll");
            Assert.Contains(assembly.GetName().Name, file.Name);
        }
    }
}
