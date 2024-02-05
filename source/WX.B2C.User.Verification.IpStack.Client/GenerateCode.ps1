Remove-Item *.cs -Recurse
Remove-Item Models/*.cs

Echo "Generate client"
autorest --version=3.4.5 `
        --input-file=openapi.json `
        --output-folder=./ `
        --csharp --use:@microsoft.azure/autorest.csharp@2.3.91 `
        --namespace=WX.B2C.User.Verification.IpStack.Client `
        --sync-methods=none `
        --override-client-name=IpStackApiClient `
        --verbose