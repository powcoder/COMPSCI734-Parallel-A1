<Query Kind="FSharpProgram" />

// ===

open System
open System.Diagnostics
open System.Threading

let tid () = Thread.CurrentThread.ManagedThreadId

let duration f = 
    let timer = new Stopwatch()
    timer.Start()
    let res = f()
    timer.Stop()
    printfn "... [%d] duration: %i ms" 
            (tid()) timer.ElapsedMilliseconds
    res

// ===

let asyncs n = // list<Async<unit>>
    [ for i in 0 .. n ->
        async { 
            do! Async.Sleep(1000)
            if i % 10000 = 0 then
                printfn "    [%d] async %d" (tid()) i 
            // return ()
        } ]

printfn "... [%d] massive sleeping test" (tid())

let test2 n = 
    Async.Parallel (asyncs n)
    |> Async.RunSynchronously

let n = 100000
let res2 = duration (fun () -> test2 n) |> Array.length
printfn "n = %d, res2 = %d, ok = %b" (n+1) res2 (res2 = n + 1)

// ===