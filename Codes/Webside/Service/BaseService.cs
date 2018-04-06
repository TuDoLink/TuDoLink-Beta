using NPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace TudolinkWeb.Service
{
    public class BaseService
    {
        private DbConnection connection;

        public BaseService()
        {
            connection = new SQLiteConnection(Config.DataSource);
        }

        /// <summary>
        /// 查询所有对象
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <returns></returns>
        public List<T> Query<T>()
        {
            var list = new List<T>();
            try
            {
                string className = typeof(T).ToString();

                var db = new Database(connection);
                string name = className.Substring(
                    className.LastIndexOf('.') + 1,
                    className.Length - className.LastIndexOf('.') - 1);

                connection.Open();

                list = db.Fetch<T>(string.Format("select * from {0} ", name));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }


        public List<T> Fetch<T>()
        {
            var list = new List<T>();
            try
            {
                string className = typeof(T).ToString();

                var db = new Database(connection);
                string name = className.Substring(
                    className.LastIndexOf('.') + 1,
                    className.Length - className.LastIndexOf('.') - 1);

                connection.Open();

                list = db.Fetch<T>(string.Format("select * from {0} ", name), "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }


        /// <summary>
        /// 删除对象
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="objData"></param>
        /// <returns></returns>
        public int Delete<T>(object objData)
        {
            var result = 0;
            try
            {
                var db = new Database(connection);
                connection.Open();
                result = db.Delete<T>(objData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 添加一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <returns></returns>
        public T Add<T>(object objData)
        {
            T result;
            try
            {
                var db = new Database(connection);
                connection.Open();
                result = (T)Convert.ChangeType(db.Insert(objData), typeof(T));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}