<Query Kind="FSharpProgram" />

open System
open System.Threading

let tid () = Thread.CurrentThread.ManagedThreadId

let agent1 = MailboxProcessor<string>.Start(fun (inbox: MailboxProcessor<string>) -> 
    let count = ref 0
    async { 
        while true do
            let! msg = inbox.Receive()
            incr count
            printfn "[%d] agent1 received %A, total=%d messages" (tid()) msg !count 
            do! Async.Sleep 100
            printfn "[%d] agent1 ..." (tid())
    } )
    
let agent2 = MailboxProcessor.Start(fun inbox -> 
    let rec loop count = 
        async { 
            let! msg = inbox.Receive()
            printfn "[%d] agent2 received %A, total=%d messages" (tid()) msg (count+1)
            do! Async.Sleep 100
            printfn "[%d] agent2 ..." (tid())
            return! loop (count+1)
        }
    loop 0 
    )

printfn "[%d] main" (tid())

["the"; "quick"; "brown"; "fox";] 
|> List.map agent1.Post
|> ignore

["jumps"; "over"; "the"; "lazy"; "dog";] 
|> List.map agent2.Post
|> ignore

//Console.WriteLine ("<enter> to exit")
//Console.ReadLine () |> ignore