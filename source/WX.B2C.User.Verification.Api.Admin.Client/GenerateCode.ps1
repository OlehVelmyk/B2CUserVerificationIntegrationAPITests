function RemoveMultipartFormData
{
   param ($swagger)

   $newPaths = $swagger.paths
   foreach ($element in $newPaths.PSObject.Properties) {
       $objName = $element.Name
       $objValue = $element.Value 
       if (!($objValue.post.requestBody.content.'multipart/form-data' -eq $null))
       {
            Write-Host "Removed object $objName -- $propName"
            $newPaths.PSObject.Properties.Remove($objName)
       }

       if ($objValue.get.operationId -eq 'Files_Download')
       {
           Write-Host "Removed object $objName -- $propName"
           $newPaths.PSObject.Properties.Remove($objName)
       }
   }

   $swagger.paths = $newPaths
}

add-type @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public class TrustAllCertsPolicy : ICertificatePolicy {
    public bool CheckValidationResult(
        ServicePoint srvPoint, X509Certificate certificate,
        WebRequest request, int certificateProblem) {
        return true;
    }
}
"@
$AllProtocols = [System.Net.SecurityProtocolType]'Ssl3,Tls,Tls11,Tls12'
[System.Net.ServicePointManager]::SecurityProtocol = $AllProtocols
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
$Response = Invoke-WebRequest -Uri https://localhost:17622/swagger/v1/swagger.json -Method GET -UseBasicParsing
if ($Response.StatusCode.Equals(200))
{
   Echo "Update swagger file"
   $swagger = $Response.Content | ConvertFrom-Json
   RemoveMultipartFormData($swagger)
   $content = $swagger | ConvertTo-Json -Depth 100 -Compress
   Set-Content -Path 'swagger.json' -Value $content
}

Remove-Item ./Generated -Include *.cs -Recurse

Echo "Generate client"
autorest --version=3.4.5 `
        --input-file=swagger.json `
        --output-folder=./Generated `
        --csharp `
        --use:@microsoft.azure/autorest.csharp@2.3.91 `
        --namespace=WX.B2C.User.Verification.Api.Admin.Client `
        --sync-methods=none `
        --override-client-name=UserVerificationApiClient `
        --add-credentials