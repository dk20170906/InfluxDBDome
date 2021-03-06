﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace testinfluxdb
{
    public class InfluxDBHelper
    {
        static string _baseAddress;  //地址
        static string _username;  //用户名
        static string _password;  //密码
        static string _database;  //数据库名

        /// <summary>
        /// 构造函数
        /// </summary>
        public InfluxDBHelper(string baseAddress,string username,string password,string database)
        {

            _baseAddress = baseAddress;
            _username = username;
            _password = password;
            _database = database;
        }

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public  string Query(string sql)
        {
            string pathAndQuery = string.Format("query?pretty=true&db={0}&epoch=u&q={1}", _database, sql);
            string url = _baseAddress + pathAndQuery;

            string result = HttpHelperGet(url);
            return result;
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string Write(string paramStr)
        {
            string pathAndQuery = string.Format("write?db={0}", _database);
            string url = _baseAddress + pathAndQuery;

            string result = HttpHelperPost(url, paramStr);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HttpHelperGet(string uri)
        {
            try
            {
                string result = string.Empty;
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(uri);
                if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                {
                    wbRequest.Credentials = GetCredentialCache(uri, _username, _password);
                    wbRequest.Headers.Add(_username, _password);
                }
                wbRequest.Method = "GET";
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream))
                    {
                        result = sReader.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="paramStr"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HttpHelperPost(string uri, string paramStr)
        {
            try
            {
                string result = string.Empty;
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(uri);
                wbRequest.Proxy = null;
                wbRequest.Method = "POST";
                wbRequest.ContentType = "application/x-www-form-urlencoded";
                wbRequest.ContentLength = Encoding.UTF8.GetByteCount(paramStr);
                if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                {
                    wbRequest.Credentials = GetCredentialCache(uri, _username, _password);
                    wbRequest.Headers.Add(_username, _password);
                }
                byte[] data = Encoding.ASCII.GetBytes(paramStr);
                using (Stream requestStream = wbRequest.GetRequestStream())
                {
                    using (StreamWriter swrite = new StreamWriter(requestStream))
                    {
                        swrite.Write(paramStr);
                    }
                }
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sread = new StreamReader(responseStream))
                    {
                        result = sread.ReadToEnd();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static CredentialCache GetCredentialCache(string uri, string username, string password)
        {
            string authorization = string.Format("{0}:{1}", username, password);
            CredentialCache credCache = new CredentialCache();
            credCache.Add(new Uri(uri), "Basic", new NetworkCredential(username, password));
            return credCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string GetAuthorization(string username, string password)
        {
            string authorization = string.Format("{0}:{1}", username, password);
            return "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(authorization));
        }

    }
}
