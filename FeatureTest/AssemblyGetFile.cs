﻿using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class AssemblyGetFile
    {
        [Fact]
        public void GetFilesOnCoreLib() => ValidateAssemblyGetFiles(typeof(object).Assembly);

        [Fact]
        public void GetFilesOnAppAssembly() => ValidateAssemblyGetFiles(typeof(ModuleFullyQualifiedName).Assembly);

        [Fact]
        public void GetFilesOnProjectReferenceAssembly() => ValidateAssemblyGetFiles(typeof(DeploymentUtilities).Assembly);

        [Fact]
        public void GetFilesOnPackageReferenceAssembly() => ValidateAssemblyGetFiles(typeof(Assert).Assembly);

        [Fact]
        public void GetFilesOnExternalAssembly()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(GetFilesOnExternalAssembly));
            Assembly pluginAssembly = testAlc.LoadFromAssemblyPath(Path.Join(DeploymentUtilities.ExecutableLocation, "PluginLibrary.dll"));
            ValidateOnDiskGetFiles(pluginAssembly);
        }

        [Fact]
        public void GetFilesOnAssemblyLoadedFromStream()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(GetFilesOnAssemblyLoadedFromStream));
            using var fileStream = File.OpenRead(Path.Join(DeploymentUtilities.ExecutableLocation, "PluginLibrary.dll"));
            Assembly assembly = testAlc.LoadFromStream(fileStream);
            ValidateInMemoryGetFiles(assembly);
        }

        static void ValidateAssemblyGetFiles(Assembly assembly)
        {
            if (DeploymentUtilities.IsAssemblyInSingleFile(assembly.GetName().Name))
            {
                ValidateInMemoryGetFiles(assembly);
            }
            else
            {
                ValidateOnDiskGetFiles(assembly);
            }
        }

        static void ValidateInMemoryGetFiles(Assembly assembly)
        {
#pragma warning disable IL3001
            Assert.Throws<FileNotFoundException>(() => assembly.GetFiles());
            Assert.Throws<FileNotFoundException>(() => assembly.GetFile(assembly.GetName().Name + ".dll"));
#pragma warning restore IL3001
        }

        static void ValidateOnDiskGetFiles(Assembly assembly)
        {
#pragma warning disable IL3001
            var files = assembly.GetFiles();
#pragma warning restore IL3001
            Assert.Equal(1, files.Length);
            var file = files[0];
            Assert.Contains(assembly.GetName().Name, file.Name);

#pragma warning disable IL3001
            file = assembly.GetFile(assembly.GetName().Name + ".dll");
#pragma warning restore IL3001
            Assert.Contains(assembly.GetName().Name, file.Name);
        }
    }
}
