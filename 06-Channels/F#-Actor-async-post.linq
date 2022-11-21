<Query Kind="FSharpProgram" />

open System
open System.Threading

let tid () = Thread.CurrentThread.ManagedThreadId

let agent = MailboxProcessor.Start(fun inbox -> 
    let rec loop count = 
        async { 
            let! m = inbox.Receive()
            Console.Error.WriteLine ("[{0}] take {1}", tid(), m)
            do! Async.Sleep 100
            return! loop (count+1)
        }
    loop 0 
    )

let s = ["the"; "quick"; "brown"; "fox"; "jumps"; "over"; "the"; "lazy"; "dog";]

for m in s do 
    Console.Error.WriteLine ("[{0}] post {1}", tid(), m)
    agent.Post m

Console.Error.WriteLine ()
//

