using System;

namespace ShortUrl
{
    /// <summary>
    /// Container for the ShortURL object
    /// </summary>
    public class Container
    {
        private string _real_url;
        private string _short_url;
        private DateTime _create_date;
        private string _created_by;

        public Container()
        {
            this.CreateDate = DateTime.Now;
            this.CreatedBy = "Unknown";
            this.RealUrl = null;
            this.ShortenedUrl = "Unknown";
        }

        public string RealUrl
        {
            get
            {
                return _real_url;
            }
            set
            {
                _real_url = value;
            }
        }

        public string ShortenedUrl
        {
            get
            {
                return _short_url;
            }
            set
            {
                _short_url = value;
            }
        }

        public DateTime CreateDate
        {
            get
            {
                return _create_date;
            }
            set
            {
                _create_date = value;
            }
        }

        public string CreatedBy
        {
            get
            {
                return _created_by;
            }
            set
            {
                _created_by = value;
            }
        }
    }
}
