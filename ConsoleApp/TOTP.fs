module TOTP

open System
open Spectre.Console
open System.Text

let useDefaultIfEmpty value defaultValue =
    if String.IsNullOrWhiteSpace value then defaultValue else value

let askOrDefault (prompt) (defaultValue:string) =
    match AnsiConsole.Ask<string>($"{prompt} (use \"y\" to use deafault: \"{defaultValue}\")") with
    | "y" -> defaultValue
    | input -> input

let GenerateCode (secretKey:string) =
    let bytes = ASCIIEncoding.ASCII.GetBytes(secretKey)
    let totp = OtpNet.Totp(bytes)
    let totpNumber = totp.ComputeTotp()
    AnsiConsole.MarkupLine $"TOTP: [yellow bold]{totpNumber}[/]"

let CreateNewAuthentication () =
    AnsiConsole.MarkupLine "\n[aqua bold]# Generate TOTP #[/]\n"

    let secretKey = SecretKeyGenerator.generate ()

    AnsiConsole.MarkupLine $"Secret Key: [yellow bold]{secretKey}[/]"

    let issuer = askOrDefault "Issuer" "MyApp"
    let account = askOrDefault "Account" "user@myapp.com"

    QRCodeHelper.displayTotpQrCode secretKey issuer account

    GenerateCode secretKey
    
