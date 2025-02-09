using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Route53;
using Constructs;

namespace BDiazECertificate
{
    public class BDiazECertificateStack : Stack
    {
        internal BDiazECertificateStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            string appName = System.Environment.GetEnvironmentVariable("APP_NAME")!;
            string domainName = System.Environment.GetEnvironmentVariable("DOMAIN_NAME");
            string[] alternativesNames = System.Environment.GetEnvironmentVariable("ALTERNATIVES_NAMES").Split(",");

            // Se setean los hosteds zones para validación
            IHostedZone hostedZone = HostedZone.FromLookup(this, $"{appName}HostedZone", new HostedZoneProviderProps {
                DomainName = domainName,
            });

            // Se crea certificado SSL para el dominio
            Certificate certificate = new Certificate(this, $"{appName}Certificate", new CertificateProps {
                CertificateName = $"{appName}Certificate",
                DomainName = domainName,
                SubjectAlternativeNames = alternativesNames,
                Validation = CertificateValidation.FromDns(hostedZone),
            });
        }
    }
}
