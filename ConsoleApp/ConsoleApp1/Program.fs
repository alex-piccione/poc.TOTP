// For more information see https://aka.ms/fsharp-console-apps
open Spectre.Console

AnsiConsole.Write(FigletText("TOTP POC", Color=Color.CadetBlue))


let rec execute () =
    let action = 
        AnsiConsole.Prompt(SelectionPrompt<string>(Title="What do you want to do?")
            .AddChoices("TOTP", "Exit"))            

    
    match action with    
    | "Exit" -> AnsiConsole.WriteLine "Bye bye"
    | "TOTP" -> TOTP.Run()
    | _ -> AnsiConsole.WriteLine $"Unknown action: {action}"

    if action <> "Exit" then execute()
     

//execute()

for i in 1..10 do 
    AnsiConsole.MarkupLine $"Key: [yellow]{SecretKeyGenerator.generate_2 (25+i)}[/]"