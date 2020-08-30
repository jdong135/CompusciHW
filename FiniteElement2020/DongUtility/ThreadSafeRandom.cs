﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DongUtility
{
    /// <summary>
    /// A thread-safe random class
    /// Thanks, StackOverflow!
    /// </summary>
    public static class ThreadSafeRandom
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static Random Random()
        {
            return random.Value;
        }
    }

}
