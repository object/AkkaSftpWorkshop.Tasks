[<AutoOpen>]
module CommonTypes

    type Alphanumeric = 
        | Alphanumeric of string
        member this.Value = this |> function | Alphanumeric s -> s

    type UncPath = 
        | UncPath of string
        member this.Value = this |> function | UncPath s -> s

    type Url = 
        | Url of string
        member this.Value = this |> function | Url s -> s

    type AbsoluteUrl = 
        | AbsoluteUrl of string
        member this.Value = this |> function | AbsoluteUrl s -> s

    type RelativeUrl = 
        | RelativeUrl of string
        member this.Value = this |> function | RelativeUrl s -> s

    type FileName = 
        | FileName of string
        member this.Value = this |> function | FileName s -> s

    [<Measure>] type s
    [<Measure>] type MB
