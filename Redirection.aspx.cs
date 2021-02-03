using System;
using System.Web;

public partial class Redirection : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ShortUrl.Container oShortUrl;

        oShortUrl = ShortUrl.Utils.RetrieveUrlFromDatabase(ShortUrl.Utils.InternalShortUrlFromRedirect(Request.Url.ToString()));

        //        Response.Write(ShortUrl.Utils.InternalShortUrl(Request.Url.ToString()));

        //	Response.Write(Request.Url.ToString());
        //	Response.Write(oShortUrl.RealUrl.ToString());

        if (oShortUrl.RealUrl != null)
        {
            Response.Redirect(oShortUrl.RealUrl);
        }
        else
        {
            Response.Redirect("MissingUrl.aspx");
        }
    }
}
