module TOTP

open System
open Spectre.Console
open System.Text
open OtpNet

let useDefaultIfEmpty value defaultValue =
    if String.IsNullOrWhiteSpace value then defaultValue else value

let askOrDefault (prompt) (defaultValue:string) =
    match AnsiConsole.Ask<string>($"{prompt} (use \"y\" to use deafault: \"{defaultValue}\")") with
    | "y" -> defaultValue
    | input -> input

let rec GenerateCode (secretKey:string) =
    let bytes = ASCIIEncoding.ASCII.GetBytes(secretKey)
    let totp = Totp(bytes, 30, OtpHashMode.Sha256)

    let totpNumber = totp.ComputeTotp()
    AnsiConsole.MarkupLine $"TOTP: [yellow bold]{totpNumber}[/]"

    System.Threading.Thread.Sleep(2_000)

    // Is culture influencing hte TOTP ?
    System.Threading.Thread.CurrentThread.CurrentCulture <- System.Globalization.CultureInfo("en-US")
    System.Threading.Thread.CurrentThread.CurrentUICulture <- System.Globalization.CultureInfo("en-US")
    let totpNumber = totp.ComputeTotp()
    AnsiConsole.MarkupLine $"TOTP: [yellow bold]{totpNumber}[/]"


    System.Threading.Thread.Sleep(10_000)

    match AnsiConsole.Ask<string> "Continue (y/n)?" with
    | "y" -> GenerateCode secretKey
    | _ -> ()


let rec VrifyCode (secretKey:string) =
    let bytes = ASCIIEncoding.ASCII.GetBytes(secretKey)
    let totp = Totp(bytes, 30, OtpHashMode.Sha256)


    let code = AnsiConsole.Ask "Code:"
    let matches = totp.VerifyTotp(code, [<Out>] timeStepMatched)

    if matches then AnsiConsole.Markup $"Code matches"
    else AnsiConsole.Markup $"[red]Code NOT matches[/]"

    ()
    //let totpNumber = totp.VerifyTotp DateTime.UtcNow 
    //AnsiConsole.MarkupLine $"TOTP: [yellow bold]{totpNumber}[/]"
    //System.Threading.Thread.Sleep(30_000)

    //match AnsiConsole.Ask<string> "Continue (y/n)?" with
    //| "y" -> GenerateCode secretKey
    //| _ -> ()


let CreateNewAuthentication () =
    AnsiConsole.MarkupLine "\n[aqua bold]# Generate TOTP #[/]\n"

    let secretKey = SecretKeyGenerator.generate ()

    AnsiConsole.MarkupLine $"Secret Key: [yellow bold]{secretKey}[/]"

    let issuer = askOrDefault "Issuer" "MyApp"
    let account = askOrDefault "Account" "user@myapp.com"

    QRCodeHelper.displayTotpQrCode secretKey issuer account

    GenerateCode secretKey
    
