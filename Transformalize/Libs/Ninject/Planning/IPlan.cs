#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using System.Collections.Generic;
using Transformalize.Libs.Ninject.Planning.Directives;

#endregion

namespace Transformalize.Libs.Ninject.Planning
{
    /// <summary>
    ///     Describes the means by which a type should be activated.
    /// </summary>
    public interface IPlan
    {
        /// <summary>
        ///     Gets the type that the plan describes.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     Adds the specified directive to the plan.
        /// </summary>
        /// <param name="directive">The directive.</param>
        void Add(IDirective directive);

        /// <summary>
        ///     Determines whether the plan contains one or more directives of the specified type.
        /// </summary>
        /// <typeparam name="TDirective">The type of directive.</typeparam>
        /// <returns>
        ///     <c>True</c> if the plan has one or more directives of the type; otherwise, <c>false</c>.
        /// </returns>
        bool Has<TDirective>() where TDirective : IDirective;

        /// <summary>
        ///     Gets the first directive of the specified type from the plan.
        /// </summary>
        /// <typeparam name="TDirective">The type of directive.</typeparam>
        /// <returns>
        ///     The first directive, or <see langword="null" /> if no matching directives exist.
        /// </returns>
        TDirective GetOne<TDirective>() where TDirective : IDirective;

        /// <summary>
        ///     Gets all directives of the specified type that exist in the plan.
        /// </summary>
        /// <typeparam name="TDirective">The type of directive.</typeparam>
        /// <returns>A series of directives of the specified type.</returns>
        IEnumerable<TDirective> GetAll<TDirective>() where TDirective : IDirective;
    }
}