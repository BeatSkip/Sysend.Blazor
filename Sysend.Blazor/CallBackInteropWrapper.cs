using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sysend.Blazor
{
    public class CallBackInteropWrapper
    {
        [JsonPropertyName("__isCallBackWrapper")]
        public string IsCallBackWrapper { get; set; } = "";

        private CallBackInteropWrapper()
        {

        }
        public static CallBackInteropWrapper Create<T>(Func<T, Task> callback)
        {
            var res = new CallBackInteropWrapper
            {
                CallbackRef = DotNetObjectReference.Create(new JSInteropActionWrapper<T>(callback))
            };
            return res;
        }

        public static CallBackInteropWrapper Create<T1,T2>(Func<T1,T2, Task> callback)
        {
            var res = new CallBackInteropWrapper
            {
                CallbackRef = DotNetObjectReference.Create(new JSInteropActionWrapper<T1, T2>(callback))
            };
            return res;
        }

        public static CallBackInteropWrapper Create(Func<Task> callback)
        {
            var res = new CallBackInteropWrapper
            {
                CallbackRef = DotNetObjectReference.Create(new JSInteropActionWrapper(callback))
            };
            return res;
        }

        public object CallbackRef { get; set; }


        private class JSInteropActionWrapper
        {
            private readonly Func<Task> toDo;

            internal JSInteropActionWrapper(Func<Task> toDo)
            {
                this.toDo = toDo;
            }
            [JSInvokable]
            public async Task Invoke()
            {
                await toDo.Invoke();
            }
        }

        private class JSInteropActionWrapper<T>
        {
            private readonly Func<T, Task> toDo;

            internal JSInteropActionWrapper(Func<T, Task> toDo)
            {
                this.toDo = toDo;
            }
            [JSInvokable]
            public async Task Invoke(T arg1)
            {
                await toDo.Invoke(arg1);
            }
        }

        private class JSInteropActionWrapper<T1, T2>
        {
            private readonly Func<T1, T2, Task> toDo;

            internal JSInteropActionWrapper(Func<T1, T2, Task> toDo)
            {
                this.toDo = toDo;
            }
            [JSInvokable]
            public async Task Invoke(T1 arg1, T2 arg2)
            {
                await toDo.Invoke(arg1,arg2);
            }
        }
    }
}
