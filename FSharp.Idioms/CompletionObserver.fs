namespace FSharp.Idioms

open System

type CompletionObserver<'p>(complete) =
    interface IObserver<'p> with
        member this.OnNext (inp:'p) = ()
        member this.OnError err = ()
        member this.OnCompleted() = complete ()

    new(lambda:Action) = CompletionObserver( fun () -> lambda.Invoke() )
