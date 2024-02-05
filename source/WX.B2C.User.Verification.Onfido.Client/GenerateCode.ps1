Remove-Item ./Generated/* -Recurse

Echo "Generate client"
autorest --version=3.4.5 `
        --input-file=swagger.json `
        --output-folder=./Generated `
        --csharp --use:@microsoft.azure/autorest.csharp@2.3.91 `
        --namespace=WX.B2C.User.Verification.Onfido.Client `
        --sync-methods=none `
        --override-client-name=OnfidoApiClient `
        --add-credentials