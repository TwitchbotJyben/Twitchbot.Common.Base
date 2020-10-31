
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Twitchbot.Common.Base.Test
{
    /// <summary>
    /// Provides methods for units tests.
    /// </summary>
    /// <typeparam name="T">The type of test class.</typeparam>
    public abstract class BaseApiTest<T> where T: class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BaseApiTest{T}"/>.
        /// </summary>
        public BaseApiTest()
        {
        }

        /// <summary>
        /// Initializes a <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="fileName">The name of the configuration file.</param>
        /// <returns>The <see cref="IConfiguration"/>.</returns>
        public virtual IConfiguration InitializeConfiguration(string fileName)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(fileName)
                .AddUserSecrets<T>()
                .Build();
            return config;
        }

        /// <summary>
        /// Initializes a <see cref="WebHostBuilder"/>.
        /// </summary>
        /// <typeparam name="T">The startup class.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public virtual IWebHostBuilder InitializeWebHostBuilder<TStartup>(IConfiguration configuration) where TStartup: class
        {
            return new WebHostBuilder().UseConfiguration(configuration).UseStartup<TStartup>();
        }
    }
}
