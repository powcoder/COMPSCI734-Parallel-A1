<Query Kind="FSharpProgram" />

open System
open System.Threading
let tid () = Thread.CurrentThread.ManagedThreadId

type AdoneMSg =
    | Done of int
    | Subscribe of AsyncReplyChannel<bool>

let acount = 7

let adone = 
    MailboxProcessor<AdoneMSg>.Start(fun inbox ->
    let rec loop count 
            (chs: list<AsyncReplyChannel<bool>>) =
        async {
            let! m = inbox.Receive ()
            Console.WriteLine (sprintf "... [%d] m: (%A)" (tid()) m)
             
            match m with 
            | Done i -> 
                if count > 1 then return! loop (count-1) chs
                else 
                    Console.WriteLine (sprintf "... [%d] done chs: (%d)" (tid()) (List.length chs))
                    chs |> List.iter (fun ch -> ch.Reply true)
                    return! loop 0 []
            
            | Subscribe ch -> 
                if count > 0 then return! loop count (ch::chs)
                else 
                    Console.WriteLine (sprintf "... [%d] done chs: (%d)" (tid()) (List.length chs))
                    ch.Reply true
                    return! loop 0 []
                    
        }
        
    loop acount [])
    
let act i = MailboxProcessor<int>.Start(fun inbox ->
    async {
        let! m = inbox.Receive ()
        Console.WriteLine (sprintf "... [%d] %d" (tid()) i)
        
        adone.Post (Done i)
    })

Console.WriteLine (sprintf "    [%d] start" (tid()))

let isdone = adone.PostAndAsyncReply Subscribe

[1..acount] |> List.iter (fun i -> (act i).Post i)

Console.WriteLine (sprintf "    [%d] ..." (tid()))

Async.RunSynchronously isdone |> ignore

Console.WriteLine (sprintf "    [%d] stop" (tid()))

// ---

Thread.Sleep 2000

let isdone' = adone.PostAndAsyncReply Subscribe

Async.RunSynchronously isdone' |> ignore

Console.WriteLine (sprintf "    [%d] stop'" (tid()))

//Console.ReadLine () |> ignore