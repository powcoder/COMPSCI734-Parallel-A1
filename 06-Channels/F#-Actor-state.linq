<Query Kind="FSharpProgram" />

open System
open System.Threading

let tid () = Thread.CurrentThread.ManagedThreadId

[<NoComparison>]
type Message =
     | Toggle
     | Add of int
     | Get of AsyncReplyChannel<int>

let agent = 
    MailboxProcessor<Message>.Start (fun inbox ->
        let rec active n = async { 
            let! msg = inbox.Receive()
            printfn "active   %d (%A)" n msg
            match msg with
                | Toggle -> return! inactive n
                | Add m -> return! active (n + m)
                | Get ch -> ch.Reply n; return! active n 
            }
                
        and inactive n = async { 
            let! msg = inbox.Receive()
            printfn "inactive %d (%A)" n msg
            match msg with
                | Toggle -> return! active n
                | Add _ -> return! inactive n
                | Get ch -> ch.Reply n; return! inactive n
            }
    
        active 0 )           

agent.Post (Add 10)          
agent.Post Toggle            
agent.Post (Add 20)          
agent.Post Toggle            

agent.Post (Add 30)          

Thread.Sleep 10

printfn ""
let r = 
    agent.PostAndAsyncReply Get    // async posty
    // Get = fun ch -> (Get ch) 
let n = Async.RunSynchronously r
printfn "... got %d" n

printfn ""
let n2 = 
    agent.PostAndReply Get         // sync post 
    // Get = fun ch -> (Get ch) 
printfn "... got %d" n2

printfn "\r\n... done"
//