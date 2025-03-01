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
    let bytes = Base32Encoding.ToBytes(secretKey)

    let totp = Totp(bytes, 30, OtpHashMode.Sha1)
    let totpNumber = totp.ComputeTotp()
    AnsiConsole.MarkupLine $"TOTP: [yellow bold]{totpNumber}[/]"

    match AnsiConsole.Ask<string> "Continue (y/n)?" with
    | "y" -> GenerateCode secretKey
    | _ -> ()


let rec VerifyCode (secretKey:string) =
    let bytes = Base32Encoding.ToBytes secretKey
    let totp = Totp(bytes, 30, OtpHashMode.Sha1)

    let code = AnsiConsole.Ask "Code:"

    let mutable matchedTimeStep = 0L
    let matches = totp.VerifyTotp(code, &matchedTimeStep)

    if matches then AnsiConsole.Markup $"[green]Code matches.[/]"
    else AnsiConsole.Markup $"[red]Code NOT matches.[/]"

    Threading.Thread.Sleep 5_000


let CreateNewAuthentication () =
    AnsiConsole.MarkupLine "\n[aqua bold]# Generate TOTP #[/]\n"

    let secretKey = SecretKeyGenerator.generate ()

    AnsiConsole.MarkupLine $"Secret Key: [yellow bold]{secretKey}[/]"

    let issuer = askOrDefault "Issuer" "MyApp"
    let account = askOrDefault "Account" "user@myapp.com"

    QRCodeHelper.displayTotpQrCode secretKey issuer account

    GenerateCode secretKey
    
