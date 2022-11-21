<Query Kind="FSharpProgram" />

type Message =
     | Toggle
     | Add of int
     | Get of AsyncReplyChannel<int>

let agent = 
    MailboxProcessor<Message>.Start (fun inbox ->
        let rec active n = async { 
            printfn "active   %d" n
            let! msg = inbox.Receive()
            match msg with
                | Toggle -> return! inactive n
                | Add m -> return! active (n + m)
                | Get ch -> ch.Reply n; return! active n 
            }
                
        and inactive n = async { 
            printfn "inactive %d" n
            let! msg = inbox.Receive()
            match msg with
                | Toggle -> return! active n
                | Add _ -> return! inactive n
                | Get ch -> ch.Reply n; return! inactive n
            }
    
        active 0 )           // Agent prints "active   0"

agent.Post (Add 10)          // Agent prints "active   10"
agent.Post Toggle            // Agent prints "inactive 10"
agent.Post (Add 20)          // Agent prints "inactive 10"
agent.Post Toggle            // Agent prints "active   10"

agent.Post (Add 30)          // Agent prints "active   40"

let n = 
    agent.PostAndReply (     // Main calls & waits for the reply
        fun ch -> (Get ch)) 

let n2 = 
    agent.PostAndReply Get   // Main calls & waits for the reply 

printfn "... got %d %d" n n2 // Main prints "got 40"
//