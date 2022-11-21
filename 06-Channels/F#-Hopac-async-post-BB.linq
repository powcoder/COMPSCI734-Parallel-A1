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
    
let achan = BoundedMb<string> (1)  // 0, 1, 2, ...

let agent = 
    let rec loop count = job {
        let! m = BoundedMb.take achan
        Console.Error.WriteLine ("[{0}] take {1}", tid(), m)
        do! timeOutMillis 100
        return! loop (count+1)
        }
    start (loop 0) 

let setup = job {
    let s = ["the"; "quick"; "brown"; "fox"; "jumps"; "over"; "the"; "lazy"; "dog";]
    
    for m in s do 
        Console.Error.WriteLine ("[{0}] send {1}", tid(), m)
        do! BoundedMb.put achan m
    
    Console.Error.WriteLine ()
    }

run setup
//