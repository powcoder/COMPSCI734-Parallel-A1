<Query Kind="FSharpProgram" />

let rec und = MailboxProcessor.Start (fun inbox ->
    let rec loop count =
        async {
            und.Post 1
            let! msg = inbox.Receive ()
            match msg with
            | -1 -> 
                printfn "%d" count
                return ()
            | _ -> 
                return! loop (count+1)
        }
    loop 0)

printfn "und started" 
Thread.Sleep 0
und.Post -1
Thread.Sleep 1000