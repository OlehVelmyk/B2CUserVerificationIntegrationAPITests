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
   }

   $swagger.paths = $newPaths
}

$Response = Invoke-WebRequest -Uri http://localhost:17522/swagger/v1/swagger.json -Method GET -UseBasicParsing
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
        --output-folder=./Generated  `
        --csharp --use:@microsoft.azure/autorest.csharp@2.3.91 `
        --namespace=WX.B2C.User.Verification.Api.Internal.Client `
        --sync-methods=none `
        --override-client-name=UserVerificationApiClient