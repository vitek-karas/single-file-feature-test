using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FeatureTest
{
    public class LoadFromAssemblyPath
    {
        [Fact]
        public void LoadExternalAssembly()
        {
            AssemblyLoadContext testAlc = new AssemblyLoadContext(nameof(LoadExternalAssembly));
            string filePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "PluginLibrary.dll");
            Assembly pluginAssembly = testAlc.LoadFromAssemblyPath(filePath);
            Assembly pluginAssemblySecondLoad = testAlc.LoadFromAssemblyPath(filePath);
            Assert.Same(pluginAssembly, pluginAssemblySecondLoad);
        }
    }
}
