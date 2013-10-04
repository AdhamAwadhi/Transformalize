#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using Transformalize.Libs.Ninject.Infrastructure;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Providers
{
    /// <summary>
    ///     A provider that delegates to a callback method to create instances.
    /// </summary>
    /// <typeparam name="T">The type of instances the provider creates.</typeparam>
    public class CallbackProvider<T> : Provider<T>
    {
        /// <summary>
        ///     Initializes a new instance of the CallbackProvider&lt;T&gt; class.
        /// </summary>
        /// <param name="method">The callback method that will be called to create instances.</param>
        public CallbackProvider(Func<IContext, T> method)
        {
            Ensure.ArgumentNotNull(method, "method");
            Method = method;
        }

        /// <summary>
        ///     Gets the callback method used by the provider.
        /// </summary>
        public Func<IContext, T> Method { get; private set; }

        /// <summary>
        ///     Invokes the callback method to create an instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        protected override T CreateInstance(IContext context)
        {
            return Method(context);
        }
    }
}