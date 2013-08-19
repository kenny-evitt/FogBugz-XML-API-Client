using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FogBugzClient
{
    public static class FogBugzClient
    {
        static string _token;
        static string _url;

        public static void Logon(string url, string userNameOrEmail, string password)
        {
            _url = url;

            XElement logonResult = SubmitCommand("logon", new Dictionary<string, string> { { "email", userNameOrEmail }, { "password", password } }, null);

            _token = logonResult.Element("token").Value;
        }

        public static XElement SubmitCommand(string command, IDictionary<string, string> arguments = null, IEnumerable<IDictionary<string, byte[]>> attachments = null)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();

            arguments.Add("cmd", command);

            if (command != "logon")
            {
                if (String.IsNullOrEmpty(_token))
                    throw new Exception("Not logged in to FogBugz; no token is available.");
                else
                    arguments.Add("token", _token);
            }

            return XElement.Parse(CallRestApi(arguments, attachments));
        }

        private static string CallRestApi(IEnumerable<KeyValuePair<string, string>> arguments, IEnumerable<IDictionary<string, byte[]>> attachments)
        {
            const string newLine = "\r\n";
            const string bounds = "--------------------------------";
            const string bounds2 = "--" + bounds;

            var encoding = new ASCIIEncoding();
            var utf8Encoding = new UTF8Encoding();
            var http = (HttpWebRequest)WebRequest.Create(_url);
            http.Method = "POST";
            http.AllowWriteStreamBuffering = true;
            http.ContentType = "multipart/form-data; boundary=" + bounds;

            var parts = new Queue();

            foreach (var argument in arguments)
            {
                parts.Enqueue(encoding.GetBytes(bounds2 + newLine));
                parts.Enqueue(encoding.GetBytes("Content-Type: text/plain; charset=\"utf-8\"" + newLine));
                parts.Enqueue(encoding.GetBytes(String.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", argument.Key, newLine)));
                parts.Enqueue(utf8Encoding.GetBytes(argument.Value));
                parts.Enqueue(encoding.GetBytes(newLine));
            }

            if (attachments != null)
            {
                foreach (Dictionary<string, byte[]> attachment in attachments)
                {
                    parts.Enqueue(encoding.GetBytes(bounds2 + newLine));
                    parts.Enqueue(encoding.GetBytes("Content-Disposition: form-data; name=\""));
                    parts.Enqueue(attachment["name"]);
                    parts.Enqueue(encoding.GetBytes("\"; filename=\""));
                    parts.Enqueue(attachment["filename"]);
                    parts.Enqueue(encoding.GetBytes("\"" + newLine));
                    parts.Enqueue(encoding.GetBytes("Content-Transfer-Encoding: base64" + newLine));
                    parts.Enqueue(encoding.GetBytes("Content-Type: "));
                    parts.Enqueue(attachment["contenttype"]);
                    parts.Enqueue(encoding.GetBytes(newLine + newLine));
                    parts.Enqueue(attachment["data"]);
                    parts.Enqueue(encoding.GetBytes(newLine));
                }
            }

            parts.Enqueue(encoding.GetBytes(bounds2 + "--"));

            var nContentLength = parts.Cast<byte[]>().Sum(part => part.Length);
            http.ContentLength = nContentLength;

            var stream = http.GetRequestStream();

            foreach (Byte[] part in parts)
                stream.Write(part, 0, part.Length);

            stream.Close();

            var r = http.GetResponse().GetResponseStream();

            if (r != null)
            {
                var reader = new StreamReader(r);
                var retValue = reader.ReadToEnd();
                reader.Close();
                return retValue;
            }

            throw new Exception("HTTP response stream is null.");
        }
    }
}
