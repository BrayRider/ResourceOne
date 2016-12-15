using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.Xml.Linq;
using System.IO;

namespace RSM.Integration.S2.Stub
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRestService" in both code and config file together.
	[ServiceContract]
	public interface IRestService
	{
		//[WebInvoke(Method = "POST", UriTemplate = "test?APIcommand={xml}",

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "test")]
			//RequestFormat = WebMessageFormat.Xml,
			//ResponseFormat = WebMessageFormat.Xml,
			//BodyStyle = WebMessageBodyStyle.Bare)]
		XElement Command(Stream request);
	}
}
