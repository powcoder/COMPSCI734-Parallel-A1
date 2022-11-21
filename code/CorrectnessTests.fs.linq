<Query Kind="FSharpProgram" />

let tests = [
    "abxy", "axby", 3;
    "GAC", "AGCAT", 2;
    "AGCAT", "GAC", 2;
    "XMJYAUZ", "MZJAWXU", 4;
    "ABCDGH", "AEDFHR", 3;
    "AGGTAB", "GXTXAYB", 4;
    "GXTXAYB", "AGGTAB", 4;
    "1234", "1224533324", 4;
    "1224533324", "1234", 4;
    "thisisatest", "testing123testing", 7;
    "testing123testing", "thisisatest", 7;
    "acbaed", "abcadf", 4;
    "this is a test", "testing 123 testing", 9;
    "this is a test Это тест 这是一个测试", "testing 123 testing тестирование 123 тестирование 测试123测试", 20;
    "a", "b", 0;
    "a", "a", 1;
    "a", "aaaaaa", 1;
    "aa", "abba", 2;
    ]

let naive (z1:string) (z2:string) : int =
    let n1, n2 = z1.Length, z2.Length
    let a = Array2D.create (n1+1) (n2+1) 0
    
    for i1 in 1..n1 do
        for i2 in 1..n2 do
            a.[i1,i2] <- if z1.[i1-1] = z2.[i2-1] then a.[i1-1,i2-1] + 1 else max a.[i1,i2-1] a.[i1-1,i2]
    //a.Dump("a")
    a.[n1,n2]

let duration f = 
    let timer = new System.Diagnostics.Stopwatch()
    timer.Start()
    let res = f()
    timer.Stop()
    printfn "... duration: %i ms" 
            timer.ElapsedMilliseconds
    res

let foo () =
    printfn "\r\nfoo"
    for s1, s2, res in tests do
        printfn "... s1:[%s], s2:[%s], r:%d ?" s1 s2 res
        let rn = naive s1 s2
        if rn <> res then printfn "*** rn:%d !" rn

let boo () =
    printfn "\r\nboo"
    let s1 = File.ReadAllText ("in1.txt")
    let s2 = File.ReadAllText ("in2.txt")
    let res = 220
    printfn "... s1.Length: %d, s2.Length: %d, r:%d ?" s1.Length s2.Length res
    
    let boo' () = 
        let rn = naive s1 s2
        rn
    
    let rn = duration boo'
    if rn <> res then printfn "*** rn:%d !" rn

let main () =
    foo ()
    boo ()

main ()
//