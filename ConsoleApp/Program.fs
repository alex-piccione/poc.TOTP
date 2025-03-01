// For more information see https://aka.ms/fsharp-console-apps
open Spectre.Console

AnsiConsole.Write(FigletText("TOTP POC", Color=Color.CadetBlue))

let mutable secretKey:string option = None

let rec execute () =
    AnsiConsole.Clear()
    let action = 
        AnsiConsole.Prompt(SelectionPrompt<string>(Title="What do you want to do?")
            .AddChoices("Create new TOTP", "Generate TOTP code", "Verify TOTP Code", "Exit"))
    
    match action with
    | "Exit" -> AnsiConsole.WriteLine "Bye bye"
    | "Create new TOTP" -> TOTP.CreateNewAuthentication ()
    | "Generate TOTP code" ->
        if secretKey.IsNone
        then secretKey <- Some(AnsiConsole.Ask<string>("Secret Key:"))
        TOTP.GenerateCode (secretKey.Value)
    | "Verify TOTP Code"->
        if secretKey.IsNone
        then secretKey <- Some(AnsiConsole.Ask<string>("Secret Key:"))
        TOTP.VerifyCode (secretKey.Value)
    | _ -> AnsiConsole.WriteLine $"Unknown action: {action}"

    if action <> "Exit" then execute()

execute()