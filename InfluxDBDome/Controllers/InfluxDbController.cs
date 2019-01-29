using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using testinfluxdb;

namespace InfluxDBDome.Controllers
{
    public class InfluxDbController : Controller
    {

        public ActionResult Index ()
        {
            return View();
        }
        [DllImport("kernel32.dll")]
        public static extern int WinExec (string exeName, int operType);

        #region 服务


        /// <summary>
        /// 启动服务 
        /// </summary>
        /// <param name="influxdUrl"></param>
        /// <returns></returns>
        public JsonResult OpenInfluxdDbService (string influxdUrl)
        {
            //   E:\InfluxDb\InfluxDb\influxdb - 1.6.2 - 1

            try
            {
                StopInfluxDbService();
                influxdUrl = @"E:/InfluxDBDome/InfluxDBDome/Influxdb/influxd.exe";
                // influxdUrl = System.IO.Directory.GetCurrentDirectory() + "/influxdb-1.6.2-1/influxd.exe";
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = influxdUrl;
                info.Arguments = "";
                info.WindowStyle = ProcessWindowStyle.Minimized;
                Process pro = Process.Start(info);
                pro.WaitForExit();
                return Json(new
                {
                    success = true,
                    message = "Influxd.exe服务启动成功"
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Influxd.exe服务启动失败" + ex.Message
                });
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public JsonResult StopInfluxDbService ()
        {
            try
            {
                Process[] allProgresse = System.Diagnostics.Process.GetProcessesByName("Influxd.exe");
                foreach (Process closeProgress in allProgresse)
                {
                    if (closeProgress.ProcessName.Equals("Influxd.exe"))
                    {
                        closeProgress.Kill();
                        closeProgress.WaitForExit();
                        break;
                    }
                }
                return Json(new
                {
                    success = true,
                    message = "停止influxd服务成功"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "停止influxd服务失败" + ex.Message
                });

            }
        }
        #endregion

        #region 客户端
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public JsonResult CreateDatabase (string url, string dbName)
        {
            try
            {
                url = "http://192.168.3.122:8086/query";
                dbName = "testInfluxdb";
                string pathAndQuery = string.Format("q=CREATE DATABASE {0}", dbName);
                var result = InfluxDBHelper.HttpHelperPost(url, pathAndQuery);
                return Json(new
                {
                    success = true,
                    message = "创建数据库成功",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "创建数据库失败" + ex.Message,
                });
            }
        }

        public JsonResult DropDatabase (string url, string dbName)
        {
            try
            {
                url = "http://192.168.3.122:8086/query";
                dbName = "testInfluxdb";
                string pathAndQuery = string.Format("q=DROP DATABASE {0}", dbName);
                var result = InfluxDBHelper.HttpHelperPost(url, pathAndQuery);
                return Json(new
                {
                    success = true,
                    message = "删除数据库成功",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "删除数据库失败" + ex.Message,
                });
            }
        }

        /// <summary>
        ///插入数据 1条或多条
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public JsonResult InsertDataDome (string dbName)
        {
            try
            {
                var url = "http://192.168.3.122:8086/";
                var username = "admin";
                var password = "admin";
                dbName = "testInfluxdb";
                InfluxDBHelper influxDBHelper = new InfluxDBHelper(url, username, password, dbName);
                string sql = "cpu_load_short,host=server02 value=0.67 ";    //插入一条

                #region 插入多条
                //  List<string> list = new List<string>();
                //  list.Add("test code=code002,mac=aaa");    //requestsQueued为指标的名称
                //  list.Add("test code=code003,mac=aaa");
                //  list.Add("test code=code004,mac=aaa");
                //var   sqlpaem = string.Join("\n", list);
                //  sql = sql + "\n" + sqlpaem;

                #endregion

                var str = influxDBHelper.Write(sql);

                // 'http://localhost:8086/write?db=mydb'--data - binary 'cpu_load_short,host=server01,region=us-west value=0.64 1434055562000000000'
                var mmm = "cpu_load_short,host=server01,region=us-west value=0.64 1434055562000000000";

                var mkkk = InfluxDBHelper.HttpHelperPost("http://localhost:8086/write?db=testInfluxdb", mmm);

                return Json(new
                {
                    success = true,
                    message = "插入数据成功",
                    data = str
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "插入数据失败" + ex.Message,
                });

            }

        }


        public JsonResult GetDataDome ()
        {
            try
            {
                var url = "http://192.168.3.122:8086/";
                var username = "admin";
                var password = "admin";
                InfluxDBHelper influxDBHelper = new InfluxDBHelper(url, username, password, "testInfluxdb");
                string sql = "select * from cpu_load_short";
                //string sql = "select * from test;select * from test";        多条


                var str = influxDBHelper.Query(sql);
                return Json(new
                {
                    success = true,
                    message = "读取数据成功",
                    data = str
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "读取数据失败" + ex.Message,
                });

            }

        }


        public JsonResult DeleteDataDome ()
        {
            try
            {
                var url = "http://192.168.3.122:8086/";
                var username = "admin";
                var password = "admin";
                InfluxDBHelper influxDBHelper = new InfluxDBHelper(url, username, password, "testInfluxdb");
                string sql = "delete from cpu_load_short where time=1537261373570440200; ";

                var str = influxDBHelper.Query(sql);
                return Json(new
                {
                    success = true,
                    message = "删除数据成功",
                    data = str
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "删除数据失败" + ex.Message,
                });

            }
        }



        #endregion


    }
}