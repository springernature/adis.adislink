<%@ WebService Language="C#" Class="API" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

[WebService(Namespace = "http://link.adisinsight.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class API  : System.Web.Services.WebService {

    [WebMethod]
    public ShortUrl.Container CreateUrl(string real_url)
    {
        ShortUrl.Container oShortUrl = new ShortUrl.Container();

        oShortUrl.RealUrl = real_url;
        
        oShortUrl.ShortenedUrl = ShortUrl.Utils.CheckIfUrlExists(oShortUrl.RealUrl);

        if (oShortUrl.ShortenedUrl == null || (String)oShortUrl.ShortenedUrl == String.Empty)
        {
	        oShortUrl.ShortenedUrl = ShortUrl.Utils.UniqueShortUrl();
	        oShortUrl.CreateDate = DateTime.Now;
	        oShortUrl.CreatedBy = HttpContext.Current.Request.UserHostAddress;
	
	        ShortUrl.Utils.AddUrlToDatabase(oShortUrl);
	
        }
        oShortUrl.ShortenedUrl = ShortUrl.Utils.PublicShortUrl(oShortUrl.ShortenedUrl);
        return oShortUrl;
    }

    [WebMethod]
    public ShortUrl.Container GetUrl(string short_url)
    {
        short_url = ShortUrl.Utils.InternalShortUrl(short_url);
        return ShortUrl.Utils.RetrieveUrlFromDatabase(short_url);
    }
    
}

