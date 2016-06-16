using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jurassic.Library
{
    [Serializable]
    public partial class PromiseConstructor : ClrStubFunction
    {
        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new map constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal PromiseConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = new List<PropertyNameAndValue>();
            InitializeConstructorProperties(properties, "Promise", 0, PromiseInstance.CreatePrototype(Engine, this));
            FastSetProperties(properties);
        }

        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the Map object is invoked like a function, e.g. var x = Promise().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Constructor Promise requires 'new'");
        }

        [JSConstructorFunction]
        public PromiseInstance Construct(object executor)
        {
            if (executor == Undefined.Value)
                throw new JavaScriptException(this.Engine, ErrorType.TypeError, "Executor is undefined");

            if (!(executor is FunctionInstance))
                throw new JavaScriptException(this.Engine, ErrorType.TypeError, "Executor must be callable");

            // Create a new set.
            var result = new PromiseInstance(this.InstancePrototype);

            //// If iterable is not null or undefined, then iterate through the values and add them to the set.
            //if (iterable != Undefined.Value && iterable != Null.Value)
            //{
            //    var iterator = TypeUtilities.GetIterator(Engine, TypeConverter.ToObject(Engine, iterable));
            //    if (iterator != null)
            //    {
            //        // Get a reference to the set function.
            //        var setFunc = result["set"] as FunctionInstance;
            //        if (setFunc == null)
            //            throw new JavaScriptException(Engine, ErrorType.TypeError, "Missing 'set' function");

            //        // Call the set function for each value.
            //        foreach (var value in TypeUtilities.Iterate(Engine, iterator))
            //        {
            //            var obj = value as ObjectInstance;
            //            if (obj == null)
            //                throw new JavaScriptException(Engine, ErrorType.TypeError, $"Expected iterator return value to be an object, but was {value}");
            //            setFunc.Call(result, obj[0], obj[1]);
            //        }
            //    }
            //}

            this.Engine.AddPromise(result);

            return result;
        }

        [JSInternalFunction(Name = "all")]
        public PromiseInstance All(object iterable)
        {
            throw new NotImplementedException();
        }

        [JSInternalFunction(Name = "race")]
        public PromiseInstance Race(object iterable)
        {
            throw new NotImplementedException();
        }

        [JSInternalFunction(Name = "reject")]
        public PromiseInstance Reject(object r)
        {
            ClrStubFunction func = new ClrStubFunction(this.Engine.FunctionInstancePrototype, (engine, thisObj, args) =>
            {
                FunctionInstance resolve = (FunctionInstance)args[0];
                FunctionInstance reject = (FunctionInstance)args[1];
                reject.Call(thisObj, r);
                return Undefined.Value;
            });
            return this.Construct(func);
        }

        [JSInternalFunction(Name = "resolve")]
        public PromiseInstance Resolve(object x)
        {
            ClrStubFunction func = new ClrStubFunction(this.Engine.FunctionInstancePrototype, (engine, thisObj, args) =>
            {
                FunctionInstance resolve = (FunctionInstance)args[0];
                FunctionInstance reject = (FunctionInstance)args[1];
                resolve.Call(thisObj, x);
                return Undefined.Value;
            });
            return this.Construct(func);
        }
    }
}
