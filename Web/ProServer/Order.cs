using Microsoft.CSharp;
using ProInterface;
using ProInterface.Models;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProServer
{
    /// <summary>  
    /// 代码执行类  
    /// </summary>  
    public partial class Service : IOrder
    {
        public bool OrderDelete(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_ORDER.SingleOrDefault(a => a.ID == keyId);
                    if (ent.YL_ORDER_INSURE != null)
                    {
                        foreach (var t in ent.YL_ORDER_INSURE.YL_ORDER_INSURE_PRODUCT.ToList())
                        {
                            db.YL_ORDER_INSURE_PRODUCT.Remove(t);
                        }
                        db.YL_ORDER_INSURE.Remove(ent.YL_ORDER_INSURE);
                    }
                    foreach (var t in ent.YL_ORDER_FLOW.ToList())
                    {
                        db.YL_ORDER_FLOW.Remove(t);
                    }
                    if (ent.YL_ORDER_RESCUE != null)
                    {
                        db.YL_ORDER_RESCUE.Remove(ent.YL_ORDER_RESCUE);
                    }
                    db.YL_ORDER.Remove(ent);
                    db.SaveChanges();
                    TaskDelete(loginKey, ref err, keyId);
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Delete);
                    return true;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetExceptionMessage(e);
                    return false;
                }
            }
        }

        public YlOrder OrderSingleId(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_ORDER.SingleOrDefault(x => x.ID == keyId);
                if (ent == null)
                {
                    return null;
                }
                var taskFlow = FunTask.TaskSingle(loginKey, ref err, 0, ent.ID);
                if(err.IsError)
                {
                    return null;
                }

                YlOrder reEnt = Fun.ClassToCopy<YL_ORDER, YlOrder>(ent);

                reEnt.TaskId = taskFlow.ID;
                reEnt.TaskFlowId = taskFlow.NowFlowId;
                reEnt.AllFlow = Fun.ClassListToCopy<TTaskFlow, ProInterface.Models.YL_ORDER_FLOW>(taskFlow.AllFlow);

                reEnt.Car = CarSingleId(loginKey,ref err,ent.CAR_ID);

                

                reEnt.Client = Fun.ClassToCopy<YL_CLIENT, YlClient>(ent.YL_CLIENT);
                reEnt.Client = Fun.ClassToCopy<YL_USER, YlClient>( ent.YL_CLIENT.YL_USER, reEnt.Client);

               

                reEnt.NextButton = taskFlow.AllButton;


                reEnt.AllFiles = Fun.ClassListToCopy<YL_FILES, ProInterface.Models.FILES>(db.YL_FILES.Where(x=>x.YL_TASK_FLOW_HANDLE.Where(y=>y.TASK_FLOW_ID== reEnt.ID).Count()>0).ToList());
                reEnt.ClientName = ent.YL_CLIENT.YL_USER.NAME;
                reEnt.ClientPhone = ent.YL_CLIENT.YL_USER.LOGIN_NAME;
                if (string.IsNullOrEmpty(ent.ORDER_NO))
                {
                    string idStr = "0000" + ent.ID;
                    idStr = idStr.Substring(idStr.Length - 4);
                    reEnt.ORDER_NO = ent.CREATE_TIME.ToString("yyyyMMddHHmm") + idStr;
                }
                reEnt.CarPlateNumber = ent.YL_CAR.PLATE_NUMBER;
                reEnt.LastStatus = "新建";
                var allFlow = ent.YL_ORDER_FLOW.OrderByDescending(x => x.LEVEL_ID).ToList();
                if (allFlow.Count() > 0)
                {
                    reEnt.COST = allFlow.Sum(x => x.COST);
                    reEnt.LastStatus = allFlow[0].SUBJECT;
                }

                return reEnt;
            }
        }

        /// <summary>
        /// 添加定单
        /// </summary>
        /// <param name="err"></param>
        /// <param name="db"></param>
        /// <param name="inBean"></param>
        /// <param name="UserIdArrStr"></param>
        /// <param name="allPar"></param>
        /// <returns></returns>
        public YL_ORDER OrderSaveFun(GlobalUser gu, ref ErrorInfo err, DBEntities db, YlOrder inBean, IList<string> allPar)
        {
            YL_ORDER ent = db.YL_ORDER.SingleOrDefault(x => x.ID == inBean.ID);
            if (ent == null)
            {
                ent = Fun.ClassToCopy<YlOrder, YL_ORDER>(inBean);
                ent.ID = Fun.GetSeqID<YL_ORDER>();
                string idStr = "0000" + ent.ID;
                idStr = idStr.Substring(idStr.Length - 4);
                ent.ORDER_NO = DateTime.Now.ToString("yyyyMMddHHmm") + idStr;
                ent.ORDER_TYPE = inBean.ORDER_TYPE;
                ent.PAY_STATUS = "待核价";
                ent.PAY_STATUS_TIME = DateTime.Now;
                ent.CREATE_TIME = DateTime.Now;
                ent.STATUS = "新建";
                ent.STATUS_TIME = DateTime.Now;
                var car = db.YL_CAR.SingleOrDefault(x => x.ID == inBean.CAR_ID);
                if (car == null)
                {
                    var carList = db.YL_CLIENT.Single(x => x.ID == inBean.CLIENT_ID).YL_CAR.ToList();
                    if (carList.Count() > 0)
                    {
                        ent.CAR_ID = carList.OrderBy(x => x.IS_DEFAULT).ToList()[0].ID;
                    }
                    else {
                        err.IsError = true;
                        err.Message = "没有设置车辆，请先添加车辆信息";
                        return null;
                    }
                }

                int flowId = 0;
                switch (inBean.ORDER_TYPE)
                {
                    case "救援":
                        flowId = 2;
                        break;
                    case "维修":
                        flowId = 3;
                        break;
                    case "保养":
                        flowId = 3;
                        break;
                    case "投保":
                        flowId = 1;
                        break;
                    case "审车":
                        flowId = 4;
                        break;
                }

                TNode inEnt = new TNode();
                inEnt.FlowID = flowId;
                inEnt.TaskName = string.Format("{0}工单", car.PLATE_NUMBER);
                inEnt.AllFilesStr = JSON.DecodeToStr(inBean.AllFiles);
                inEnt.UserIdArrStr = inBean.UserIdArrStr;
                inEnt.Remark = "提交";
                inEnt.TaskKey = null;
                string submit = null;
                switch(flowId)
                {
                    case 1:
                        if (gu.RoleID.Contains(3))
                        {
                            submit = "转算单员";
                        }
                        else
                        {
                            submit = "提交";
                        }
                        break;
                    case 2:
                        if (!string.IsNullOrEmpty(inEnt.UserIdArrStr))
                        {
                            submit = "抢单";
                        }
                        break;
                    case 3:
                        if (!string.IsNullOrEmpty(inEnt.UserIdArrStr))
                        {
                            submit = "抢单";
                        }
                        break;
                }

                var taskFlow = FunTask.StartTask(db, ref err, gu, inEnt, submit);

                if (err.IsError)
                {
                    return null;
                }
                ent.ID = taskFlow.ID;
                db.YL_ORDER.Add(ent);
            }
            else
            {
                ent = Fun.ClassToCopy(inBean, ent, allPar);
            }

            return ent;
        }

    }
}
