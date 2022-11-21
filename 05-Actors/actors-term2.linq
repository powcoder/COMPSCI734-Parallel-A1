<Query Kind="FSharpProgram" />

open System
open System.Threading
open System.Threading.Tasks
let tid () = Thread.CurrentThread.ManagedThreadId

let acount = ref 7
let adone = TaskCompletionSource<bool> ()

let act i = MailboxProcessor<int>.Start(fun inbox ->
    async {
        let! m = inbox.Receive ()
        Console.WriteLine (sprintf "... [%d] %d" (tid()) i)
        
        if Interlocked.Decrement acount = 0 then 
            Console.WriteLine (sprintf "... [%d] done all" (tid()))
            adone.SetResult true
    })

let acts = [for i = 1 to !acount do yield i]

Console.WriteLine (sprintf "    [%d] start" (tid()))

acts |> List.iter (fun i -> (act i).Post i)

Console.WriteLine (sprintf "    [%d] ..." (tid()))

adone.Task.Wait ()

Console.WriteLine (sprintf "    [%d] stop" (tid()))

//Console.ReadLine () |> ignore