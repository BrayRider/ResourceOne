using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using RSM.Service.Library.Controllers;
using RSM.Service.Library.Model.Reflection;

namespace RSM.Service.Library.Model.Reflection
{
	public class EntityFilters<T>
		where T: class, new()
	{
		public static string FilterSettingFormat = "Filter.{0}.{1}";

		public Dictionary<string, string> Filters { get; set; }

		public ModelMapper<T> Mapper { get; set; }

		public EntityFilters(ModelMapper<T> mapper)
		{
			Filters = new Dictionary<string, string>();
			Mapper = mapper;
		}

		public string FilterName(string field, string className = null)
		{
			return string.Format(FilterSettingFormat, (className == null) ? Mapper.TargetType.Name : className, field);
		}

		public EntityFilters<T> Load(TaskSettings settings)
		{
			var className = Mapper.TargetType.Name;

			foreach (var key in Mapper.PropertyGetters.Keys)
			{
				var name = FilterName(key, className);
				var filter = settings.GetValue(name);
				if (filter == null)
					continue;

				Filters.Add(key, filter);
			}
			return this;
		}

		public bool IsMatch(T entity)
		{
			if (Filters.Count == 0)
				return true;

			var mapper = Mapper;

			//All regular expression filters defined must match otherwise the entity will be ignored.
			foreach (var filter in Filters)
			{
				var value = (string)mapper.PropertyGetters[filter.Key].Invoke(entity) ?? string.Empty;

				if (!Regex.IsMatch(value, filter.Value))
					return false;
			}

			return true;
		}
	}
}
