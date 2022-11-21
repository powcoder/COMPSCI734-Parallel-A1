https://powcoder.com
代写代考加微信 powcoder
Assignment Project Exam Help
Add WeChat powcoder
package main;

import "fmt"
import "time"

var achan = make (chan string);

func agent () { 
    var count = 0;
	for {
        var msg = <- achan;
		count += 1;
        fmt.Printf ("agent received \"%s\", total=%d messages\r\n", msg, count)
        time.Sleep (100*time.Millisecond)
        fmt.Printf ("agent ...\r\n");
    }
}

func setup () {
	fmt.Printf ("... main\r\n");
	
	go agent ();
	
	for _, m := range []string {"the", "quick", "brown", "fox",} {
	   	achan <- m;
	}
	    
	for _, m := range []string {"jumps", "over", "the", "lazy", "dog",} {
	   	achan <- m;
	}
}

func main () {
    setup ();
    fmt.Printf ("... done\r\n"); 
}

// main ();

