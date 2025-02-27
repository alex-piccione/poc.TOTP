// Completely generated bu Claude 3.7

open System
open System.Xml.Linq
open QRCoder

// Main function to generate and display OTP QR code in the console
let displayOtpQrCode (secretKey: string) (issuer: string) (accountName: string) =
    // Format the OTP URI according to the standard format
    // See: https://github.com/google/google-authenticator/wiki/Key-Uri-Format
    let otpAuthUri = sprintf "otpauth://totp/%s:%s?secret=%s&issuer=%s" 
                            (Uri.EscapeDataString(issuer)) 
                            (Uri.EscapeDataString(accountName)) 
                            secretKey 
                            (Uri.EscapeDataString(issuer))
    
    printfn "Generating QR Code for: %s" otpAuthUri
    
    // Generate QR code
    let qrGenerator = new QRCodeGenerator()
    let qrCodeData = qrGenerator.CreateQrCode(otpAuthUri, QRCodeGenerator.ECCLevel.M)
    
    // Get QR code as SVG
    let svgQrCode = new SvgQRCode(qrCodeData)
    let svgString = svgQrCode.GetGraphic(20)
    
    // Parse the SVG to find the rectangles
    let svgDoc = XDocument.Parse(svgString)
    let ns = svgDoc.Root.Name.Namespace
    
    // Extract rectangles from the SVG (path and rect elements)
    let rectangles = 
        svgDoc.Descendants(ns + "rect")
        |> Seq.map (fun rect -> 
            let x = float (rect.Attribute(XName.Get("x")).Value)
            let y = float (rect.Attribute(XName.Get("y")).Value)
            let width = float (rect.Attribute(XName.Get("width")).Value)
            let height = float (rect.Attribute(XName.Get("height")).Value)
            let fill = rect.Attribute(XName.Get("fill")).Value = "#000000"
            (x, y, width, height, fill))
        |> Seq.toArray
    
    // Find the boundaries of the QR code
    let minX = rectangles |> Array.map (fun (x, _, _, _, _) -> x) |> Array.min
    let minY = rectangles |> Array.map (fun (_, y, _, _, _) -> y) |> Array.min
    let maxX = rectangles |> Array.map (fun (x, _, w, _, _) -> x + w) |> Array.max
    let maxY = rectangles |> Array.map (fun (_, y, _, h, _) -> y + h) |> Array.max
    
    // Calculate cell size based on rectangle dimensions
    let cellWidth = rectangles |> Array.map (fun (_, _, w, _, _) -> w) |> Array.min
    let cellHeight = rectangles |> Array.map (fun (_, _, _, h, _) -> h) |> Array.min
    
    // Create a grid to represent the QR code
    let gridWidth = int (ceil ((maxX - minX) / cellWidth))
    let gridHeight = int (ceil ((maxY - minY) / cellHeight))
    let grid = Array2D.create gridHeight gridWidth false
    
    // Fill in the grid based on the rectangles
    for (x, y, width, height, fill) in rectangles do
        if fill then
            let gridX = int (floor ((x - minX) / cellWidth))
            let gridY = int (floor ((y - minY) / cellHeight))
            let gridW = max 1 (int (ceil (width / cellWidth)))
            let gridH = max 1 (int (ceil (height / cellHeight)))
            
            for dy in 0..(gridH-1) do
                for dx in 0..(gridW-1) do
                    if gridX + dx < gridWidth && gridY + dy < gridHeight then
                        grid.[gridY + dy, gridX + dx] <- true
    
    // Print the QR code to the console
    Console.OutputEncoding <- System.Text.Encoding.UTF8
    
    printfn "\nQR Code for TOTP Setup:\n"
    for y in 0..(gridHeight-1) do
        for x in 0..(gridWidth-1) do
            printf "%s" (if grid.[y, x] then "██" else "  ")
        printfn ""
    
    // Print instructions
    printfn "\nScan this QR code with your authenticator app (Google Authenticator, Microsoft Authenticator, Authy, etc.)"
    printfn "Secret key: %s" secretKey
    printfn "Account: %s" accountName
    printfn "Issuer: %s" issuer

[<EntryPoint>]
let main argv =
    // Check if secret key is provided as command-line argument
    let secretKey =
        if argv.Length > 0 then
            argv.[0]
        else
            // Generate a random secret key if none provided
            let random = new Random()
            let chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567" // Base32 character set
            String([|for i in 1..16 -> chars.[random.Next(chars.Length)]|])
    
    // Get account name and issuer
    let accountName = 
        if argv.Length > 1 then argv.[1] else "user@example.com"
    
    let issuer = 
        if argv.Length > 2 then argv.[2] else "MyApp"
    
    // Display the QR code
    displayOtpQrCode secretKey issuer accountName
    
    0 // Return success code
