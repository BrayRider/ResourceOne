using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Fasterflect;

namespace RSM.Service.Library.Model.Reflection
{
	public class ModelMapper<T>
		where T:class, new()
	{
		public Type TargetType { get; private set; }
		public string[] PropertyNames { get; private set; }
		public Dictionary<string, MemberGetter> PropertyGetters { get; private set; }

		private ObjectMapper Mapper;

		public ModelMapper(string[] propNames)
		{
			TargetType = typeof(T);
			PropertyNames = propNames.ToArray();

			Mapper = TargetType.DelegateForMap(TargetType, MemberTypes.Property, MemberTypes.Property, Flags.InstancePublic, PropertyNames);

			PropertyGetters = new Dictionary<string,MemberGetter>();

			var props = TargetType.GetProperties();
			foreach (var prop in props)
			{
				PropertyGetters.Add(prop.Name, TargetType.DelegateForGetPropertyValue(prop.Name));
			}
		}

		/// <summary>
		/// If source is a different type, only properties that are identically named and typed will be
		/// copied to the target.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public T MapProperties(object source, T target)
		{
			Mapper.Invoke(source, target);
			return target;
		}

		public T MapProperties(T source, T target)
		{
			Mapper.Invoke(source, target);
			return target;
		}

		public T Clone(T source)
		{
			var target = new T();
			Mapper.Invoke(source, target);
			return target;
		}

		public object GetValue(T source, string property)
		{
			return PropertyGetters[property].Invoke(source);
		}
	}
}
