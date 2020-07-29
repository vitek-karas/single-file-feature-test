using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace FeatureTest
{
    public class AssemblyGetFile
    {
        //[Fact]
        public void GetFiles()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(GetFiles));
            using var fileStream = File.OpenRead(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "PluginLibrary.dll"));
            Assembly assembly = testAlc.LoadFromStream(fileStream);

            Console.WriteLine(assembly.GetModules()[0].FullyQualifiedName);
            Console.WriteLine(assembly.GetFiles()[0].Name);

            foreach (var m in typeof(AssemblyGetFile).Assembly.GetModules())
            {
                Console.WriteLine(m.FullyQualifiedName);
            }

            foreach (var v in typeof(AssemblyGetFile).Assembly.GetFiles())
            {
                Console.WriteLine(v.Name);
            }
        }
    }
}
