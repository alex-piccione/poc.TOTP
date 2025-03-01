module private SecretKeyGenerator

// refs: RFC 4648, RFC 6238, RFC 4226

open System.Security.Cryptography

// Base32 character set (A-Z, 2-7) as per RFC ...
let private alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567" // A-Z + 2-7 
let private length = 20 // TOTP standard requires at least 128 bits, 160 or more are suggested. 32 bytes are 256 bits


// Each character in Base32 can represent a value from 0 to 31, that are 5 bits of data (2^5 = 32).

let generate () =
    let randomBytes = Array.zeroCreate<byte> length
    use rng = RandomNumberGenerator.Create()
    rng.GetBytes(randomBytes)

    // this is enough but as exercise/study I leave the custom implementation here
    //let secretKey = OtpNet.Base32Encoding.ToString randomBytes

    let base32Encode (bytes: byte[]) : string =
        let mutable bitsBuffer = 0 // use a integer as buffer for 32 bits
        let mutable bitIndex = 0 // to keep track of the position
        let mutable result = ""
        
        for i = 0 to bytes.Length - 1 do
            // "clean" the 8 rightmost bits and "add" the incoming value
            bitsBuffer <- (bitsBuffer <<< 8) ||| int bytes[i] 
            bitIndex <- bitIndex + 8
            
            // Extract 5-bit chunks
            while bitIndex >= 5 do
                // shift to right to position the first 5 bits on the rightmost aprt of the buffer                
                bitIndex <- bitIndex - 5
                // bitwise OR with 11111 to get only the 5 bits we want
                let index = (bitsBuffer >>> bitIndex) &&& 0x1F 
                // use the value of the 5 bits tpo get the alphabet character
                result <- result + string alphabet[index]               
                
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
