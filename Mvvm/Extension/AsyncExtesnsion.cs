using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.Extension
{
    public static partial class Extensions
    {
        ////only 1 thread can access
        //private static readonly SemaphoreSlim _sl = new SemaphoreSlim(initialCount: 1);

        //public async Task SeraphoneAsync<T>(this Task<T> task)
        //{
        //    await _sl.WaitAsync();

        //    try
        //    {
        //        await task;
        //        // Do async work here

        //    }
        //    finally
        //    {
        //        // Release the thread
        //        _sl.Release();
        //    }
        //}

    }
}
