<Query Kind="FSharpProgram" />

open System
open System.Threading
let tid () = Thread.CurrentThread.ManagedThreadId

let acount = ref 7
let adone = new AutoResetEvent (false)

let act i = MailboxProcessor<int>.Start(fun inbox ->
    async {
        let! m = inbox.Receive ()
        Console.WriteLine (sprintf "... [%d] %d" (tid()) i)
        
        if Interlocked.Decrement acount = 0 then 
            Console.WriteLine (sprintf "... [%d] done all" (tid()))
            adone.Set () |> ignore
    })

let acts = [for i = 1 to !acount do yield i]

Console.WriteLine (sprintf "    [%d] start" (tid()))

acts |> List.iter (fun i -> (act i).Post i)

Console.WriteLine (sprintf "    [%d] ..." (tid()))

adone.WaitOne () |> ignore

Console.WriteLine (sprintf "    [%d] stop" (tid()))

//Console.ReadLine () |> ignore