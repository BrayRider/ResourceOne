using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Service.Library.Model;

namespace RSM.Service.Library.Interfaces
{
	public interface IAuthentication
	{
		Result<UserSession> Login(string username, string password, string uri = null);

		void Logoff(UserSession session);
	}
}
