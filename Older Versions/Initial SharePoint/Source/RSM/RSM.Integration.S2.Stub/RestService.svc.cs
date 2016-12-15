using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
//using System.Xml;
using System.Xml.Linq;
using System.ServiceModel.Channels;
using System.Diagnostics;

using RSM.Support;
using RSM.Integration.S2.Stub.Data;
using RSM.Integration.S2.Stub.Handlers;

namespace RSM.Integration.S2.Stub
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RestService" in code, svc and config file together.
	public class RestService : IRestService
	{
		public XElement Command(Stream input)
        {
			return DoCommand<Handler>(input);
        }

		private XElement DoCommand<T>(Stream input)
			where T: ICommands, new()
		{
			try
			{
				var ts = new TraceSource("Messages");

				var handler = new T();

				var reader = new StreamReader(input);
				var text = reader.ReadToEnd();

				ts.TraceInformation(text);

				var prefix = "APIcommand=";
				if (text.StartsWith(prefix))
					text = text.Substring(prefix.Length);

				var request = XDocument.Parse(text);
				var node = request.Element("NETBOX-API").Element("COMMAND");
				var cmd = node.Attribute("name").Value;
				var num = node.Attribute("num").Value;

				XElement root = null;
				if (cmd.Equals("Login", System.StringComparison.InvariantCultureIgnoreCase))
				{
					root = handler.Login(request);
				}
				else if (cmd.Equals("Logout", System.StringComparison.InvariantCultureIgnoreCase))
				{
					root = handler.Logout(request);
				}
				else if (cmd.Equals("GetAccessHistory", System.StringComparison.InvariantCultureIgnoreCase))
				{
					root = handler.GetAccessHistory(request);
				}
				else if (cmd.Equals("GetPerson", System.StringComparison.InvariantCultureIgnoreCase))
				{
					root = handler.GetPerson(request);
				}
				else if (cmd.Equals("GetPortals", System.StringComparison.InvariantCultureIgnoreCase))
				{
					root = handler.GetPortals(request);
				}
				else if (cmd.Equals("GetReader", System.StringComparison.InvariantCultureIgnoreCase))
				{
					root = handler.GetReader(request);
				}
				else
					throw new ApplicationException(string.Format("This command is not supported: {0}", cmd));

				root = root ?? Handler.NewResponse().Root;
				var elem = root.Element("RESPONSE");
				elem.SetAttributeValue("command", cmd);
				elem.SetAttributeValue("num", num);

				ts.TraceInformation(root.ToString());

				return root;
			}
			catch (Exception e)
			{
				Trace.TraceError("Exception encountered: {0}", e.ToString());
				return Handler.SetCode("FAIL", msg: e.ToString()).Root;
			}
		}
    }
}
