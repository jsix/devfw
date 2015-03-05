using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AtNet.DevFw.Framework.TaskBox.HttpSync
{
    /// <summary>  
    /// HTTP��������  
    /// </summary>  
    public class HttpSimpleRequest
    {
        private static readonly string DefaultUserAgent =
            "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        /// <summary>  
        /// ����GET��ʽ��HTTP����  
        /// </summary>  
        /// <param name="url">�����URL</param>  
        /// <param name="timeout">����ĳ�ʱʱ��</param>  
        /// <param name="userAgent">����Ŀͻ����������Ϣ������Ϊ��</param>  
        /// <param name="cookies">��ͬHTTP�����͵�Cookie��Ϣ���������Ҫ������֤����Ϊ��</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreateGetHttpResponse(
            string url,
            string userAgent,
            CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }

            //if (timeout.HasValue)
            //{
            //    request.Timeout = timeout.Value;
            //}


            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }


        /// <summary>  
        /// ����POST��ʽ��HTTP����  
        /// </summary>  
        /// <param name="url">�����URL</param>  
        /// <param name="parameters">��ͬ����POST�Ĳ������Ƽ�����ֵ�ֵ�</param>  
        /// <param name="timeout">����ĳ�ʱʱ��</param>  
        /// <param name="userAgent">����Ŀͻ����������Ϣ������Ϊ��</param>  
        /// <param name="requestEncoding">����HTTP����ʱ���õı���</param>  
        /// <param name="cookies">��ͬHTTP�����͵�Cookie��Ϣ���������Ҫ������֤����Ϊ��</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreatePostHttpResponse(
            string url,
            Hashtable parameters,
            string userAgent,
            Encoding requestEncoding,
            CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //����Ƿ���HTTPS����  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }

            //if (timeout.HasValue)
            //{
            //    request.Timeout = timeout.Value;
            //}

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //�����ҪPOST����  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = requestEncoding.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        private static bool CheckValidationResult(object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors errors)
        {
            //���ǽ���  
            return true;
        }
    }
}