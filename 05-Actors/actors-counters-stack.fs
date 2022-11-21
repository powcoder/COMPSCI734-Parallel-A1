https://powcoder.com
代写代考加微信 powcoder
Assignment Project Exam Help
Add WeChat powcoder
module actors_counters_stack

open System
open System.Threading
let tid () = Thread.CurrentThread.ManagedThreadId

let adone = new AutoResetEvent (false)

let agent1 limit1 trace1 = MailboxProcessor<int>.Start(fun inbox -> 
    Console.Error.WriteLine (sprintf "agent1 limit1:%d trace1:%d" limit1 trace1)
    let count = ref 0

    async { 
        let mutable cont = true
        
        while cont do
            if !count % trace1 = 0 then    
                Console.Error.WriteLine (sprintf "[%d] agent1 received total=%d messages" (tid()) !count)
                Console.Error.Flush ()

            if !count >= limit1 then
                cont <- false
                adone.Set () |> ignore
                
            else 
                let! msg = inbox.Receive()
                incr count
    } )
    
let agent2 limit2 trace2 = MailboxProcessor<int>.Start(fun inbox -> 
    Console.Error.WriteLine (sprintf "agent2 limit2:%d trace2:%d" limit2 trace2)

    let rec loop count = 
        async { 
            if count % trace2 = 0 then
                Console.Error.WriteLine (sprintf "[%d] agent2 received total=%d messages" (tid()) count)
                Console.Error.Flush ()
                
            if count >= limit2 then
                adone.Set () |> ignore
                return ()
                
            else 
                let! msg = inbox.Receive()
                return! loop (count+1)
        }
    loop 0 
    )


let main2 () =
    let limit2 = 100000000
    let trace2 = limit2 / 10 
    let agent2 = agent2 limit2 trace2
    for i = 1 to limit2 do
        agent2.Post i

    adone.WaitOne () |> ignore
    Console.Error.WriteLine ()
    Console.Error.Flush ()
    
    let limit1 = 100000000
    let trace1 = limit1 / 10 
    let agent1 = agent1 limit1 trace1
    for i = 1 to limit1 do
        agent1.Post i

    adone.WaitOne () |> ignore
    Console.Error.WriteLine ()
    Console.Error.Flush ()
    
[<EntryPoint()>]
let main (args: string[]) =
    try
        main2 ()
        0
        
    with ex ->
        Console.Error.WriteLine (sprintf "*** %s" ex.Message)
        1