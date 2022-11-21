<Query Kind="FSharpProgram" />

// -------------------------

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open System.Diagnostics

// -------------------------

let tid () = Thread.CurrentThread.ManagedThreadId

let duration f = 
    let timer = new Stopwatch()
    timer.Start()
    let res = f()
    timer.Stop()
    printfn "... [%d] duration: %i ms" (tid()) timer.ElapsedMilliseconds
    res

// -------------------------

type Message = int // FromNorth of int

let n1, n2 = 30, 40 // 300, 400
let agents = Array.zeroCreate<MailboxProcessor<Message>> n1 // typed nulls
let result = new TaskCompletionSource<int*int> ()

// not really needed, just to collect some threading stats
let threads = new ConcurrentDictionary<int, int> ()

// -------------------------

let agent i1 = 
    MailboxProcessor<Message>.Start <| fun inbox ->
        let rec westeastloop i1 i2 w = 
            async {
                let! n = inbox.Receive () 
                
                threads.AddOrUpdate (tid (), 1, fun k v -> v + 1) |> ignore // for threading stats only
                Thread.SpinWait 100000 // simulate intensive computation
                
                if i1 = n1-1 && i2 = n2-1 then
                    result.SetResult (n+1, w+1)
                else
                    if i1 < n1-1 then agents.[i1+1].Post (n+1) // Post to South
                    if i2 < n2-1 then return! westeastloop i1 (i2+1) (w+1) // Impersonate East
            }
        westeastloop i1 0 0

// -------------------------

for i1 = 0 to n1-1 do
    agents.[i1] <- agent i1

for i2 = 1 to n2-1 do 
    agents.[0].Post (0)
    
// -------------------------

let n, w = 
    duration <| fun () -> 
        // actual start from the North-West corner
        agents.[0].Post (0)
        result.Task.Result
        
printfn "... [%d] n:%d, w:%d" (tid()) n w

// optional stats
threads |> Seq.iter (fun kv -> printfn "... [%d] %d times" kv.Key kv.Value)
let tcount = threads |> Seq.length
let acount = threads |> Seq.sumBy (fun kv -> kv.Value)
printfn "... [%d] total threads:%d, agents:%d" (tid()) tcount acount

// -------------------------