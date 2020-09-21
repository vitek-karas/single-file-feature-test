using System;
using System.IO;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class AppDomainBaseDirectory
    {
        [Fact]
        public void BaseDirectory()
        {
            // We need to somehow detect the 3.1 backward compat case where single-file extracts everything onto disk
            // The application assembly seems like the best detection mechanism - if it's a single-file
            // but even the application assembly is not loaded from bundle -> fully extracted.
            if (DeploymentUtilities.IsSingleFile && !DeploymentUtilities.IsAssemblyInSingleFile(typeof(AppDomainBaseDirectory).Assembly.GetName().Name))
            {
                Assert.NotEqual(DeploymentUtilities.ExecutableLocation, AppDomain.CurrentDomain.BaseDirectory);
                Assert.Equal(Path.GetDirectoryName(typeof(AppDomainBaseDirectory).Assembly.Location), AppDomain.CurrentDomain.BaseDirectory);
            }
            else
            {
                Assert.Equal(DeploymentUtilities.ExecutableLocation, AppDomain.CurrentDomain.BaseDirectory);
            }
        }
    }
}
