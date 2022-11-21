<Query Kind="FSharpProgram" />

let rec und = MailboxProcessor.Start (fun inbox ->
    let count = ref 0
    async {
        let mutable cont = true
        while cont do
            und.Post 1
            let! msg = inbox.Receive ()
            match msg with
            | -1 -> 
                printfn "%d" !count
                cont <- true
            | _ -> 
                incr count
    })
    
printfn "und started" 
Thread.Sleep 0
und.Post -1
Thread.Sleep 1000