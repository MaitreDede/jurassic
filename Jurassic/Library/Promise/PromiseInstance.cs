using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jurassic.Library
{
    [Serializable]
    public partial class PromiseInstance : ObjectInstance
    {
        private PromiseState m_state = PromiseState.Pending;
        private bool m_finalizable = false;
        private readonly List<object> m_fulfilledNext = new List<object>();
        private readonly List<object> m_rejectededNext = new List<object>();

        public PromiseInstance(ObjectInstance prototype) : base(prototype)
        {
        }

        /// <summary>
        /// Creates the Map prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, PromiseConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(engine.Symbol.ToStringTag, "Promise", PropertyAttributes.Configurable));

            //// From the spec: the initial value of the @@iterator property is the same function
            //// object as the initial value of the Map.prototype.entries property.
            //PropertyNameAndValue entriesProperty = properties.Find(p => "entries".Equals(p.Key));
            //if (entriesProperty == null)
            //    throw new InvalidOperationException("Expected entries property.");
            //properties.Add(new PropertyNameAndValue(engine.Symbol.Iterator, entriesProperty.Value, PropertyAttributes.NonEnumerable));

            result.FastSetProperties(properties);
            return result;
        }

        private object m_result;

        internal void PerformResolve(object result)
        {
            if (this.m_state != PromiseState.Pending)
                return;
            this.m_result = result;
            this.m_state = PromiseState.Fulfilled;
            this.m_finalizable = true;
        }

        internal void PerformReject(object result)
        {
            if (this.m_state != PromiseState.Pending)
                return;
            this.m_result = result;
            this.m_state = PromiseState.Rejected;
            this.m_finalizable = true;
        }

        [JSInternalFunction(Name = "catch")]
        public PromiseInstance Catch(object onRejected)
        {
            return this.Then(null, onRejected);
        }

        [JSInternalFunction(Name = "then")]
        public PromiseInstance Then(object onFulfilled, object onRejected)
        {
            this.CheckAdd(onFulfilled, this.m_fulfilledNext);
            this.CheckAdd(onRejected, this.m_rejectededNext);

            return this;
        }

        private void CheckAdd(object item, List<object> lst)
        {
            if (item != null && item != Undefined.Value && item != Null.Value)
            {
                lst.Add(item);
            }
        }

        public bool IsFinalizable { get { return this.m_finalizable; } }

        internal void FinalizePromise()
        {
            if (!this.m_finalizable)
            {
                throw new InvalidOperationException();
            }
            throw new NotImplementedException();
        }
    }
}
