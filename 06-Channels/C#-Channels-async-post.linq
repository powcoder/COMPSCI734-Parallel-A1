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

static BoundedChannelOptions one2one = new BoundedChannelOptions (1)  // 1, 2, ...
    {SingleWriter=true, SingleReader=true,};

Channel<string> achan = Channel.CreateBounded<string> (one2one);

async Task agent () { 
    var count = 0;
	for (;;) {
        var m = await achan.Reader.ReadAsync ();
		count += 1;
        Console.Error.WriteLine ("[{0}] read {1}", tid(), m);
        await Task.Delay (100);
    }
}

async Task Main() {
	var a = agent ();
    
    var s = new[] {"the", "quick", "brown", "fox", "jumps", "over", "the", "lazy", "dog",};    
	
	foreach (var m in s) {
        Console.Error.WriteLine ("[{0}] write {1}", tid(), m);
	   	await achan.Writer.WriteAsync (m);
	}
    
    Console.Error.WriteLine ();
}
//