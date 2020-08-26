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
            // BUG https://github.com/dotnet/runtime/issues/41267
            //if (DeploymentUtilities.IsSingleFile)
            //{
            //    Assert.Equal("", appContextDepsFiles);
            //}
            //else
            {
                string expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FeatureTest.deps.json");
                Assert.Contains(expectedPath, appContextDepsFiles);
            }
        }
    }
}
