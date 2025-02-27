module SvgHelper

open System.Xml.Linq
open System

type QRSquare = {
    X: float
    Y: float
    Fill: bool
}

type QRBit = {
    X: int
    Y: int
    Fill: bool
}


let parseSvgQrSquares svg blackFill =

    let xdoc = XDocument.Parse svg

    let ns = XNamespace.Get("http://www.w3.org/2000/svg")

    let rectElements = xdoc.Descendants(ns + "rect")

    let rectangles:QRSquare list = 
        rectElements |> Seq.map (fun rect ->
            let getAttribute name defaultValue =
                let attr = rect.Attribute(XName.Get(name))
                if attr <> null then attr.Value else defaultValue

            let getFloatAttribute name =
                getAttribute name "0" |> float

            let getFillAttribute () =
                match getAttribute "fill" null with
                | null -> false
                | fill when fill = blackFill -> true
                | _ -> false

            {
                QRSquare.X = getFloatAttribute "x"
                QRSquare.Y = getFloatAttribute "y"
                Fill = getFillAttribute ()
            }
            )
            |> Seq.toList

    // debug
    //for x in 0..10 do
    //    printfn "%s" (rectangles[x].ToString())


    let size = Math.Sqrt rectangles.Length // Assume QR Code is squared
    //let ordered = rectangles |> List.sortBy(fun r -> r.X + r.Y*size)

    let bits = 
        rectangles 
        |> Seq.sortBy (fun r -> r.X + r.Y*size) // use the size to make Y value a order of difference from the max X
        |> Seq.map (fun r -> r.Fill)

    //let sizeInt = size |> int
    //let bits = 
    //    ordered 
    //    |> Seq.mapi (
    //    fun i rect -> 
    //        let row = i % sizeInt
    //        {
    //            QRBit.X = i % sizeInt
    //            QRBit.Y = i / sizeInt
    //            Fill = rect.Fill
    //        }
    //    ) 
    //    |> Seq.toList

    //for x in 0..10 do
    //    printfn "%s" (bits[x].ToString())

    size |> int, bits 

type SvgRectangle = {
    X: float
    Y: float
    Width: float
    Height: float
    Fill: string option
}

let parseSvgRectangles svg =

    let xdoc = XDocument.Parse svg

    let ns = XNamespace.Get("http://www.w3.org/2000/svg")

    let rectElements = xdoc.Descendants(ns + "rect")

    rectElements 
    |> Seq.map (fun rect ->
        let getAttribute name defaultValue =
            let attr = rect.Attribute(XName.Get(name))
            if attr <> null then attr.Value else defaultValue

        let getFloatAttribute name =
            getAttribute name "0" |> float

        let getFillAttribute () =
            let fill = getAttribute "fill" null
            if fill <> null then Some fill else None

        {
            X = getFloatAttribute "x"
            Y = getFloatAttribute "y"
            Width = getFloatAttribute "width"
            Height = getFloatAttribute "height"
            Fill = getFillAttribute ()
        }
        )
        |> Seq.toList
