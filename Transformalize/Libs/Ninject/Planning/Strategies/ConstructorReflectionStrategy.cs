#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

#region Using Directives

using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Injection;
using Transformalize.Libs.Ninject.Planning.Directives;
using Transformalize.Libs.Ninject.Selection;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Strategies
{
    /// <summary>
    ///     Adds a directive to plans indicating which constructor should be injected during activation.
    /// </summary>
    public class ConstructorReflectionStrategy : NinjectComponent, IPlanningStrategy
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstructorReflectionStrategy" /> class.
        /// </summary>
        /// <param name="selector">The selector component.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        public ConstructorReflectionStrategy(ISelector selector, IInjectorFactory injectorFactory)
        {
            Ensure.ArgumentNotNull(selector, "selector");
            Ensure.ArgumentNotNull(injectorFactory, "injectorFactory");

            Selector = selector;
            InjectorFactory = injectorFactory;
        }

        /// <summary>
        ///     Gets the selector component.
        /// </summary>
        public ISelector Selector { get; private set; }

        /// <summary>
        ///     Gets the injector factory component.
        /// </summary>
        public IInjectorFactory InjectorFactory { get; set; }

        /// <summary>
        ///     Adds a <see cref="ConstructorInjectionDirective" /> to the plan for the constructor
        ///     that should be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            Ensure.ArgumentNotNull(plan, "plan");

            var constructors = Selector.SelectConstructorsForInjection(plan.Type);
            if (constructors == null)
                return;

            foreach (var constructor in constructors)
            {
                plan.Add(new ConstructorInjectionDirective(constructor, InjectorFactory.Create(constructor)));
            }
        }
    }
}