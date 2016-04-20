using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Radiance.Contract
{
	public static class KnownTypesProvider
	{
		public static Type[] GetKnownTypes(ICustomAttributeProvider attributeTarget)
		{
			Type dataContractType = typeof(System.Runtime.Serialization.DataContractAttribute);
			Type serviceContractType = (Type)attributeTarget;
			Type[] exportedTypes = serviceContractType.Assembly.GetExportedTypes();
			List<Type> knownTypes = new List<Type>();
			foreach (Type type in exportedTypes)
			{
				if (System.Attribute.IsDefined(type, dataContractType, false))
				{
					knownTypes.Add(type);
				}
			}
			return knownTypes.ToArray();
		}
	}
}
