module SecretKeyConsolePrinter

open QRCoder

let displayQrCode (secretKey: string) =
    // Generate the OTP URI
    let otpAuthUri = sprintf "otpauth://totp/MyApp:user?secret=%s&issuer=MyApp" secretKey
    
    // Generate QR code
    let qrGenerator = new QRCodeGenerator()
    let qrCodeData = qrGenerator.CreateQrCode(otpAuthUri, QRCodeGenerator.ECCLevel.Q)
    
    // Get access to the raw bit matrix
    // Most QR code libraries provide a way to access this directly
    let matrix = qrCodeData.ModuleMatrix
    
    // Print the matrix
    for y in 0..(matrix.Count - 1) do
        for x in 0..(matrix.[y].Count - 1) do
            printf "%s" (if matrix.[y].[x] then "██" else "  ")
        printfn ""

