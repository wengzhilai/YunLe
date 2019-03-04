using Microsoft.CSharp;
using ProInterface;
using ProInterface.Models;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProServer
{
    /// <summary>  
    /// 代码执行类  
    /// </summary>  
    public partial class Service : IOrderFlow
    {
        

        public bool OrderFlowSave(string loginKey, ref ErrorInfo err, ProInterface.Models.YL_ORDER_FLOW inEnt, IList<string> allPar)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时，请重新登录";
                return false;
            }

            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {

                var ent = db.YL_ORDER_FLOW.SingleOrDefault(a => a.ORDER_FLOW_NO == inEnt.ORDER_FLOW_NO);
                var order = db.YL_ORDER.SingleOrDefault(x => x.ID == inEnt.ORDER_ID);
                if (order == null)
                {
                    err.IsError = true;
                    err.Message = "订单ID不存在";
                    return false;
                }

                var taskFlow = db.YL_TASK_FLOW.SingleOrDefault(x => x.ID == order.ID);
                if (taskFlow != null)
                {
                    order.STATUS = taskFlow.YL_TASK.STATUS;
                    order.STATUS_TIME = taskFlow.YL_TASK.STATUS_TIME;
                }


                var maxLevel = order.YL_ORDER_FLOW.Max(x => x.LEVEL_ID);
                if (maxLevel == null)
                {
                    maxLevel = 0;
                }
                maxLevel++;
                bool isAdd = false;
                if (ent == null)
                {
                    isAdd = true;
                    ent = Fun.ClassToCopy<ProInterface.Models.YL_ORDER_FLOW, YL_ORDER_FLOW>(inEnt);
                    ent.ORDER_FLOW_NO = order.ORDER_NO + maxLevel;
                    ent.LEVEL_ID = maxLevel;
                    ent.STATUS_TIME = DateTime.Now;
                    //if (ent.COST > 0)
                    //{
                    //    order.PAY_STATUS = "待支付";
                    //    order.PAY_STATUS_TIME = DateTime.Now;
                    //}
                    //if (inEnt.SUBJECT == "完成")
                    //{
                    //    if (order.YL_ORDER_FLOW.Sum(x => x.COST) == 0 && ent.COST == 0)
                    //    {
                    //        order.PAY_STATUS = "完成";
                    //        order.PAY_STATUS_TIME = DateTime.Now;
                    //    }
                    //}
                }
                else
                {
                    ent = Fun.ClassToCopy<ProInterface.Models.YL_ORDER_FLOW, YL_ORDER_FLOW>(inEnt, ent, allPar);
                }

                if (isAdd)
                {

                    db.YL_ORDER_FLOW.Add(ent);
                }
                try {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = Fun.GetDbEntityErrMess(e);
                    return false;
                }
                //SmsSendOrder(ent.YL_ORDER.YL_CLIENT.YL_USER.LOGIN_NAME, ent.YL_ORDER.ORDER_NO, string.Format("于{0}被{1}", DateTime.Now.ToString(), ent.SUBJECT));
                UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
                return true;

            }
        }
        
        public ProInterface.Models.YL_ORDER_FLOW OrderFlowSingleId(string loginKey, ref ErrorInfo err, string keyId, int orderId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_ORDER_FLOW.SingleOrDefault(x => x.ORDER_FLOW_NO == keyId);
                var order= db.YL_ORDER.SingleOrDefault(x => x.ID == orderId);
                if (ent != null)
                {
                    order = ent.YL_ORDER;
                }
                
                var reEnt = new ProInterface.Models.YL_ORDER_FLOW();
                reEnt = Fun.ClassToCopy<YL_ORDER, ProInterface.Models.YL_ORDER_FLOW>(order);

                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_ORDER_FLOW, ProInterface.Models.YL_ORDER_FLOW>(ent, reEnt);
                    
                }
                else
                {
                    reEnt.ORDER_ID = orderId;
                }
                return reEnt;
            }
        }


        public ProInterface.Models.YL_ORDER_FLOW OrderFlowGetWaitPay(string loginKey, ref ErrorInfo err, int orderId)
        {
            using (DBEntities db = new DBEntities())
            {
                ProInterface.Models.YL_ORDER_FLOW reEnt = new ProInterface.Models.YL_ORDER_FLOW();
                var flowAll = db.YL_ORDER_FLOW.Where(x => x.ORDER_ID == orderId && x.STATUS== "待支付").OrderBy(x=>x.LEVEL_ID).ToList();
                if (flowAll.Count() > 0)
                {
                    reEnt = Fun.ClassToCopy<YL_ORDER_FLOW, ProInterface.Models.YL_ORDER_FLOW>(flowAll[0]);
                    return reEnt;
                }
                else
                {
                    err.IsError = true;
                    err.Message = "该订单已经支付，无需再进行支付";
                    return null;
                }
            }
        }

        public ProInterface.Models.YL_ORDER_FLOW OrderFlowSavePay(string loginKey, ref ErrorInfo err, string out_trade_no, string cash_fee, string transaction_id,string openid)
        {
            using (DBEntities db = new DBEntities())
            {
                ProInterface.Models.YL_ORDER_FLOW reEnt = new ProInterface.Models.YL_ORDER_FLOW();
                var flowAll = db.YL_ORDER_FLOW.SingleOrDefault(x =>x.ORDER_FLOW_NO== out_trade_no);
                if (flowAll!=null)
                {
                    flowAll.STATUS = "已支付";
                    flowAll.STATUS_TIME = DateTime.Now;
                    flowAll.OUT_TRADE_NO = transaction_id;
                    flowAll.SELLER_ID = openid;
                    flowAll.YL_ORDER.PAY_STATUS = "已支付";
                    flowAll.YL_ORDER.PAY_STATUS_TIME = flowAll.STATUS_TIME;
                    reEnt = Fun.ClassToCopy<YL_ORDER_FLOW, ProInterface.Models.YL_ORDER_FLOW>(flowAll);
                    db.SaveChanges();

                    //if (flowAll.YL_ORDER.YL_ORDER_RESCUE != null && flowAll.YL_ORDER.YL_ORDER_RESCUE.YL_GARAGE != null)
                    //{
                    //    var allUser = flowAll.YL_ORDER.YL_ORDER_RESCUE.YL_GARAGE.YL_USER.ToList();
                    //    foreach (var t in allUser)
                    //    {
                    //        SmsSendOrder(loginKey,ref err,t.LOGIN_NAME, flowAll.YL_ORDER.ORDER_NO, string.Format("用户已付款{0}", flowAll.COST),t.ID, flowAll.ORDER_ID.Value,"1");
                    //    }
                    //}


                    return reEnt;
                }
                else
                {
                    err.IsError = true;
                    err.Message = "订单不存在";
                    return null;
                }
            }
        }

    }
}
