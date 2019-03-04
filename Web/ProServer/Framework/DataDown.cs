
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using ProInterface.Models;
using ProInterface;
using System.Linq.Expressions;
using LINQExtensions;
using System.Data;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

namespace ProServer
{
    public partial class Service : IDataDown
    {
        public bool DataDownEventStart(string loginKey, ref ErrorInfo err, int downID)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            GlobalUser gu=Global.GetUser(loginKey);

            using (DBEntities db = new DBEntities())
            {

                var down = db.YL_DATA_DOWN.SingleOrDefault(x => x.ID == downID);


                // string s = GetSelectScript(down.SELECT_SCRIPT);
                //获得相对应的参数
                //GetMonthPath(ref belongMonth, ref path, down.CREATE_TYPE, down.CREATE_TABLE_NAME);

                string tableName = Fun.ReplaceDataTime(down.CREATE_TABLE_NAME, DateTime.Now, loginKey);

                var eventEnt = new YL_DATA_DOWN_EVENT();
                eventEnt.ID = Fun.GetSeqID<YL_DATA_DOWN_EVENT>();
                eventEnt.TARGET_NAME = tableName;
                eventEnt.ALL_NUM = 0;
                eventEnt.LAST_MONTH_NUM = 0;
                eventEnt.PATH = down.TO_PATH;
                eventEnt.DATA_DOWN_ID = downID;
                eventEnt.START_TIME = DateTime.Now;
                eventEnt.USER_ID = gu.UserId;
                db.YL_DATA_DOWN_EVENT.Add(eventEnt);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(dbEx);
                    return false;
                }


                string createScript = "";
                if (!string.IsNullOrEmpty(down.CREATE_SCRIPT))
                {
                    createScript = down.CREATE_SCRIPT.Replace("{@TABLE_NAME}", eventEnt.TARGET_NAME);
                }

                foreach (var to in down.YL_DATA_DOWN_TO.ToList())
                {
                    var toServer = to.YL_DB_SERVER;
                    #region 在目标服务器上创建表
                    try
                    {
                        try
                        {
                            DbServerNonQuery(toServer.ID, "drop table " + eventEnt.TARGET_NAME);
                        }
                        catch (Exception e)
                        {

                        }
                        DbServerNonQuery(toServer.ID, createScript);
                    }
                    catch (Exception e)
                    {
                        err.IsError = true;
                        err.Message = "在服务器【" + toServer.NICKNAME + "】上建表失败：\r\nSQL:" + createScript + "\r\n" + e.Message;
                        return false;
                    }
                    #endregion
                    foreach (var from in down.YL_DB_SERVER.ToList())
                    {

                        YL_DATA_DOWN_TASK task = new YL_DATA_DOWN_TASK();
                        task.ID = Fun.GetSeqID<YL_DATA_DOWN_TASK>();
                        task.NAME = eventEnt.TARGET_NAME;
                        task.SELECT_SCRIPT = down.SELECT_SCRIPT;
                        task.EVENT_ID = eventEnt.ID;
                        #region 替换SELECT_SCRIPT
                        foreach (var replace in JSON.EncodeToEntity<IList<KV>>(to.REPLACE_STR))
                        {
                            task.SELECT_SCRIPT = task.SELECT_SCRIPT.Replace(replace.K, replace.V);
                        }
                        task.SELECT_SCRIPT = Fun.ReplaceDataTime(task.SELECT_SCRIPT, DateTime.Now, loginKey);
                        #endregion

                        #region 生成@[00-99]
                        if (task.SELECT_SCRIPT.IndexOf("@") != -1)
                        {
                            task.SELECT_SCRIPT =Fun.GetSelectScript(task.SELECT_SCRIPT);
                        }
                        #endregion

                        //设置存放路径
                        task.TO_PATH = Fun.ReplaceDataTime(down.TO_PATH, DateTime.Now, loginKey);

                        int thisAllNum = 0;

                        task.ALL_NUM = thisAllNum;
                        int upMonthNum = 0;

                        task.LAST_MONTH_NUM = upMonthNum;
                        task.SELECT_DB_SERVER = from.ID;
                        task.SELECT_SERVER = string.Format("(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVER=DEDICATED)(SID={2})))", from.IP, from.PORT, from.DBNAME);
                        task.SELECT_UID = from.UID;
                        task.SELECT_PWD = from.PASSWORD;
                        task.EVENT_TYPE = 1;
                        if (down.SUCC_SCRIPT != null)
                        {
                            task.SUCC_SCRIPT = down.SUCC_SCRIPT.Replace("{@TABLE_NAME}", eventEnt.TARGET_NAME);
                        }
                        task.TO_DB_SERVER = toServer.ID;
                        task.TO_SERVER = toServer.DBNAME;
                        task.TO_UID = toServer.UID;
                        task.TO_PWD = toServer.PASSWORD;
                        task.CREATE_SCRIPT = createScript;
                        task.ERROR_NUM = 0;
                        task.STATUS = "等待";
                        task.PAGE_SIZE = down.PAGE_SIZE;
                        task.SPLIT_STR = down.SPLIT_STR;
                        task.IS_CANCEL = 0;
                        task.ORDER_NUM = task.ID;
                        db.YL_DATA_DOWN_TASK.Add(task);
                    }
                }
                db.SaveChanges();
                return true;
            }
        }


        public DataDown DataDownSingleId(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_DATA_DOWN.SingleOrDefault(x => x.ID == keyId);
                var reEnt = new ProInterface.Models.DataDown();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_DATA_DOWN, ProInterface.Models.DataDown>(ent);
                    reEnt.FormServerStr = string.Join(",", ent.YL_DB_SERVER.Select(x => x.ID));
                    reEnt.ToServerStr = JSON.DecodeToStr(Fun.ClassListToCopy<YL_DATA_DOWN_TO, ProInterface.Models.DATA_DOWN_TO>(ent.YL_DATA_DOWN_TO.ToList()));
                }
                return reEnt;
            }
        }


        public IList<TableFiled> DataDowGetSqlFilde(ref ErrorInfo err, string sql, int serverID)
        {
            if (sql.ToLower().IndexOf(" where ") > 0)
            {
                sql = sql + " and rownum<1 ";
            }
            else
            {
                sql = sql + " where rownum<1 ";
            }
            DataTable dt = new DataTable();
            try
            {
                dt = DbServerDataTable(serverID, sql);
            }
            catch (Exception error)
            {
                err.IsError = true;
                err.Message = error.Message;
                err.Excep = error;
                return null;
            }
            if (dt == null) return null;
            IList<ProInterface.Models.TableFiled> nowCfgList = new List<ProInterface.Models.TableFiled>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var t = dt.Columns[i];
                nowCfgList.Add(new ProInterface.Models.TableFiled() { Code = t.ColumnName, CSharpType = t.DataType.FullName });
            }
            return nowCfgList;
        }

        public bool DataDownSave(string loginKey, ref ErrorInfo err, ProInterface.Models.DataDown inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_DATA_DOWN.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        isAdd = true;
                        ent = Fun.ClassToCopy<ProInterface.Models.DataDown, YL_DATA_DOWN>(inEnt);
                    }
                    else
                    {
                        ent = Fun.ClassToCopy<ProInterface.Models.DataDown, YL_DATA_DOWN>(inEnt, ent, allPar);
                    }

                    var allFormDbServer = inEnt.FormServerStr.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                    ent.YL_DB_SERVER.Clear();
                    ent.YL_DB_SERVER = db.YL_DB_SERVER.Where(x => allFormDbServer.Contains(x.ID)).ToList();

                    var allToDbServer = JSON.EncodeToEntity<IList<YL_DATA_DOWN_TO>>(inEnt.ToServerStr);

                    foreach (var t in ent.YL_DATA_DOWN_TO.ToList())
                    {
                        if (allToDbServer.SingleOrDefault(x => x.DB_SERVER_ID == t.DB_SERVER_ID) == null)
                        {
                            db.YL_DATA_DOWN_TO.Remove(t);
                        }
                    }

                    foreach (var t in allToDbServer)
                    {
                        var tmp = ent.YL_DATA_DOWN_TO.SingleOrDefault(x => x.DB_SERVER_ID == t.DB_SERVER_ID);
                        if (tmp == null)
                        {
                            ent.YL_DATA_DOWN_TO.Add(new YL_DATA_DOWN_TO { DATA_DOWN_ID = t.DATA_DOWN_ID, DB_SERVER_ID = t.DB_SERVER_ID, REPLACE_STR = t.REPLACE_STR, TO_SERVER_NAME = t.TO_SERVER_NAME });
                        }
                        else
                        {
                            tmp.TO_SERVER_NAME = t.TO_SERVER_NAME;
                            tmp.REPLACE_STR = t.REPLACE_STR;
                        }
                    }


                    if (isAdd)
                    {
                        db.YL_DATA_DOWN.Add(ent);
                    }
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = e.Message;
                    return false;
                }
            }
        }

               
        

    }
}
