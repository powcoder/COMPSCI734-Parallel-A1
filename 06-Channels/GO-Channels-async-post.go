https://powcoder.com
代写代考加微信 powcoder
Assignment Project Exam Help
Add WeChat powcoder
package main;

import "fmt"
import "time"

var achan = make (chan string);  // , 0, 1, 2, ...

func agent () { 
    var count = 0;
	for {
        var m = <- achan;
		count += 1;
        fmt.Printf ("take \"%s\"\r\n", m)
        time.Sleep (100*time.Millisecond)
    }
}

func setup () {
	go agent ();
	
    var s = []string {"the", "quick", "brown", "fox", "jumps", "over", "the", "lazy", "dog",}

	for _, m := range s {
        fmt.Printf ("send \"%s\"\r\n", m)
	   	achan <- m;
	}
    
    fmt.Printf ("\r\n")
}

func main () {
    setup ();
    time.Sleep (1000*time.Millisecond)
}

