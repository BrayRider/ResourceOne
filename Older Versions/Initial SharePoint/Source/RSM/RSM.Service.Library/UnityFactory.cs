using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace RSM.Service.Library
{
	public class UnityFactory
	{
		public IUnityContainer Container { get; private set; }

		public UnityFactory(string configuration)
		{
			Contract.Assert(!string.IsNullOrEmpty(configuration), "Container configuration name cannot be null or empty");

			Container = new UnityContainer().LoadConfiguration(configuration);
		}

		public T Create<T>()
		{
			return Container.Resolve<T>();
		}
	}
}
