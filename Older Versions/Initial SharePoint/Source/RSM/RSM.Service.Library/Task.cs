using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using RSM.Service.Library.Controllers;
using RSM.Service.Library.Interfaces;
using RSM.Service.Library.Model;
using RSMDB = RSM.Support;

namespace RSM.Service.Library
{
	/// <summary>
	/// Base class for a Task that runs in the RSM Service.
	/// </summary>
	public abstract class Task : ITask, IDisposable	
	{
		#region Properties
		public string Name { get; internal set; }
		public string ActivityName { get; protected set; }
		public string FullName { get; protected set; }

		protected int _state;
		public TaskState State 
		{
			get
			{
				return (TaskState)_state;
			}
			set
			{
				// must be protected for multi-threaded access
				Interlocked.Exchange(ref _state, (int)value);
			}
		}

		public bool PauseRequested 
		{
			get
			{
				return PauseEvent.WaitOne(0);
			}
		}

		public TaskProfile Profile { get; internal set; }

		public ServiceProfile ServiceProfile { get; internal set; }

		private Timer Timer { get; set; }

		private ManualResetEvent RunEvent { get; set; }

		private ManualResetEvent PauseEvent { get; set; }

		private long _iterations;
		public long Iterations
		{
			get
			{
				return _iterations;
			}
		}

		public ExternalSystem ExternalSystem { get; protected set; }

		public UnityFactory UnityFactory { get; protected set; }

		#endregion

		public Task()
		{
			State = TaskState.New;
			RunEvent = new ManualResetEvent(false);
			PauseEvent = new ManualResetEvent(false);
			Timer = new Timer(this.ExecuteWrapper, RunEvent, Timeout.Infinite, Timeout.Infinite);
			_iterations = 0;
		}

		public static Task Create(string taskName, ServiceProfile svcProfile, string configPrefix = null)
		{
			var container = new UnityFactory(taskName);

			var task = container.Create<ITask>() as Task;
			task.UnityFactory = container;

			task.ServiceProfile = svcProfile;
			task.Name = taskName;
			task.FullName = string.Format("{0}.{1}", task.Name, task.ActivityName);

			task.Profile = new TaskProfile(task, configPrefix);

			return task;
		}

		#region ITask Methods
		public virtual void Start(string[] args)
		{
			try
			{
				State = TaskState.Loading;
				var due = Profile.Schedule.NextDueTime();
				LogMessage("starting... Repeat: {0}, Interval: {1} min", Profile.Schedule.Repeat, Profile.Schedule.RepeatInterval.TotalMinutes);

				State = TaskState.Idle;
				Timer.Change(Profile.Schedule.NextDueTime(_iterations), Profile.Schedule.NextInterval());
			}
			catch (Exception e)
			{
				LogException(e, "failed Start process");
			}
		}

		public virtual void Pause()
		{
			try
			{
				PauseEvent.Set();
				//Timer.Change(Timeout.Infinite, Timeout.Infinite);

				// Wait for any current execution to finish
				var timeout = Profile.StopTaskTimeout;
				if (RunEvent.WaitOne(0)) 
				{
					ServiceProfile.Service.RequestAdditionalTime((int)timeout.TotalMilliseconds);
					if (RunEvent.WaitOne(timeout))
					{
						LogWarning("still running during Pause process. Wait timer exceeded.");
					}
				}

				State = TaskState.Paused;

			}
			catch (Exception e)
			{
				LogException(e, "failed Pause process.");
			}
		}

		public virtual void Continue()
		{
			try
			{
				PauseEvent.Reset();
				//Timer.Change(Profile.Schedule.NextDueTime(_iterations), Profile.Schedule.NextInterval());
			}
			catch (Exception e)
			{
				LogException(e, "failed Continue process");
			}
		}

		public virtual void Stop()
		{
			try
			{
				Timer.Change(Timeout.Infinite, Timeout.Infinite);

				// wait for any executions to finish
				if (RunEvent.WaitOne(0))
				{
					var timeout = Profile.StopTaskTimeout;
					ServiceProfile.Service.RequestAdditionalTime((int)timeout.TotalMilliseconds);
					if (RunEvent.WaitOne(timeout))
					{
						LogWarning("wait timer exeeded Stop process.");
					}
				}

				LogMessage("stopped.");

				// At this point we stop regardless...
				Dispose();
				State = TaskState.Stopped;
			}
			catch (Exception e)
			{
				LogException(e, "failed Stop process.");
			}
		}

		public virtual void Shutdown()
		{
			Stop();
		}

		/// <summary>
		/// This is expected to be overriden to perform the actual task logic.
		/// </summary>
		/// <param name="stateInfo"></param>
		public virtual Result<string> Execute(object stateInfo)
		{
			return Result<string>.Success();
		}

		/// <summary>
		/// This is used to load the task with any details required for it to run, such as configuration information. This is invoked once
		/// during the creation of a task.
		/// </summary>
		/// <param name="args">command line arguements provided during start of this service</param>
		public virtual void Load(string[] args = null)
		{
			LogMessage("loading...");
		}
		#endregion


		#region Helpers
		protected virtual void ExecuteWrapper(object stateInfo)
		{
			try
			{
				// if already running then do nothing
				if (RunEvent.WaitOne(0))
				{
					LogWarning("another thread already running.");
					return;
				}

				if (PauseEvent.WaitOne(0))
				{
					LogWarning("service is paused.");
					return;
				}

				var result = Result<string>.Success();
				var batch = new RSMDB.BatchHistory
				{
					RunStart = DateTime.Now,
					SystemId = this.ExternalSystem.Id,
				};

				try
				{
					RunEvent.Set();
					Interlocked.Increment(ref _iterations);
					LogMessage("executing.");

					State = TaskState.Running;
					result = Execute(stateInfo);
				}
				finally
				{
					State = TaskState.Idle;
					RunEvent.Reset();

					using (var controller = new BatchHistories())
					{
						var text = result != null ? result.Entity : string.Empty;
						var msg = (result.Succeeded)
							? LogMessageDetail(result.ToString(), "completed. {0}", text)
							: LogErrorDetail(result.ToString(), "execution failed. {0}", text);

						batch.RunEnd = DateTime.Now;
						batch.Message = msg;
						batch.Outcome = (int)result.Outcome;
						controller.Add(batch);
					}
				}
			}
			catch (Exception e)
			{
				LogException(e, "execution failed.");
			}
		}

		/// <summary>
		/// Responsible for creating an API instance for this task.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		protected virtual IAPI CreateAPI(Config config)
		{
			return UnityFactory.Create<IAPI>();
		}

		#endregion

		#region IDisposable
		private bool disposed = false; // to detect redundant calls

		~Task()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;

			if (disposing)
			{
				if (Timer != null)
				{
					Timer.Dispose();
					Timer = null;
				}
				if (PauseEvent != null)
				{
					PauseEvent.Close();
					PauseEvent.Dispose();
					PauseEvent = null;
				}
				if (RunEvent != null)
				{
					RunEvent.Close();
					RunEvent.Dispose();
					RunEvent = null;
				}
				if (UnityFactory != null)
				{
					UnityFactory.Container.Dispose();
					UnityFactory = null;
				}
			}

			disposed = true;
		}
		#endregion

		#region Config Helpers
		public class Config
		{
			public Task Task { get; protected set;}

			public Config(Task task)
			{
				Task = task;
			}

			public virtual Result<Config> Load()
			{
				var result = Result<Config>.Success();

				result.Entity = this;

				return result;
			}

			public virtual void Save()
			{ }
		}

		/// <summary>
		/// Responsible for creating/loading the Config instance for this task.  Override to instantiate a custom Config class.
		/// </summary>
		/// <returns></returns>
		protected virtual Result<Config> LoadConfig()
		{
			return new Config(this).Load();
		}

		#endregion

		#region Log Helpers
		public string LogMessage(string message, params object[] parms)
		{
			return ServiceProfile.LogMessage(ExternalSystem.Id, EventLogEntryType.Information, string.Empty, string.Format("{0} task {1}", FullName, message), parms);
		}
		public string LogWarning(string message, params object[] parms)
		{
			return ServiceProfile.LogMessage(ExternalSystem.Id, EventLogEntryType.Warning, string.Empty, string.Format("Warning: {0} task {1}", FullName, message), parms);
		}
		public string LogError(string message, params object[] parms)
		{
			return ServiceProfile.LogMessage(ExternalSystem.Id, EventLogEntryType.Error, string.Empty, string.Format("Error: {0} task {1}", FullName, message), parms);
		}
		public string LogException(Exception e, string message, params object[] parms)
		{
			return ServiceProfile.LogException(ExternalSystem.Id, e, string.Format("Exception: {0} task {1}", FullName, message), parms);
		}

		public string LogMessageDetail(string detail, string message, params object[] parms)
		{
			return ServiceProfile.LogMessageDetail(ExternalSystem.Id, EventLogEntryType.Information, detail, string.Format("{0} task {1}", FullName, message), parms);
		}
		public string LogWarningDetail(string detail, string message, params object[] parms)
		{
			return ServiceProfile.LogMessageDetail(ExternalSystem.Id, EventLogEntryType.Warning, detail, string.Format("Warning: {0} task {1}", FullName, message), parms);
		}
		public string LogErrorDetail(string detail, string message, params object[] parms)
		{
			return ServiceProfile.LogMessageDetail(ExternalSystem.Id, EventLogEntryType.Error, detail, string.Format("Error: {0} task {1}", FullName, message), parms);
		}

		#endregion
	}
}
