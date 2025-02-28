module SecretKeyGenerator

// refs: RFC 4648, RFC 6238, RFC 4226

open System
open System.Security.Cryptography

// Base32 character set (A-Z, 2-7) as per RFC ...
let alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567" // 32 
let lenght = 20 // TOTP standard requires at least 128 bits, 160 or more are suggested. 32 bytes are 256 bits



// Each character in Base32 can represent a a value from 0 to 31, that are 5 bits of data (2^5 = 32).



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




let generate_2 lenght =
    let randomBytes = Array.zeroCreate<byte> lenght
    use rng = RandomNumberGenerator.Create()
    rng.GetBytes(randomBytes)

    let base32Encode (bytes: byte[]) : string =
        let mutable bitIndex = 0
        let mutable byteValue = 0
        let mutable result = ""
        for i = 0 to bytes.Length - 1 do
            byteValue <- (byteValue <<< 8) ||| int bytes[i]
            bitIndex <- bitIndex + 8
            while bitIndex >= 5 do
                result <- result + string alphabet[(byteValue >>> (bitIndex - 5)) &&& 0x1F]
                bitIndex <- bitIndex - 5
        if bitIndex > 0 then
            result <- result + string alphabet[(byteValue <<< (5 - bitIndex)) &&& 0x1F]
        result

    base32Encode randomBytes

let generate_3 length =
    let randomBytes = Array.zeroCreate<byte> length
    use rng = RandomNumberGenerator.Create()
    rng.GetBytes(randomBytes)
       
    let base32Encode (bytes: byte[]) : string =        
        let mutable bitsBuffer = 0 // use a integer as buffer for 32 bits
        let mutable bitIndex = 0 // to keep track of the position
        let mutable result = ""
        
        for i = 0 to bytes.Length - 1 do
            bitsBuffer <- (bitsBuffer <<< 8) ||| int bytes[i] // "clean" the 8 rightmost bits and "add" the incoming value
            bitIndex <- bitIndex + 8
            
            // Extract 5-bit chunks
            while bitIndex >= 5 do
                // shift to right to position the first 5 bytes on the rightmost bits of th ebuffer
                // and cleanup the ("0") the bits on the left of the 5 interesting bits
                let index = (bitsBuffer >>> (bitIndex - 5)) &&& 0x1F 
                // use the value of the buffer (that is defined by teh 5 bits we are looking at) to get the character
                result <- result + string alphabet[index]
                bitIndex <- bitIndex - 5
        
        // Handle remaining bits
        if bitIndex > 0 then
            let index = (bitsBuffer <<< (5 - bitIndex)) &&& 0x1F
            result <- result + string alphabet[index]
        
        // Add padding
        let paddingCount = 
            match bytes.Length % 5 with
            | 0 -> 0            // No padding needed
            | 1 -> 6            // 1 byte → 2 chars + 6 padding
            | 2 -> 4            // 2 bytes → 4 chars + 4 padding
            | 3 -> 3            // 3 bytes → 5 chars + 3 padding
            | 4 -> 1            // 4 bytes → 7 chars + 1 padding
            | _ -> 0            // Should never happen
            
        result + String.replicate paddingCount "="
    
    base32Encode randomBytes