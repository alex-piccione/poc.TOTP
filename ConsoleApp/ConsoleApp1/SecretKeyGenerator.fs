module SecretKeyGenerator

open System
open System.Security.Cryptography

// Base32 character set (A-Z, 2-7)
let alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"
let lenght = 32 // TOTP standard requires a Base32 

let generate () =    

    let randomBytes = Array.zeroCreate<byte> lenght
    use rng = RandomNumberGenerator.Create()
    rng.GetBytes(randomBytes)

    let getChar byte =
        let x = 0
        alphabet[x]

    randomBytes |> Seq.map (fun b -> getChar b) |> Seq.toArray |> String

    //String([|for b in randomBytes -> getChar b|])

let generate_withRandom () =
    let random = new Random(DateTime.UtcNow.Millisecond)
    String([|for i in 1..lenght -> alphabet[random.Next(alphabet.Length)]|])