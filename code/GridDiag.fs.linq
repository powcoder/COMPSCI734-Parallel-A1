<Query Kind="FSharpProgram" />

let n1, n2 = 3, 7

// --- lines referring to a are only needed for visualisation

let a = Array2D.create (n1+1) (n2+1) 0
let d = Array.create (n1+1) 0

for k = 2 to n1+n2 do
    let lo = max 1 (k-n2)
    let hi = min (k-1) n1
    for i = lo to hi do
        d.[i] <- k
        a.[i, k-i] <- d.[i] 
    if k = n1 then a.Dump "a pass1"
    elif k = n2 then a.Dump "a pass1"
a.Dump "a pass3=final"

// --- lines referring to a' are only needed for visualisation

let a' = Array2D.create (n1+1) (n2+1) 0
let d' = Array.create (n1+1) 0

for k = 2 to n1 do
    for i = 1 to k-1 do
        d'.[i] <- k
        a'.[i, k-i] <- d'.[i] 
a'.Dump "a' pass1"

for k = n1+1 to n2 do
    for i = 1 to n1 do
        d'.[i] <- k
        a'.[i, k-i] <- d'.[i] 
a'.Dump "a' pass2"

for k = n2+1 to n2+n1 do
    for i = k-n2 to n1 do
        d'.[i] <- k
        a'.[i, k-i] <- d'.[i] 
a'.Dump "a' pass3=final"

// ---
