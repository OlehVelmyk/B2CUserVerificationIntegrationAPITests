Remove-Item ./Generated/* -Recurse

Echo "Generate client"
autorest --version=3.4.5 `
        --input-file=openapi.json `
        --output-folder=./Generated `
        --csharp --use:@microsoft.azure/autorest.csharp@2.3.91 `
        --namespace=WX.B2C.User.Verification.PassFort.Client `
        --sync-methods=none `
        --override-client-name=PassFortApiClient `
        --add-credentials `
        --verbose