module TOTP

open System
open Spectre.Console
open System.Text

let useDefaultIfEmpty value defaultValue =
    if String.IsNullOrWhiteSpace value then defaultValue else value


let GenerateCode (secretKey:string) =
    let bytes = ASCIIEncoding.ASCII.GetBytes(secretKey)
    let totp = OtpNet.Totp(bytes)
    let totpNumber = totp.ComputeTotp()
    AnsiConsole.MarkupLine $"TOTP: [yellow bold]{totpNumber}[/]"

let CreateNewAuthentication () =
    AnsiConsole.MarkupLine "\n[aqua bold]# Generate TOTP #[/]\n"

    let secretKey = SecretKeyGenerator.generate ()

    AnsiConsole.MarkupLine $"Secret Key: [yellow bold]{secretKey}[/]"

    let defaultIssuer = "MyApp"
    let defaultAccount = "user@myapp.com"
    let issuer = useDefaultIfEmpty (AnsiConsole.Ask<string> $"Issuer (left blank for {defaultIssuer})") defaultIssuer
    let account = useDefaultIfEmpty (AnsiConsole.Ask<string> $"Account (left blank for {defaultAccount})") defaultAccount

    QRCodeHelper.displayTotpQrCode (secretKey) issuer account

    GenerateCode secretKey
    
