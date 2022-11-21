GridActorsModel2
----------------

A simple model of a n1*n2 rectangular grid of actors, with a grid dataflow running from N to S (n1 axis) and W to E (n2 axis)

* Each actor waits for two messages, one from N and one from W

* Then, the actor simulates an intensive computation

* Finally, the actor posts two messages, one to S and one to E

* The process starts when the NW actor gets its two messages

* The process ends when the SE actor completes and sends its results to the main program, by way of a `TaskCompletionSource`

The sample dataflow matches the grid coordinates

GridActorsModel1
----------------

Similar scenario, using just n1 actors, allocated on the N to S axis. 

Each actor is subsequently reused n2 times, i.e. on the same W to E axis.

Only N to S messages are here needed.

Implementation Note
-------------------

These samples use F#'s own MailboxProcessor.

