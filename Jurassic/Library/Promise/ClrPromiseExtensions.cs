using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jurassic.Library
{
    public static class ClrPromiseExtensions
    {
        public static PromiseInstance Construct(this PromiseConstructor ctor, Action<Action<object>, Action<object>> resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            ClrStubFunction funcResolve = new ClrStubFunction(ctor.Engine.FunctionInstancePrototype, (engine, thisObj, args) =>
            {
                FunctionInstance jsResolve = (FunctionInstance)args[0];
                FunctionInstance jsReject = (FunctionInstance)args[1];

                resolver(
                    (r) => jsResolve.Call(null, r),
                    (x) => jsReject.Call(null, x)
                    );

                return null;
            });
            return ctor.Construct(executor: funcResolve);
        }
    }
}
