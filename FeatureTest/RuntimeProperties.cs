using System;
using System.IO;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class RuntimeProperties
    {
        [Fact]
        public void AppContextDepsFiles()
        {
            string appContextDepsFiles = (string)AppContext.GetData("APP_CONTEXT_DEPS_FILES");
            string expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FeatureTest.deps.json");
            if (DeploymentUtilities.IsSingleFile)
            {
                Assert.DoesNotContain(expectedPath, appContextDepsFiles);
            }
            else
            {
                Assert.Contains(expectedPath, appContextDepsFiles);
            }
        }
    }
}
