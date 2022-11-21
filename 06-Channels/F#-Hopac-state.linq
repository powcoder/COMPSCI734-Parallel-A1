<Query Kind="FSharpProgram">
  <Reference>FSharp.Core.dll</Reference>
  <Reference>Hopac.dll</Reference>
  <Reference>Hopac.Core.dll</Reference>
</Query>

open System
open System.Threading

let tid () = Thread.CurrentThread.ManagedThreadId

open Hopac
open Hopac.Core
open Hopac.Infixes

[<NoComparison>]
type Message =
     | Toggle
     | Add of int
     | Get of Ch<int>

let achan = Ch<Message> ()

let agent = 
    let rec active n : Job<unit> = job { 
        let! msg = achan
        printfn "active   %d (%A)" n msg
        match msg with
            | Toggle -> return! inactive n
            | Add m -> return! active (n + m)
            | Get ch -> 
                do! ch *<- n
                return! active n 
        }
            
    and inactive n : Job<unit> = job { 
        let! msg = achan
        printfn "inactive %d (%A)" n msg
        match msg with
            | Toggle -> return! active n
            | Add _ -> return! inactive n
            | Get ch -> 
                do! ch *<- n
                return! inactive n
        }

    start (active 0)           

let main = job {
    do! achan *<- (Add 10)          
    do! achan *<- Toggle            
    do! achan *<- (Add 20)         
    do! achan *<- Toggle           
    
    do! achan *<- (Add 30)          
    
    let rch = Ch<int> ()
    
    do! timeOutMillis 10
    
    printfn ""
    do! achan *<+ (Get rch)      // async send
    let! n = rch
    printfn "... got %d" n
    
    printfn ""
    do! achan *<- (Get rch)      // sync give 
    let! n2 = rch
    printfn "... got %d" n2
}

run main
printfn "\r\n... done"
//