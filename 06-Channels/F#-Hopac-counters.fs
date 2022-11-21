https://powcoder.com
代写代考加微信 powcoder
Assignment Project Exam Help
Add WeChat powcoder
//
module Main

open System
open System.Threading

let tid () = Thread.CurrentThread.ManagedThreadId

open Hopac
open Hopac.Core
open Hopac.Infixes

let achan = Ch<string> ()

let agent = 
    let rec loop count = job {
        let! msg = achan
        //let! msg = Ch.take achan
        printfn "[%d] agent received %A, total=%d messages" (tid()) msg (count+1)
        do! timeOutMillis 100
        printfn "[%d] agent ..." (tid())
        return! loop (count+1)
        }
    start (loop 0) 

printfn "[%d] main" (tid())

let setup = job {
    for m in ["the"; "quick"; "brown"; "fox";] do 
        do! Ch.give achan m
    
    for m in ["jumps"; "over"; "the"; "lazy"; "dog";] do 
        do! achan *<- m
}

run setup

Console.WriteLine ("<enter> to exit")
Console.ReadLine () |> ignore

(*
sync post:   do! ch *<- m   // do! Ch.give ch m
async post:  do! ch *<+ m   // do! Ch.send ch m
receive:     let! m = ch    // do! m = Ch.take ch
*)