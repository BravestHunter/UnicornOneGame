using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Core.Utils
{
    public static class DuckEnumerable
    {
        //public static IEnumerable<T> AsDuckEnumerable<T>(this object source)
        //{
        //    dynamic src = (dynamic)source;
        //
        //    dynamic e = src.GetEnumerator();
        //    try
        //    {
        //        while ((bool)e.MoveNext())
        //            yield return (T)e.Current;
        //    }
        //    finally
        //    {
        //        var d = e as IDisposable;
        //        if (d != null)
        //        {
        //            d.Dispose();
        //        }
        //    }
        //}
    }
}
