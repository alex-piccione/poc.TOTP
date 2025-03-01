# Time-based One Time Password P.O.C.

POC about Time-based One-Time Password.

I choose Otp.net for TOTP generation.  
I choose QRCoder for generate the SQR code.  


## Otp.net

TOTP geneation requires a time window.  
Google authentivcator uses Sha1.  


## QRCoder

It allows to create the SVG easily.  
The SVG can be sent as plain text, along with Secret Key, to the UI to render the image.  
  
In this Console application the SVG is rendered only to facilitate the creation of the TOTP record on the Authenticator applicartion easily.  

TOTP geneation requires a time window.  

