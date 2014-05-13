using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BitTorrentProtocol
{
    /// <summary>
    /// Rough generator of HTTP GET requests
    /// </summary>
    public class UriGenerator
    {
        public String Path { get; set; }
        public String Uri { get; set; }

        public bool IsCorrect
        {
            get
            {
                return CorrectUri(Uri);
            }
        }

        public bool HasParameters
        {
            get
            {
                return Uri.Contains('?');
            }
        }

        public UriGenerator(String path)
        {
            Path = path;
            Uri = path;
        }

        public UriGenerator AddParameter(String parameter_name, byte[] value)
        {
            String escaped_value = HttpUtility.UrlEncode(value);
            return SetParameter(parameter_name, escaped_value);
        }

        public UriGenerator AddParameter(String parameter_name, long value)
        {
            return SetParameter(parameter_name, value.ToString());
        }

        public UriGenerator AddParameter(String parameter_name, String value)
        {
            String escaped_value = HttpUtility.UrlEncode(value);
            return SetParameter(parameter_name, escaped_value);
        }

        protected UriGenerator SetParameter(String parameter_name, String value)
        {
            if (String.IsNullOrEmpty(parameter_name) || String.IsNullOrEmpty(value)) return null;

            Uri = String.Format("{0}{1}{2}={3}", Uri, ParametersSeparator(), parameter_name, value);
            return this;
        }

        protected char ParametersSeparator()
        {
            return HasParameters ? '&' : '?';
        }

        protected bool CorrectUri(String uri)
        {
            return System.Uri.IsWellFormedUriString(uri, System.UriKind.Absolute);
        }
    }
}
