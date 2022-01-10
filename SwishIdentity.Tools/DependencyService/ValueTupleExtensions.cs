namespace SwishIdentity.Tools.DependencyService
{
    public static class ValueTupleExtensions
    {
        public static (T1, T2, T3) With<T1, T2, T3>(this (T1, T2) tuple, T3 add)
        {
            return (tuple.Item1, tuple.Item2, add);
        }

        public static (T1, T2, T3, T4) With<T1, T2, T3, T4>(this (T1, T2, T3) tuple, T4 add)
        {
            return (tuple.Item1, tuple.Item2, tuple.Item3, add);
        }
    }
}