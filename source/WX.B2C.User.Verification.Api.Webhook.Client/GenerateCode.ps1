$Response = Invoke-WebRequest -Uri http://localhost:18722/swagger/v1/swagger.json -Method GET -UseBasicParsing
if ($Response.StatusCode.Equals(200))
{
   Echo "Update swagger file"
   Set-Content -Path 'swagger.json' -Value $Response.Content
}

Remove-Item ./Generated -Include *.cs -Recurse

Echo "Generate client"
autorest --version=3.4.5 `
        --input-file=swagger.json `
        --output-folder=./Generated  `
        --csharp --use:@microsoft.azure/autorest.csharp@2.3.91 `
        --namespace=WX.B2C.User.Verification.Api.Webhook.Client `
        --sync-methods=none `
        --override-client-name=WebhookApiClient