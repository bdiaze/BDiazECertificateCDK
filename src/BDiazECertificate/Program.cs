using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BDiazECertificate
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            string appName = System.Environment.GetEnvironmentVariable("APP_NAME") ?? throw new ArgumentNullException("APP_NAME");
            string account = System.Environment.GetEnvironmentVariable("ACCOUNT_AWS") ?? throw new ArgumentNullException("ACCOUNT_AWS");
            string region = System.Environment.GetEnvironmentVariable("REGION_AWS") ?? throw new ArgumentNullException("REGION_AWS");
            string secondaryRegion = System.Environment.GetEnvironmentVariable("SECONDARY_REGION_AWS") ?? throw new ArgumentNullException("SECONDARY_REGION_AWS");

            var app = new App();

            // Despliegue en región primaria...
            _ = new BDiazECertificateStack(app, $"Cdk{appName}Certificate", new StackProps {
                Env = new Amazon.CDK.Environment {
                    Account = account,
                    Region = region,
                }
            });

            // Despliegue en región secundaria...
            _ = new BDiazECertificateStack(app, $"Cdk{appName}Certificate", new StackProps {
                Env = new Amazon.CDK.Environment {
                    Account = account,
                    Region = secondaryRegion,
                }
            });

            app.Synth();
        }
    }
}
