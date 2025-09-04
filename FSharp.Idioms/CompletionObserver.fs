namespace FSharp.Idioms

open System

type CompletionObserver<'p>(complete: unit -> unit) =
    interface IObserver<'p> with
        member _.OnNext(_: 'p) = ()
        member _.OnError _ = ()
        member _.OnCompleted() = complete ()

    static member from(lambda: Action) =
        CompletionObserver(fun () -> lambda.Invoke())
