<Query Kind="FSharpProgram" />

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
// -------------------------------------

let mutable PARALLEL = true
let mutable AGENTS1 = true

// -------------------------------------

let mutable str1's = Array.zeroCreate<string> 0
let mutable str2's = Array.zeroCreate<string> 0
let mutable sent1's = Array.zeroCreate<int[]> 0
let mutable sent2's = Array.zeroCreate<int[]> 0
let mutable diag0's = Array.zeroCreate<int[]> 0
let mutable diag1's = Array.zeroCreate<int[]> 0
let mutable diag2's = Array.zeroCreate<int[]> 0

// -------------------------------------

let par_split (str1:string) (str2:string) (div1:int) (div2:int) =
    str1.Dump "str1"
    str2.Dump "str2"
    let n1, n2 = str1.Length, str2.Length

    let split' n div =
        let q, r = n/div, n%div
        let l = List.replicate r (q+1) @ if q = 0 then [] else List.replicate (div-r) q
        let b = l |> List.scan (fun s n -> s+n) 0
        let z = Seq.zip b l
        Seq.toArray z
        
    let z1, z2 = split' n1 div1, split' n2 div2
    // s1:abcde; s2:abcdefg
    // n1:5; n2:7; div1:2; div2:3; z1:[|(0, 3); (3, 2)|]; z2:[|(0, 3); (3, 2); (5, 2)|]
    // zi:[| (beg', len) ... |]

    printfn "z1: %A" z1
    printfn "z2: %A" z2

    let str1', str2' = "?" + str1, "?" + str2

    str1's <- z1 |> Array.map (fun (b, l) -> str1'.Substring (b, l+1))
    str2's <- z2 |> Array.map (fun (b, l) -> str2'.Substring (b, l+1))

    printfn "str1's: %A" str1's
    printfn "str2's: %A" str2's

    sent1's <- z1 |> Array.map (fun (_, l) -> Array.zeroCreate<int> (l+1))
    sent2's <- z2 |> Array.map (fun (_, l) -> Array.zeroCreate<int> (l+1))

    printfn "sent1's: %A" sent1's
    printfn "sent2's: %A" sent2's

    diag0's <- z1 |> Array.map (fun (_, l) -> Array.zeroCreate<int> (l+1))
    diag1's <- z1 |> Array.map (fun (_, l) -> Array.zeroCreate<int> (l+1))
    diag2's <- z1 |> Array.map (fun (_, l) -> Array.zeroCreate<int> (l+1))

    //printfn "diag0's: %A" diag0's
    //printfn "diag1's: %A" diag1's
    //printfn "diag2's: %A" diag2's

    (z1, z2)

// -------------------------------------
par_split "abcde" "abcdefg" 2 3 |> ignore
par_split "abcd" "abbdecccbd" 2 3 |> ignore
// -------------------------------------