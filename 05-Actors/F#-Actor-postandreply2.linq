<Query Kind="FSharpProgram" />

open System
open System.Threading

let tid () = Thread.CurrentThread.ManagedThreadId

type Message = string * AsyncReplyChannel<string>

let agent = MailboxProcessor<Message>.Start(fun inbox ->
    let rec loop n =
        async {            
            try
                let! (message, replyChannel) = inbox.Receive () // (1000);
                printfn "    [%d] n=%d: %s" (tid())  n message
                replyChannel.Reply(sprintf "n=%d: %s" n message)
                do! loop (n + 1)

            with
            | :? TimeoutException -> 
                printfn "*** [%d] n=%d: timeout" (tid()) n
        }
        
    loop 0
    )

let ask question = MailboxProcessor<unit>.Start(fun inbox ->
    async {
        let asyncreply = 
            agent.PostAndAsyncReply (
                (fun replyChannel -> (question, replyChannel))) // , 1000)
        printfn "... [%d] " (tid()) 
        let! reply = asyncreply
        printfn "... [%d] %s" (tid()) reply
    })

["the"; "quick"; "brown"; "fox"] 
|> List.map ask
|> ignore

//Thread.Sleep (3000)
//ask "ayt?"