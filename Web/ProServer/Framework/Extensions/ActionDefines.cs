using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 返回值为bool的回调
    /// </summary>
    /// <returns></returns>
    public delegate bool BoolAction();


    /// <summary>
    /// 返回值为bool的回调
    /// </summary>
    public delegate bool BoolAction<in T>(T obj);

    /// <summary>
    /// 返回值为bool的回调
    /// </summary>
    public delegate bool BoolAction<in T1, in T2>(T1 obj1, T2 obj2);

    /// <summary>
    /// 返回值为bool的回调
    /// </summary>
    public delegate bool BoolAction<in T1, in T2, in T3>(T1 obj1, T2 obj2, T3 obj3);

    /// <summary>
    /// 返回值为bool的回调
    /// </summary>
    public delegate bool BoolAction<in T1, in T2, in T3, in T4>(T1 obj1, T2 obj2, T3 obj3, T4 obj4);

    /// <summary>
    /// 返回值为int的回调
    /// </summary>
    /// <returns></returns>
    public delegate int IntAction();

    /// <summary>
    /// 返回值为int的回调
    /// </summary>
    public delegate int IntAction<in T>(T obj);

    /// <summary>
    /// 返回值为int的回调
    /// </summary>
    public delegate int IntAction<in T1, in T2>(T1 obj1, T2 obj2);

    /// <summary>
    /// 返回值为int的回调
    /// </summary>
    public delegate int IntAction<in T1, in T2, in T3>(T1 obj1, T2 obj2, T3 obj3);

    /// <summary>
    /// 返回值为int的回调
    /// </summary>
    public delegate int IntAction<in T1, in T2, in T3, in T4>(T1 obj1, T2 obj2, T3 obj3, T4 obj4);

    /// <summary>
    /// 返回值为String的回调
    /// </summary>
    /// <returns></returns>
    public delegate string StringAction();
}
