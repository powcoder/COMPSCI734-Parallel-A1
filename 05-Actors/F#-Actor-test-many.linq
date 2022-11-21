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

let N = 100000
let countDown = ref N
let mre = new AutoResetEvent(false)

let agents =
    [ for i in 1 .. N ->
        MailboxProcessor<string>.Start(fun inbox -> 
            async { 
                while true do
                    let! msg = inbox.Receive()
                    if i % 10000 = 0 then
                        printfn "    [%d] agent %d got message '%s'" (tid()) i msg 
                    match msg with
                    | "ping!" -> ()
                    | "pong!" | _ -> 
                        if Interlocked.Decrement(countDown) = 0 then
                            mre.Set() |> ignore
                        return ()
    } ) ]

printfn "... [%d] massive messaging test" (tid())

let test1 () = 
    for agent in agents do
        agent.Post "ping!"
    for agent in agents do
        agent.Post "ping!"
    for agent in agents do
        agent.Post "pong!"
    mre.WaitOne() |> ignore // ensure they have all got the Pong message

let res1 = duration test1

// ===