using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSM.Artifacts.Interfaces
{
	public enum TaskState
	{
		New,
		Loading,
		Idle,
		Running,
		Paused,
		Stopped
	}

	public interface ITask
	{
		TaskState State { get; }
		bool StopRequested { get; }

		void Start();
		void Pause();
		void Continue();
		void Stop(TimeSpan timeout);

		void Execute(Object stateInfo);
		void Load();
	}
}
