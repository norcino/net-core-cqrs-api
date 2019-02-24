using Microsoft.AspNetCore.Hosting;

namespace Common.IoC
{
    public static class HostingEnvironmentExtensions
    {
        public static bool IsTesting(this IHostingEnvironment env)
        {
            return env.EnvironmentName == "Test";
        }
    }
}