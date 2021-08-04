using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Configuration;

namespace PurchaseForMe.Core
{
    public static class ConfigurationExtensions
    {
        public static Config CreateConfigurationWithEnvironment(this Config configuration)
        {
            string seedNodes = Environment.GetEnvironmentVariable("CLUSTER_SEED_NODES");
            string port = Environment.GetEnvironmentVariable("CLUSTER_PORT");
            string hostname = Environment.GetEnvironmentVariable("CLUSTER_HOSTNAME");
            string roles = Environment.GetEnvironmentVariable("CLUSTER_ROLES");
            if (!string.IsNullOrEmpty(seedNodes))
            {
                configuration = configuration.WithFallback(
                    ConfigurationFactory.ParseString($"akka.cluster.seed-nodes = [{seedNodes}]"));
            }
            if (!string.IsNullOrEmpty(port))
            {
                configuration = configuration.WithFallback(
                    ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port = {port}"));
            }
            if (!string.IsNullOrEmpty(hostname))
            {
                configuration = configuration.WithFallback(
                    ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.hostname = {hostname}"));
            }
            if (!string.IsNullOrEmpty(roles))
            {
                configuration = configuration.WithFallback(
                    ConfigurationFactory.ParseString($"akka.cluster.roles = [{roles}]"));
            }

            return configuration;
        }
    }
}