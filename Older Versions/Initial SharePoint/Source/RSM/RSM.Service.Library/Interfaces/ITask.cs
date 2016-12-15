using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSM.Service.Library.Interfaces
{
	public interface ITask
	{
		TaskState State { get; }
		TaskProfile Profile { get; }

		void Start(string[] args);
		void Pause();
		void Continue();
		void Stop();
		void Shutdown();

		Result<string> Execute(Object stateInfo);
		void Load(string[] args = null);
	}
}
