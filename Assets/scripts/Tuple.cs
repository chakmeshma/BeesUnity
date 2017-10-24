public class Tuple<T1, T2> : System.Object
{
    public T1 First { get; private set; }
    public T2 Second { get; private set; }
    internal Tuple(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }

    public override bool Equals(object obj)
    {
        Tuple<T1, T2> tuple = ((Tuple<T1, T2>)obj);


        return (tuple.First.Equals(this.First) && tuple.Second.Equals(this.Second));
    }
}

public static class Tuple
{
    public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
    {
        var tuple = new Tuple<T1, T2>(first, second);
        return tuple;
    }
}