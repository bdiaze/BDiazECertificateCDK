# Certificados AWS con CDK .NET

- [Certificados AWS con CDK .NET](#certificados-aws-con-cdk-net)
  - [Introducción](#introducción)
  - [Recursos Requeridos](#recursos-requeridos)
    - [Zona Hospedada](#zona-hospedada)
  - [Recursos Creados](#recursos-creados)
    - [Certificado](#certificado)
  - [Despliegue](#despliegue)
    - [Variables y Secretos de Entorno](#variables-y-secretos-de-entorno)

## Introducción

* El siguiente repositorio es para crear y validar un certificado usando el servicio [AWS Certificate Manager (ACM)](https://aws.amazon.com/es/certificate-manager). 
* La infraestructura se despliega mediante IaC, usando [AWS CDK en .NET 8.0](https://docs.aws.amazon.com/cdk/api/v2/dotnet/api/).
* El despliegue CI/CD se lleva a cabo mediante  [GitHub Actions](https://github.com/features/actions).
* El mismo certificado es creado en una región primaria y en una región secundaria.

## Recursos Requeridos

### Zona Hospedada

Dado que el certificado será validado mediante DNS, es necesario contar con una Zona Hospedada del dominio para el cual se creará el certificado.

<ins>Código para obtener Zona Hospedada existente:</ins>
```csharp
using Amazon.CDK.AWS.Route53;

IHostedZone ... = HostedZone.FromLookup(this, ..., new HostedZoneProviderProps {
    DomainName = ...,
});
```

## Recursos Creados

### Certificado

Una vez se cuenta con los recursos requeridos, es posible proceder con la creación del certificado.

<ins>Código para crear Certificado:</ins>
```csharp
using Amazon.CDK.AWS.CertificateManager;

Certificate ... = new(this, ..., new CertificateProps {
    CertificateName = ...,
    DomainName = ...,
    SubjectAlternativeNames = ...,
    Validation = CertificateValidation.FromDns(...),
});
```

> [!NOTE]
> La sentencia `CertificateValidation.FromDns(...)` es utilizada para indicarle a CDK que, una vez creado el certificado, deberá crear un registro CName en la Zona Hospedada para validar el certificado.

## Despliegue

El despliegue se lleva a cabo mediante GitHub Actions, para ello se configura la receta de despliegue con los siguientes pasos:

| Paso | Comando | Descripción |
|------|---------|-------------|
| Checkout Repositorio | `actions/checkout@v4` | Se descarga el repositorio en runner. |
| Instalar .NET | `actions/setup-dotnet@v4` | Se instala .NET en el runner. |
| Instalar Node.js | `actions/setup-node@v4` | Se instala Node.js en el runner. | 
| Instalar AWS CDK | `npm install -g aws-cdk` | Se instala aws-cdk con NPM. |
| Configure AWS Credentials | `aws-actions/configure-aws-credentials` | Se configuran credenciales para despliegue en AWS. |
| CDK Synth | `cdk synth` | Se sintetiza la aplicación CDK. |
| CDK Diff | `cdk --app cdk.out diff` | Se obtienen las diferencias entre nueva versión y versión desplegada. |
| CDK Deploy | `cdk --app cdk.out deploy --require-approval never` | Se despliega la aplicación CDK. |

### Variables y Secretos de Entorno

A continuación se presentan las variables que se deben configurar en el Environment para el correcto despliegue:
| Variable de Entorno | Tipo | Descripción |
|---------------------|------|-------------|
| `VERSION_DOTNET` | Variable | Versión del .NET del CDK. Por ejemplo "8". |
| `VERSION_NODEJS` | Variable | Versión de Node.js. Por ejemplo "20". |
| `ARN_GITHUB_ROLE` | Variable | ARN del Rol en IAM que se usará para el despliegue. |
| `ACCOUNT_AWS` | Variable | ID de la cuenta AWS donde desplegar. |
| `REGION_AWS` | Variable | Región primaria donde desplegar. Por ejemplo "us-west-1". |
| `SECONDARY_REGION_AWS` | Variable | Región secundaria donde desplegar. Por ejemplo "us-east-1". |
| `DIRECTORIO_CDK` | Variable | Directorio donde se encuentra archivo cdk.json. En este caso sería ".". |
| `APP_NAME` | Variable | El nombre de la aplicación a desplegar. |
| `DOMAIN_NAME` | Variable | El nombre del dominio asociado a la Zona Hospedada y al Certificado que se desea crear. Por ejemplo "example.com". |
| `ALTERNATIVES_NAMES` | Variable | Nombre alternativo del dominio para el certificado. Por ejemplo "*.example.com". |