using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Web;
using System.Security.Cryptography;
using System.IO;


namespace ProxyView.Model
{
    public class ProxyUpdate
    {
        private List<string> m_urls = new List<string>();
        private List<string> m_usernames = new List<string>();
        private List<string> m_passwords = new List<string>();
        private List<string> m_tokens = new List<string>();
        private List<string> m_fake = new List<string>();

        //添加RSA的公钥以及私钥
        private const string publiKeyFileName = "RSA.Pub";
        private const string privateKeyFileName = "RSA.Private";
        private const int RSAKeySize = 512;
        private const string dirProxyPathName = "D:\\data\\TestData\\proxy\\";
        private const string dirLogPathName = "D:\\data\\TestData\\log\\";
        private const string RSApath = "D:\\Projects\\C#\\ProxyView\\TestData\\RSA\\";

        public void ProcessRequest()
        {
            string user = "user1";
            //用户身份判断后，应从数据库读取服务url，此处代替数据库
            m_urls = new List<string>();
            m_usernames = new List<string>();
            m_passwords = new List<string>();
            m_tokens = new List<string>();

            m_urls.Add("http://localhost:6080/arcgis/rest/services/Wuhan/WHadm/MapServer");
            m_usernames.Add("");
            m_passwords.Add("");
            m_tokens.Add("");
            m_urls.Add("http://localhost:6080/arcgis/rest/services/Wuhan/WHroad/MapServer");
            m_usernames.Add("");
            m_passwords.Add("");
            m_tokens.Add("");
            /*
                m_urls.Add("https://services7.arcgis.com/w7PjH9AjCWXYUyuB/arcgis/rest/services/wuhan/FeatureServer");
                m_usernames.Add("yxs1995");
                m_passwords.Add("zhydsb1995712");
                m_tokens.Add("https://yxs1995.com/portal/sharing/generateToken");
            m_urls.Add("https://services7.arcgis.com/w7PjH9AjCWXYUyuB/arcgis/rest/services/wuhan_road/FeatureServer");
                m_usernames.Add("yxs1995");
                m_passwords.Add("zhydsb1995712");
                m_tokens.Add("https://yxs1995.com/portal/sharing/generateToken");
            */
            //生成用户专属proxy.config
            m_fake = new List<string>();
            userProxy(user, 1);
        }        

        public void userProxy(string user, int encrypt_mode)
        {
            //加载节点
            XmlDocument proxy_doc = new XmlDocument();
            //声明部分
            XmlDeclaration xml_sm = proxy_doc.CreateXmlDeclaration("1.0", "utf-8", null);
            proxy_doc.AppendChild(xml_sm);
            //根节点部分
            XmlElement proxy_config = proxy_doc.CreateElement("", "ProxyConfig", "");
            proxy_config.SetAttribute("allowedReferers", "*");
            proxy_config.SetAttribute("mustMatch", "true");
            proxy_config.SetAttribute("logFile", dirProxyPathName + user + "_proxylog.txt");
            proxy_doc.AppendChild(proxy_config);
            //子节点部分
            XmlElement server_urls = proxy_doc.CreateElement("", "serverUrls", "");
            //日志部分
            string log_fileName = dirLogPathName + user + "_log.config";
            XmlDocument log_doc = new XmlDocument();
            if (File.Exists(log_fileName))
            {
                log_doc.Load(log_fileName);
            }
            else
            {
                XmlDeclaration xml_sm1 = log_doc.CreateXmlDeclaration("1.0", "utf-8", null);
                log_doc.AppendChild(xml_sm1);
                XmlElement log_root = log_doc.CreateElement("", "Logs", "");
                XmlElement log_urls = log_doc.CreateElement("", "serverUrls", "");
                log_doc.AppendChild(log_root);
                log_root.AppendChild(log_urls);
            }
            XmlElement log_root_element = log_doc.DocumentElement;
            XmlElement urls_log_list = (XmlElement)log_root_element.GetElementsByTagName("serverUrls")[0];
            for (int i = 0; i < m_urls.Count; i++)
            {
                XmlElement proxy_child_url = (XmlElement)proxy_doc.CreateElement("", "serverUrl", "");
                string url = m_urls[i];
                string username = m_usernames[i];
                string password = m_passwords[i];
                string token = m_tokens[i];
                string encrypt = null;
                if (encrypt_mode == 1) encrypt = this.AESEncypt(user, url.Replace(':', '.'));
                if (encrypt_mode == 2)
                {
                    encrypt = this.RSAEncrypt(user, url.Replace(':', '.'), RSApath + publiKeyFileName);
                }
                encrypt = encrypt.Replace('/', 'a');
                encrypt = encrypt.Replace('=', 'a');
                encrypt = encrypt.Replace('+', 'a');
                encrypt = encrypt.Replace('-', 'a');
                encrypt = encrypt.Replace('*', 'a');
                //string fake_url = "http://" + "www.wlsp.org.cn" + encrypt;
                string fake_url = "http://" + encrypt + i.ToString();
                proxy_child_url.SetAttribute("url", fake_url);
                proxy_child_url.SetAttribute("matchAll", "true");
                proxy_child_url.SetAttribute("hostRedirect", url);
                //log
                int log_element = this.getElementFormUrl(urls_log_list, url);
                //添加时间
                string now_datetime = DateTime.Now.ToString();
                if (log_element >= 0)
                {
                    XmlElement log_element_target = (XmlElement)urls_log_list.GetElementsByTagName("serverUrl")[log_element];
                    XmlElement fake_log_url = log_doc.CreateElement("", "fakeUrl", "");
                    fake_log_url.SetAttribute("url", fake_url);
                    fake_log_url.SetAttribute("datetime", now_datetime);
                    log_element_target.AppendChild(fake_log_url);
                    log_doc.Save(log_fileName);
                }
                else
                {
                    //先新建url节点
                    XmlElement url_node = log_doc.CreateElement("", "serverUrl", "");
                    url_node.SetAttribute("url", url);
                    XmlElement fake_log_url = log_doc.CreateElement("", "fakeUrl", "");
                    fake_log_url.SetAttribute("url", fake_url);
                    fake_log_url.SetAttribute("datetime", now_datetime);
                    url_node.AppendChild(fake_log_url);
                    urls_log_list.AppendChild(url_node);
                    log_doc.Save(log_fileName);
                }
                //返回
                m_fake.Add(fake_url);
                if (username != "")
                {
                    proxy_child_url.SetAttribute("username", username);
                }
                if (password != "")
                {
                    proxy_child_url.SetAttribute("password", password);
                }
                if (token != "")
                {
                    proxy_child_url.SetAttribute("tokenServiceUri", token);
                }
                server_urls.AppendChild(proxy_child_url);
            }
            proxy_config.AppendChild(server_urls);
            proxy_doc.Save(dirProxyPathName + user + "_proxy.config");
            log_doc.Save(log_fileName );
        }

        //RSA加密
        public string RSAEncrypt(string userName, string url, string pathToPublicKey)
        {
            //获取时间戳
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            //加密文本
            int ts_allSeconds = (int)ts.TotalSeconds;
            string plain_text = ts_allSeconds.ToString() + userName;
            //加载公钥
            var rsa = new RSACryptoServiceProvider(RSAKeySize);
            var publicKey = File.ReadAllText(pathToPublicKey);
            rsa.FromXmlString(publicKey);
            var bytesToEncrypt = System.Text.Encoding.Unicode.GetBytes(plain_text);
            var bytesEncrypted = rsa.Encrypt(bytesToEncrypt, false);
            return Convert.ToBase64String(bytesEncrypted);
        }

        private int getElementFormUrl(XmlElement url_list, string url)
        {
            XmlNodeList node_list = url_list.GetElementsByTagName("serverUrl");
            XmlElement temp_element = null;
            for (int i = 0; i < node_list.Count; i++)
            {
                if (((XmlElement)node_list[i]).GetAttribute("url") == url)
                {
                    temp_element = (XmlElement)node_list[i];
                    return i;
                }
            }
            return -1;
        }

        //AES加密
        public string AESEncypt(string userName, string Url)
        {

            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            //加密文本
            int ts_allSeconds = (int)ts.TotalSeconds;
            string plain_text = ts_allSeconds.ToString() + userName;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(plain_text);
            byte[] keyBytes = new byte[32];

            string[] urls = Url.Split('/');

            Url = Url.Replace(".", "/");
            Url = Url.Replace("_", "/");


            if (Url.Length % 4 != 0)
            {
                for (; ; )
                {
                    if (Url.Length % 4 == 0) break;
                    Url += "0";
                }
            }
            byte[] urlArray = Convert.FromBase64String(Url);
            int len = urlArray.Length;
            if (len > keyBytes.Length) len = keyBytes.Length;
            System.Array.Copy(urlArray, keyBytes, len);

            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rm.CreateEncryptor();
            byte[] resultA = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultA, 0, resultA.Length);
        }

    }
}
