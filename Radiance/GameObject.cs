using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
    /// <summary>
    /// Provides the abstract base class for deriving all game related objects.
    /// </summary>
    public abstract class GameObject : IGameObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for the current object.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of properties associated with the current object.
        /// </summary>
        public PropertyCollection Properties { get; set; }

        /// <summary>
        /// Initializes a new instance of the GameObject class.
        /// </summary>
        protected GameObject()
        {
            this.Properties = new PropertyCollection();
        }
    }
}
