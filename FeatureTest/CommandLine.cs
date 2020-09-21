using System;
using System.IO;
using UtilitiesLibrary;
using Xunit;

namespace FeatureTest
{
    public class CommandLine
    {
        [Fact]
        public void CommandLineZeroArgument()
        {
            if (DeploymentUtilities.IsSingleFile)
            {
                Assert.Equal(Path.Combine(DeploymentUtilities.ExecutableLocation, "FeatureTest.exe"), Environment.GetCommandLineArgs()[0]);
            }
            else
            {
#pragma warning disable IL3000
                Assert.Equal(typeof(CommandLine).Assembly.Location, Environment.GetCommandLineArgs()[0]);
#pragma warning restore IL3000
            }
        }
    }
}
