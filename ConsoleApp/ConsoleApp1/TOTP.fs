module TOTP

open System
open Spectre.Console


let generateQrCodeSvg (secretKey:string) =

    use generator = new QRCoder.QRCodeGenerator()

    use data = generator.CreateQrCode(secretKey, QRCoder.QRCodeGenerator.ECCLevel.Default)
    use coder = new QRCoder.SvgQRCode(data)

    coder.GetGraphic(20)

let test () =
    
    let grid = Grid()

    grid.AddColumns(3) |> ignore

    grid.AddRow(Markup("a"), Markup("b"), Markup("c")) |> ignore

    let row = [|Markup("a"); Markup("b"); Markup("c")|] 

    AnsiConsole.Write(grid);

    ()

let QRFill = "#000000" // QRCodes default "black" of QR code



let printQRCode_1 svg =
    let size, bits = SvgHelper.parseSvgQrSquares svg QRFill

    let mutable row = 0
    bits 
    |> Seq.iteri (fun i bit ->
        let newRow = i / size
        
        if newRow > row then 
            printfn "" // new line 
            row <- newRow
        printf "%s" (if bit then "1" else "0")
        )
    //    if bit.Y > row then printfn "" // new line 
    //    printf "%s" (if bit then "1" else "0")
    //    row <- bit.Y

    //for bit in bits do
    //    if bit.Y > row then printfn "" // new line 
    //    printf "%s" (if bit then "1" else "0")
    //    row <- bit.Y

    //for x in 0..size-1 do
    //    let bit 
    //    let mutable y = bits[x]
    //    printfn "%s" (bits[x].ToString())
    //    //printf "%s" (if rectangles[x].Fill then "██" else "--")


    printfn ""


let printQRCode_2 svg = 
    // 1. Parse the SVG
    let rectangles = SvgHelper.parseSvgRectangles svg

    // 2. Determine QR Code Dimensions
    let qrCodeSize = Math.Sqrt(rectangles.Length |> float) |> int // Assuming a square QR code

    // 3. Create a Spectre.Console Grid

    let grid = Grid()

    // Add all the columns
    [0..qrCodeSize]
    |> Seq.iter(fun _ -> 
        let column = GridColumn()
        column.NoWrap <- true // Prevent wrapping
        grid.AddColumn(column) |> ignore
        )

    // 4. Add Rows and Cells to the Grid
    for i in 0..qrCodeSize do    

        let squares = 
            [0..qrCodeSize]
            |> Seq.map(fun j ->
                let rectIndex = i * qrCodeSize + j
                let square = if rectangles[rectIndex].Fill = Some("#000000") then "[red]██[/]" else "[white]██[/]" // Use ██ for better visual representation    
                Markup(square) :> Rendering.IRenderable)
                |> Seq.toArray

        //let row = GridRow(squares)
        grid.AddRow(squares) |> ignore
    

    // 5. Render the Grid
    AnsiConsole.Write(grid);




let Run () =
    AnsiConsole.MarkupLine "\n[aqua bold]# Generate TOTP #[/]\n"

    // bet 
    let secretKey = UUID().ToString()

    AnsiConsole.MarkupLine $"Secret Key: [yellow bold]{secretKey}[/]"


    let svg = generateQrCodeSvg secretKey

    printQRCode_1 svg

    //PrintQRCode svg

    //AnsiConsole.MarkupLine $"QR Code: [yellow bold]{svg}[/]"


    let bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(secretKey)

    let totp = OtpNet.Totp(bytes)

    let totpNumber = totp.ComputeTotp()

    AnsiConsole.MarkupLine $"TOTP: [yellow bold]{totpNumber}[/]"