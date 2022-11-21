<Query Kind="FSharpProgram" />

// http://stackoverflow.com/questions/2214954/is-f-really-faster-than-erlang-at-spawning-and-killing-processes

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

type WaitMsg = 
    | Die 

let countDown = ref 0
let mre = new AutoResetEvent(false)

let wait(i) = 
    MailboxProcessor<WaitMsg>.Start(fun inbox -> 
        async { 
            if Interlocked.Decrement(countDown) = 0 then
                mre.Set() |> ignore
                
            let! msg = inbox.Receive() 
            
            match msg with  
            | Die -> 
                if Interlocked.Decrement(countDown) = 0 then
                    mre.Set() |> ignore
                    
            //return () 
        })

let test N = 
    printfn "Processor count = %d." System.Environment.ProcessorCount
    printfn "Starting and killing %d actors" N 
    let stopwatch = Stopwatch.StartNew() 
    
    countDown := N
    let actors = [for i = 1 to N do yield wait(i)] 
    
    mre.WaitOne() |> ignore // ensure they have all spun up
    //mre.Reset() |> ignore

    countDown := N
    for actor in actors do 
        actor.Post(Die) 
    
    mre.WaitOne() |> ignore // ensure they have all got the kill message
    
    stopwatch.Stop() 
    
    printfn "Total elapsed time = %.3f seconds." (stopwatch.Elapsed.TotalMilliseconds / 1000.0) 
    printfn "Average actor start/kill time = %.3f microseconds." (stopwatch.Elapsed.TotalMilliseconds * 1000.0 / float(N)) 
    printfn "Done." 

test 100000