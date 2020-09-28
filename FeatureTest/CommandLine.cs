using System;
using System.IO;
using System.Runtime.InteropServices;
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
                string executableName = typeof(CommandLine).Assembly.GetName().Name;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    executableName += ".exe";
                }

                Assert.Equal(Path.Combine(DeploymentUtilities.ExecutableLocation, executableName), Environment.GetCommandLineArgs()[0]);
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
