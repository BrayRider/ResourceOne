using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace RSM.Integration.S2.Stub
{
	public interface ICommands
	{
		XElement Login(XDocument request);
		XElement Logout(XDocument request);
		XElement GetAccessHistory(XDocument request);
		XElement GetPerson(XDocument request);
		XElement GetPortals(XDocument request);
		XElement GetReader(XDocument request);
	}
}
