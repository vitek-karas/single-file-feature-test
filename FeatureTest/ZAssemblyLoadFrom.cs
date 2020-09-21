using System;
using System.IO;
using System.Reflection;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    // These tests load additional assemblies into the Default context and it's not possible to isolate that
    // So we rely on the fact that tests are executed in alphabetical order and that this one runs last.
    public class ZAssemblyLoadFrom
    {
        [Fact]
        public void LoadFromFrameworkAssembly()
        {
            // Note: We're not testing CoreLib here since loading CoreLib explicitly from file is forbidden

            var assembly = typeof(System.Xml.XmlReader).Assembly;
            string fullPath = DeploymentUtilities.IsAssemblyInSingleFile(assembly.GetName().Name)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly.GetName().Name + ".dll")
                : assembly.Location;

            if (DeploymentUtilities.IsAssemblyInSingleFile(assembly.GetName().Name))
            {
                Assert.Throws<FileNotFoundException>(() => Assembly.LoadFrom(fullPath));
            }
            else
            {
                ValidateLoadFromFile(fullPath);
            }
        }

        [Fact]
        public void LoadFromApplicationAssembly() => ValidateLoadFromInAppAssembly(typeof(ZAssemblyLoadFrom).Assembly);

        [Fact]
        public void LoadFromProjectReferenceAssembly() => ValidateLoadFromInAppAssembly(typeof(DeploymentUtilities).Assembly);

        [Fact]
        public void LoadFromPackageReferenceAssembly() => ValidateLoadFromInAppAssembly(typeof(Assert).Assembly);

        [Fact]
        public void LoadFromExternalAssembly() => ValidateLoadFromFile(Path.Combine(DeploymentUtilities.ExecutableLocation, "PluginLibrary.dll"));

        void ValidateLoadFromInAppAssembly(Assembly assembly)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly.GetName().Name + ".dll");
            if (DeploymentUtilities.IsAssemblyInSingleFile(assembly.GetName().Name))
            {
                Assert.Throws<FileNotFoundException>(() => Assembly.LoadFrom(fullPath));
            }
            else
            {
                ValidateLoadFromFile(fullPath);
            }
        }

        void ValidateLoadFromFile(string fullPath)
        {
            var assembly = Assembly.LoadFrom(fullPath);
            Assert.NotNull(assembly);
            Assert.Equal(Path.GetFileNameWithoutExtension(fullPath), assembly.GetName().Name);
        }
    }
}
