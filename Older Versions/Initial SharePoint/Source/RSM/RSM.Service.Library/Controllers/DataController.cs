using System;

using RSMDataModelDataContext = RSM.Support.RSMDataModelDataContext;

namespace RSM.Service.Library.Controllers
{
    /// <summary>
    /// Base Data Controller
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataController<T>: IDisposable
        where T : class, new()
    {
        public RSMDataModelDataContext DbContext { get; set; }

        public DataController()
        {
            DbContext = new RSMDataModelDataContext();
        }

        public DataController(RSMDataModelDataContext context)
        {
            DbContext = context;
        }

		#region IDisposable
		private bool disposed = false; // to detect redundant calls
		~DataController()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool freeManaged)
		{
			if (disposed)
				return;

			if (freeManaged)
			{
				if (DbContext != null)
				{
					DbContext.Dispose();
					DbContext = null;
				}
			}

			disposed = true;
		}
		#endregion
	}
}
