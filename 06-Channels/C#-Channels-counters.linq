<Query Kind="Program">
  <Reference>System.Threading.Channels.dll</Reference>
  <Reference>System.Threading.Tasks.Extensions.dll</Reference>
  <Namespace>System</Namespace>
  <Namespace>System.Threading</Namespace>
  <Namespace>System.Threading.Channels</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Threading.Channels;

Func <int> tid = () => Thread.CurrentThread.ManagedThreadId;

static BoundedChannelOptions one2one = new BoundedChannelOptions (1) {SingleWriter=true, SingleReader=true,};

Channel<string> achan = Channel.CreateBounded<string> (one2one);

async Task agent () { 
    var count = 0;
	for (;;) {
        var msg = await achan.Reader.ReadAsync ();
		count += 1;
        Console.WriteLine ("[{0}] agent received {1}, total={2} messages", tid(), msg, count);
        await Task.Delay (100);
        Console.WriteLine ("[{0}] agent ...", tid());
    }
}

async Task Main() {
	Console.WriteLine ("[{0}] main", tid());
	
	var a = agent ();
	
	foreach (var m in new[] {"the", "quick", "brown", "fox",}) {
	   	await achan.Writer.WriteAsync (m);
	}
	    
	foreach (var m in new[] {"jumps", "over", "the", "lazy", "dog",}) { 
	   	await achan.Writer.WriteAsync (m);
	}

	//Console.WriteLine ("<enter> to exit");
	//Console.ReadLine ();
	
	//return 0;
}