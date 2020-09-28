using System;
using System.IO;
using UtilitiesLibrary;
using Xunit;
using Xunit.Sdk;

namespace FeatureTest
{
    public class RuntimeProperties
    {
        [Fact]
        public void AppContextDepsFiles()
        {
            string appContextDepsFiles = (string)AppContext.GetData("APP_CONTEXT_DEPS_FILES");
            string expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FeatureTest.deps.json");
            if (DeploymentUtilities.IsAssemblyInSingleFile(typeof(RuntimeProperties).Assembly.GetName().Name))
            {
                Assert.DoesNotContain(expectedPath, appContextDepsFiles);
            }
            else
            {
                Assert.Contains(expectedPath, appContextDepsFiles);
            }

            foreach (string path in appContextDepsFiles.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!File.Exists(path))
                {
                    throw new XunitException($"APP_CONTEXT_DEPS_FILES contains paths '{path}' which doesn't exist.");
                }
            }
        }
    }
}
