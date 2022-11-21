<Query Kind="FSharpProgram" />

// http://zbray.com/2012/12/09/building-an-actor-in-f-with-higher-throughput-than-akka-and-erlang-actors/

open System
open System.Diagnostics
open System.Threading.Tasks
open Microsoft.FSharp.Control


let countDown = ref 0
let mre = new AutoResetEvent(false)

// Code to measure the number of messages the
// agent can process per second on a number of threads.
let test name f g =
   let procCount = System.Environment.ProcessorCount
   printfn "Processor count = %d." procCount
   let addValue = 100L

   let test2 addsPerProc : int64 =
      Parallel.For(0, procCount, fun _ ->
            for i = 1 to addsPerProc do f addValue)
            |> ignore
      
      let finalValue = g ()
      finalValue
     
   // Warm up!
   let w = test2 1000
   printfn "... warmup value: %d" w 
   //System.GC.Collect()

   // Real test
   let addsPerProc = 1000000
   let stopwatch = Stopwatch.StartNew()
   let finalValue = test2 addsPerProc
   stopwatch.Stop()
   
   printfn "... final value: %d" finalValue
   let msgCount = procCount * addsPerProc
   let expectedValue = (int64 msgCount) * addValue
   if finalValue <> expectedValue then
      failwith "Didn't work!"
   let msgsPerSecond = int (float msgCount / stopwatch.Elapsed.TotalSeconds)

   printfn "Total elapsed time = %.3f seconds." (stopwatch.Elapsed.TotalMilliseconds / 1000.0) 
   printfn "%s processed %d messages at the rate of %d msgs/sec" 
      name msgCount msgsPerSecond
   printfn "Done." 
    
type CounterMsg =
   | Add of int64
   | GetAndReset of (int64 -> unit)

let vanillaCounter =
   MailboxProcessor<CounterMsg>.Start <| fun inbox ->
      let rec loop value = async {
         let! msg = inbox.Receive()
         //printfn "... %d %A" value msg
         match msg with
         | Add v -> 
            return! loop (value + v)
         | GetAndReset reply ->
            reply value
            return! loop 0L
      }
      loop 0L

test "Vanilla Actor (MailboxProcessor)"
   (fun v -> vanillaCounter.Post (Add v))
   (fun () -> vanillaCounter.PostAndReply(fun channel -> GetAndReset channel.Reply))