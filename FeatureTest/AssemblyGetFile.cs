using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class AssemblyGetFile
    {
        [Fact]
        public void GetFilesOnCoreLib() => ValidateBundlededGetFiles(typeof(object).Assembly);

        [Fact]
        public void GetFilesOnAppAssembly() => ValidateBundlededGetFiles(typeof(ModuleFullyQualifiedName).Assembly);

        [Fact]
        public void GetFilesOnProjectReferenceAssembly() => ValidateBundlededGetFiles(typeof(SingleFileUtilities).Assembly);

        [Fact]
        public void GetFilesOnPackageReferenceAssembly() => ValidateBundlededGetFiles(typeof(Assert).Assembly);

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

        void ValidateBundlededGetFiles(Assembly assembly)
        {
            if (SingleFileUtilities.IsSingleFile)
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
            if (SingleFileUtilities.IsSingleFile)
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
