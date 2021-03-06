﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using sql4js.Functions;

namespace sql4js.Parser
{
    public static class S4JDefaultStateBag
    {
        static Object lck = new Object();
        static S4JStateBag i;
        public static S4JStateBag Get()
        {
            

            if (i == null)
                lock (lck)
                    if (i == null)
                    {
                        i = new S4JStateBag();
                        i.AddStatesToBag(
                            new CSharpFunction("c#"),
                            new DynLanFunction("dynlan"),
                            new TSqlFunction("sql"));
                    }

            return i;//.Clone();
        }
    }
}
