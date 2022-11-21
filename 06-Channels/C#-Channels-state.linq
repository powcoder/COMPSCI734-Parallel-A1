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
//using System.Threading.Tasks.Extensions;
//using System.Threading.Channels;

Func <int> tid = () => Thread.CurrentThread.ManagedThreadId;

static BoundedChannelOptions one2one = new BoundedChannelOptions (1) {Capacity=1, SingleWriter=true, SingleReader=true,};

enum MessageTag {Toggle, Add, Get };

struct Message { 
	public MessageTag Tag; 
	public object Val; 
	public override string ToString () { 
		return Tag.ToString() + (Val != null? " " + Val.ToString(): ""); }
	}; 

Channel<Message> achan = Channel.CreateBounded<Message> (one2one);

enum State {Active, Inactive};

async Task agent () { 
    var state = State.Active;
	var deposit = 0;
	Message msg;
	
	for (;;) {
		switch (state) {
		case State.Active:
			msg = await achan.Reader.ReadAsync ();
        	Console.WriteLine ("active   {0} ({1})", deposit, msg);
        
        	switch (msg.Tag) {
			case MessageTag.Toggle:
				state = State.Inactive;
				break;
			case MessageTag.Add:
				deposit += (int) msg.Val;
				break;
			case MessageTag.Get:
				//msg.Dump ();
				var rch = (Channel<int>) msg.Val;
				await rch.Writer.WriteAsync (deposit);
				break;
			}
			break;
		
		case State.Inactive: 
			msg = await achan.Reader.ReadAsync ();
        	Console.WriteLine ("inactive {0} ({1})", deposit, msg);
        	
			switch (msg.Tag) {
			case MessageTag.Toggle:
				state = State.Active;
				break;
			case MessageTag.Add:
				; // nothing
				break;
			case MessageTag.Get:
				var rch = (Channel<int>) msg.Val;
				await rch.Writer.WriteAsync (deposit);
				break;
			}
			break;
		}
    }
}

async Task Main() {
	var a = agent ();
	
   	await achan.Writer.WriteAsync (new Message {Tag=MessageTag.Add, Val=10});
   	await achan.Writer.WriteAsync (new Message {Tag=MessageTag.Toggle, Val=null});
   	await achan.Writer.WriteAsync (new Message {Tag=MessageTag.Add, Val=20});
   	await achan.Writer.WriteAsync (new Message {Tag=MessageTag.Toggle, Val=null});
   	await achan.Writer.WriteAsync (new Message {Tag=MessageTag.Add, Val=30});
	
	Thread.Sleep (10);

	Console.WriteLine ();
	var rch = Channel.CreateBounded<int> (one2one);
   	await achan.Writer.WriteAsync (new Message {Tag=MessageTag.Get, Val=rch});
	var n = await rch.Reader.ReadAsync ();
	Console.WriteLine ("... got {0}", n);

	Console.WriteLine ();
	await achan.Writer.WriteAsync (new Message {Tag=MessageTag.Get, Val=rch});
	var n2 = await rch.Reader.ReadAsync ();
	Console.WriteLine ("... got {0}", n2);

	//Console.WriteLine ("<enter> to exit");
	//Console.ReadLine ();
}