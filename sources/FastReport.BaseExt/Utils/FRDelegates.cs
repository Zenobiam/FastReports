namespace FastReport
{
#if DOTNET_4
    public delegate void FRAction<in T1>(T1 arg1);
    public delegate void FRAction<in T1, in T2>(T1 arg1, T2 arg2);
    public delegate void FRAction<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TResult FRFunc<out TResult>();
    public delegate TResult FRFunc<in T1, out TResult>(T1 arg1);
    public delegate TResult FRFunc<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
    public delegate TResult FRFunc<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
#else
    public delegate void FRAction<T1>(T1 arg1);
    public delegate void FRAction<T1, T2>(T1 arg1, T2 arg2);
    public delegate void FRAction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TResult FRFunc<TResult>();
    public delegate TResult FRFunc<T1, TResult>(T1 arg1);
    public delegate TResult FRFunc<T1, T2, TResult>(T1 arg1, T2 arg2);
    public delegate TResult FRFunc<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
#endif

}
