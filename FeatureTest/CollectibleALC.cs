using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class CollectibleALC
    {
        [Fact]
        public void LoadAppAssemblyAndUnload() => ValidateLoadIntoALCAndUnload(nameof(LoadAppAssemblyAndUnload), "FeatureTest");

        [Fact]
        public void LoadReferencesAssemblyAndUnload() => ValidateLoadIntoALCAndUnload(nameof(LoadAppAssemblyAndUnload), "UtilitiesLibrary");

        [Fact]
        public void LoadFrameworkAssemblyFromPathAndUnload()
        {
            var assembly = typeof(System.Xml.XmlReader).Assembly;
#pragma warning disable IL3000
            string fullPath = DeploymentUtilities.IsSelfContained
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly.GetName().Name + ".dll")
                : assembly.Location;
#pragma warning restore IL3000

            if (DeploymentUtilities.IsAssemblyInSingleFile(assembly.GetName().Name))
            {
                Assert.Throws<FileNotFoundException>(() => Assembly.LoadFrom(fullPath));
            }
            else
            {
                ValidateLoadFromPathIntoALCAndUnload(nameof(LoadFrameworkAssemblyFromPathAndUnload), fullPath);
            }
        }

        [Fact]
        public void LoadExternalAssemblyFromPathAndUnload() =>
            ValidateLoadFromPathIntoALCAndUnload(nameof(LoadExternalAssemblyFromPathAndUnload), Path.Combine(DeploymentUtilities.ExecutableLocation, "PluginLibrary.dll"));

        private void ValidateLoadIntoALCAndUnload(string alcName, string assemblyName)
        {
            var alcReference = LoadIntoALCAndUnload(alcName, assemblyName);
            while (alcReference.IsAlive)
            {
                GC.Collect(2, GCCollectionMode.Forced, true);
                Thread.Sleep(100);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private WeakReference LoadIntoALCAndUnload (string alcName, string assemblyName)
        {
            AssemblyLoadContext ctx = new AssemblyLoadContext(nameof(LoadAppAssemblyAndUnload), isCollectible: true);
            Assembly asm = ctx.LoadFromAssemblyName(new AssemblyName("FeatureTest"));
            ctx.Unload();
            return new WeakReference(ctx);
        }

        private void ValidateLoadFromPathIntoALCAndUnload(string alcName, string assemblyPath)
        {
            var alcReference = LoadFromPathIntoALCAndUnload(alcName, assemblyPath);
            while (alcReference.IsAlive)
            {
                GC.Collect(2, GCCollectionMode.Forced, true);
                Thread.Sleep(100);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private WeakReference LoadFromPathIntoALCAndUnload(string alcName, string assemblyPath)
        {
            AssemblyLoadContext ctx = new AssemblyLoadContext(nameof(LoadAppAssemblyAndUnload), isCollectible: true);
            Assembly asm = ctx.LoadFromAssemblyPath(assemblyPath);
            ctx.Unload();
            return new WeakReference(ctx);
        }
    }
}
