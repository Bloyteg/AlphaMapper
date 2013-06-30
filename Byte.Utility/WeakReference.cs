// Copyright 2013 Joshua R. Rodgers
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================

using System;

namespace Byte.Utility
{
    public class WeakReference<TRef>
        where TRef : class
    {
        private readonly WeakReference _reference;

        public WeakReference(TRef target)
        {
            _reference = new WeakReference(target);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TRef"/> to <see cref="Byte.Utility.WeakReference&lt;TRef&gt;"/>.
        /// </summary>
        /// <param name="newObject">The new object.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator WeakReference<TRef>(TRef newObject)
        {
            return new WeakReference<TRef>(newObject);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Byte.Utility.WeakReference&lt;TRef&gt;"/> to <see cref="TRef"/>.
        /// </summary>
        /// <param name="refernece">The refernece.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator TRef(WeakReference<TRef> refernece)
        {
            if(refernece == null)
            {
                return null;
            }

            return refernece.Target;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlive
        {
            get { return _reference.IsAlive; }
        }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public TRef Target
        {
            get { return _reference.Target as TRef; }
            set { _reference.Target = value; }
        }
    }
}
