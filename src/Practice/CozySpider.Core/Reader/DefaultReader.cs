﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace CozySpider.Core.Reader
{
    public class DefaultReader : IUrlReader
    {
        public DefaultReader()
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, c, ch, ssl) => true;
        }

        public string Read(string url)
        {
            using (var rspStream = ReadData(url))
            {
                using (StreamReader reader = new StreamReader(rspStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public Stream ReadData(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.Trim());
            request.AllowAutoRedirect = true;
            request.UserAgent = @"Mozilla/5.2 (Windows NT 6.1) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.122 Safari/534.30";
            request.Accept = @"*/*";
            request.Timeout = 3000;

            WebResponse respone = request.GetResponse();
            return respone.GetResponseStream();
        }
    }
}
