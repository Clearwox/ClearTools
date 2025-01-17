using Clear;
using System;

namespace Clear.Tools
{
    public static class Common
    {
        public static string GetAllExceptionMessage(Exception ex)
        {
            string msg = ex.Message;
            if (ex.InnerException != null)
                msg += string.Format(" \n[INNER: {0}]", GetAllExceptionMessage(ex.InnerException));
            return msg;
        }

        public static string GetShareLink(string url, Sharers sharer, string description, string imageUrl)
        {
            switch (sharer)
            {
                case Sharers.Facebook:
                    return string.Format("https://www.facebook.com/sharer/sharer.php?u={0}", url);
                case Sharers.Twitter:
                    return string.Format("https://twitter.com/home?status=Check%20out%20this%20article:%20{1}%20-%20{0}", url, description);
                case Sharers.Pinterest:
                    return string.Format("https://pinterest.com/pin/create/button/?url={0}&amp;media={1}&amp;description={2}", url, imageUrl, description);
                case Sharers.Google:
                    return string.Format("https://plus.google.com/share?url={0}", url);
                default:
                    return string.Empty;
            }
        }
    }
}