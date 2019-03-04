using Microsoft.CSharp;
using ProInterface;
using ProInterface.Models;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProServer
{
    /// <summary>  
    /// 代码执行类  
    /// </summary>  
    public partial class Service : IOrderInsure
    {
        public int OrderInsureAddFromExcel(string loginKey, ref ErrorInfo err, int excelId)
        {
            GlobalUser gu = Global.GetUser(loginKey);
            if (gu == null)
            {
                err.IsError = true;
                err.Message = "登录超时";
                return 0;
            }
            using (DBEntities db = new DBEntities())
            {
                var file = db.YL_FILES.SingleOrDefault(x => x.ID == excelId);
                DataTable Dt = new DataTable();
                try
                {
                    Dt = ExcelHelper.ImportExcel2007toDt(Fun.UrlToAllPath(file.URL));
                }
                catch
                {
                    Dt = new DataTable();
                }
                if (Dt == null || Dt.Rows.Count == 0)
                {
                    err.IsError = true;
                    err.Message = "Excels格式有问题";
                    return 0;
                }
                //所有列名
                List<string> allCol = new List<string>();
                for (var i = 0; i < Dt.Columns.Count; i++)
                {
                    allCol.Add(Dt.Columns[i].Caption);
                }
                if (!allCol.Contains("业务员姓名"))
                {
                    err.IsError = true;
                    err.Message = string.Format("EXCEL里必须包含【业务员姓名】列");
                    return 0;
                }
                if (!allCol.Contains("保险公司"))
                {
                    err.IsError = true;
                    err.Message = string.Format("EXCEL里必须包含【保险公司】列");
                    return 0;
                }

                if (!allCol.Contains("车牌号"))
                {
                    err.IsError = true;
                    err.Message = string.Format("EXCEL里必须包含【车牌号】列");
                    return 0;
                }

                var allCar = db.YL_CAR.ToList();
                var allUser = db.YL_USER.ToList();
                var allProduct = db.YL_INSURER_PRODUCT.ToList();
                var allInsurer = db.YL_INSURER.ToList();
                int succ_num = 0, add_num = 0;
                for (var i = 0; i < Dt.Rows.Count; i++)
                {
                    YL_SALESMAN salesman = new YL_SALESMAN();
                    YL_CAR nowCar = new YL_CAR();
                    YL_INSURER nowIns = new YL_INSURER();

                    #region 获取业务员   [业务员姓名]
                    
                    var salesmanName = Convert.ToString(Dt.Rows[i]["业务员姓名"]);
                    var salesmanList = db.YL_SALESMAN.Where(x => x.YL_USER.NAME == salesmanName).ToList();
                    if (salesmanList.Count() > 1)
                    {
                        err.IsError = true;
                        err.Message = string.Format("业务员姓名【{0}】有重复", salesmanName);
                        return 0;
                    }
                    if (salesmanList.Count() == 0)
                    {
                        err.IsError = true;
                        err.Message = string.Format("业务员姓名【{0}】不存在", salesmanName);
                        return 0;
                    }
                    salesman = salesmanList[0];

                    #endregion
                    #region 获取车辆   [车牌号]
                    
                    var carNo = Convert.ToString(Dt.Rows[i]["车牌号"]);
                    nowCar = db.YL_CAR.SingleOrDefault(x => x.PLATE_NUMBER == carNo);
                    if (nowCar == null)
                    {
                        nowCar = new YL_CAR();
                        nowCar.ID = Fun.GetSeqID<YL_CAR>();
                        nowCar.PLATE_NUMBER = carNo;
                        db.YL_CAR.Add(nowCar);
                    }

                    #endregion
                    #region 获取保险公司   [保险公司]
                    var insName = Convert.ToString(Dt.Rows[i]["保险公司"]);
                    nowIns = allInsurer.SingleOrDefault(x => x.NAME == insName);
                    if (nowIns == null)
                    {
                        err.IsError = true;
                        err.Message = string.Format("保险公司【{0}】不存在", insName);
                        return 0;
                    }
                    #endregion

                    YL_ORDER order = new YL_ORDER();
                    #region 获取并设置保单   [保单/批单号]
                    if (!allCol.Contains("保单/批单号"))
                    {
                        err.IsError = true;
                        err.Message = string.Format("EXCEL里必须包含【保单/批单号】列");
                        return 0;
                    }
                    var orderNo = Convert.ToString(Dt.Rows[i]["保单/批单号"]);
                    order = db.YL_ORDER.SingleOrDefault(x => x.ORDER_NO == orderNo);
                    if (order == null)
                    {
                        order = new YL_ORDER();
                        order.ID = Fun.GetSeqID<YL_ORDER>();
                        order.ORDER_NO = orderNo;
                        db.YL_ORDER.Add(order);
                    }
                    order.CAR_ID = nowCar.ID;
                    order.CLIENT_ID = 1;
                    order.ORDER_TYPE = "投保";
                    order.PAY_STATUS = "新生成";
                    order.PAY_STATUS_TIME = DateTime.Now;
                    if (allCol.Contains("签单日期") && Dt.Rows[i]["签单日期"].ToString().IsDateTime())
                    {
                        order.CREATE_TIME = Convert.ToDateTime(Dt.Rows[i]["签单日期"]);
                    }
                    #endregion

                    YL_ORDER_INSURE orderIns = new YL_ORDER_INSURE();
                    orderIns.ID = order.ID;
                    orderIns.INSURER_ID = nowIns.ID;
                    if(allCol.Contains("起保日期") && Dt.Rows[i]["起保日期"] != null && Dt.Rows[i]["起保日期"].ToString().IsDateTime())
                    {
                        orderIns.DATE_START = Convert.ToDateTime(Dt.Rows[i]["起保日期"]);
                    }
                    if (allCol.Contains("终保日期") && Dt.Rows[i]["终保日期"] != null && Dt.Rows[i]["终保日期"].ToString().IsDateTime())
                    {
                        orderIns.DATE_END = Convert.ToDateTime(Dt.Rows[i]["终保日期"]);
                    }

                    if (allCol.Contains("被保险人名称") && Dt.Rows[i]["被保险人名称"]!=null)
                    {
                        orderIns.CAR_USERNAME = Convert.ToString(Dt.Rows[i]["被保险人名称"]);
                    }
                    if (allCol.Contains("投保人名称") && Dt.Rows[i]["投保人名称"] != null)
                    {
                        orderIns.PROPOSER_NAME = Convert.ToString(Dt.Rows[i]["投保人名称"]);
                        orderIns.BUY_USERNAME = Convert.ToString(Dt.Rows[i]["投保人名称"]);
                    }

                    //if (allCol.Contains("结算标志") && Dt.Rows[i]["结算标志"] != null)
                    //{
                    //    orderIns.STATUS = Convert.ToString(Dt.Rows[i]["结算标志"]);
                    //}
                    //if (allCol.Contains("结算时间") && Dt.Rows[i]["结算时间"]!=null && Dt.Rows[i]["结算时间"].ToString().IsDateTime())
                    //{
                    //    orderIns.STATUS_TIME = Convert.ToDateTime(Dt.Rows[i]["结算时间"]);
                    //    orderIns.SETTLE_TIME = orderIns.STATUS_TIME;
                    //}

                    if (allCol.Contains("险种名称") && Dt.Rows[i]["险种名称"] != null)
                    {
                        orderIns.INS_NAME = Convert.ToString(Dt.Rows[i]["险种名称"]);
                    }
                    if (allCol.Contains("出单员姓名") && Dt.Rows[i]["出单员姓名"] != null)
                    {
                        orderIns.MAKE_ORDER_USERNAME = Convert.ToString(Dt.Rows[i]["出单员姓名"]);
                    }

                    if (allCol.Contains("业务性质") && Dt.Rows[i]["业务性质"] != null)
                    {
                        orderIns.BUSINESS_NATURE = Convert.ToString(Dt.Rows[i]["业务性质"]);
                    }

                    if (allCol.Contains("车辆种类") && Dt.Rows[i]["车辆种类"] != null)
                    {
                        orderIns.CAR_TYPE = Convert.ToString(Dt.Rows[i]["车辆种类"]);
                    }

                    if (allCol.Contains("结算单号") && Dt.Rows[i]["结算单号"] != null)
                    {
                        orderIns.ACCOUNT_NO = Convert.ToString(Dt.Rows[i]["结算单号"]);
                    }
                    if (allCol.Contains("产品") && Dt.Rows[i]["产品"] != null)
                    {
                        orderIns.PRODUCT_NAME = Convert.ToString(Dt.Rows[i]["产品"]);
                    }
                    if (allCol.Contains("业务归属人") && Dt.Rows[i]["业务归属人"] != null)
                    {
                        orderIns.BUSINE_USER = Convert.ToString(Dt.Rows[i]["业务归属人"]);
                    }
                    if (allCol.Contains("费用比例") && Dt.Rows[i]["费用比例"] != null && Dt.Rows[i]["费用比例"].ToString().IsDouble())
                    {
                        orderIns.COST_RATIO = Convert.ToDecimal(Dt.Rows[i]["费用比例"]);
                    }
                    if (allCol.Contains("已结算手续费") && Dt.Rows[i]["已结算手续费"] != null && Dt.Rows[i]["已结算手续费"].ToString().IsDouble())
                    {
                        orderIns.SUCC_FEE = Convert.ToDecimal(Dt.Rows[i]["已结算手续费"]);
                    }

                    if (allCol.Contains("渠道代码") && Dt.Rows[i]["渠道代码"] != null)
                    {
                        orderIns.CHANNEL_CODE = Convert.ToString(Dt.Rows[i]["渠道代码"]);
                    }
                    if (allCol.Contains("归属机构") && Dt.Rows[i]["归属机构"] != null)
                    {
                        orderIns.BE_GROUP = Convert.ToString(Dt.Rows[i]["归属机构"]);
                    }
                    if (allCol.Contains("出单机构") && Dt.Rows[i]["出单机构"] != null)
                    {
                        orderIns.MAKE_GROUP = Convert.ToString(Dt.Rows[i]["出单机构"]);
                    }
                    if (allCol.Contains("经办人") && Dt.Rows[i]["经办人"] != null)
                    {
                        orderIns.OPERATOR_USER = Convert.ToString(Dt.Rows[i]["经办人"]);
                    }

                    if (allCol.Contains("签单保费") && Dt.Rows[i]["签单保费"] != null && Dt.Rows[i]["签单保费"].ToString().IsDouble())
                    {
                        orderIns.ALL_COST = Convert.ToDecimal(Dt.Rows[i]["签单保费"]);
                    }
                    if (allCol.Contains("实收保费") && Dt.Rows[i]["实收保费"] != null && Dt.Rows[i]["实收保费"].ToString().IsDouble())
                    {
                        orderIns.FACT_COST = Convert.ToDecimal(Dt.Rows[i]["实收保费"]);
                    }

                    if (allCol.Contains("应付手续费") && Dt.Rows[i]["应付手续费"] != null && Dt.Rows[i]["应付手续费"].ToString().IsDouble())
                    {
                        orderIns.MEET_COST = Convert.ToDecimal(Dt.Rows[i]["应付手续费"]);
                    }
                    if (allCol.Contains("结算比例") && Dt.Rows[i]["结算比例"] != null && Dt.Rows[i]["结算比例"].ToString().IsDouble())
                    {
                        orderIns.SETTLE_RATIO = Convert.ToDecimal(Dt.Rows[i]["结算比例"]);
                    }
                    if (allCol.Contains("待结算手续费") && Dt.Rows[i]["待结算手续费"] != null && Dt.Rows[i]["待结算手续费"].ToString().IsDouble())
                    {
                        orderIns.WAIT_MEET_COST = Convert.ToDecimal(Dt.Rows[i]["待结算手续费"]);
                    }

                    if (allCol.Contains("验车人") && Dt.Rows[i]["验车人"] != null)
                    {
                        orderIns.DO_PEOPLE = Convert.ToString(Dt.Rows[i]["验车人"]);
                    }
                    db.YL_ORDER_INSURE.Add(orderIns);
                }
                try
                {
                    db.SaveChanges();
                    succ_num++;
                }
                catch (DbEntityValidationException e)
                {
                    err.IsError = true;
                    err.Message = string.Format("成功导入【{0}】条数据,其它新增【{1}】条,第【{2}】行数据有误：{3}", succ_num, add_num, succ_num, Fun.GetDbEntityErrMess(e));
                    return 0;
                }
                catch (Exception e)
                {
                    err.IsError = true;
                    err.Message = string.Format("成功导入【{0}】条数据,其它新增【{1}】条,第【2}】行数据有误：{3}", succ_num, add_num, succ_num, Fun.GetExceptionMessage(e));
                    return 0;
                }

                err.Message = string.Format("成功导入【{0}】条数据,其它新增【{1}】条", succ_num, add_num);
                return succ_num;
            }
        }

        public bool OrderInsureSave(string loginKey, ref ErrorInfo err, ProInterface.Models.YlOrderInsure inEnt, IList<string> allPar)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return false;
            using (DBEntities db = new DBEntities())
            {
                try
                {
                    var ent = db.YL_ORDER_INSURE.SingleOrDefault(a => a.ID == inEnt.ID);
                    bool isAdd = false;
                    if (ent == null)
                    {
                        err.IsError = true;
                        err.Message = "该订单不能在电脑端添加，请在手机上添加";
                        return false;
                    }
                    else
                    {
                        ent.YL_ORDER = Fun.ClassToCopy<ProInterface.Models.YL_ORDER, YL_ORDER>(inEnt, ent.YL_ORDER, allPar);
                        ent = Fun.ClassToCopy<ProInterface.Models.YlOrderInsure, YL_ORDER_INSURE>(inEnt, ent, allPar);
                    }
                    if (!string.IsNullOrEmpty(inEnt.SaveProductIdStr))
                    {
                        inEnt.SaveProductId = JSON.EncodeToEntity<List<YlOrderInsureProduct>>(inEnt.SaveProductIdStr);
                        inEnt.SaveProductId = inEnt.SaveProductId.Where(x => x.isCheck || x.COST!=null).ToList();
                        foreach (var t in inEnt.SaveProductId)
                        {
                            var addEnt = ent.YL_ORDER_INSURE_PRODUCT.SingleOrDefault(x => x.INSURER_ID == t.INSURER_ID && x.PRODUCT_ID == t.PRODUCT_ID);
                            if (addEnt == null)
                            {
                                addEnt = Fun.ClassToCopy<YlOrderInsureProduct, YL_ORDER_INSURE_PRODUCT>(t);
                                ent.YL_ORDER_INSURE_PRODUCT.Add(addEnt);
                            }
                            else
                            {
                                addEnt.MAX_PAY = t.MAX_PAY;
                                addEnt.COST = t.COST;
                            }
                        }

                        foreach (var t in ent.YL_ORDER_INSURE_PRODUCT.ToList())
                        {
                            var addEnt = inEnt.SaveProductId.SingleOrDefault(x => x.INSURER_ID == t.INSURER_ID && x.PRODUCT_ID == t.PRODUCT_ID);
                            if (addEnt == null)
                            {
                                db.YL_ORDER_INSURE_PRODUCT.Remove(t);
                            }
                        }
                    }
                    ent.YL_ORDER.COST = ent.YL_ORDER_INSURE_PRODUCT.Sum(x => x.COST);
                    if (isAdd)
                    {
                        db.YL_ORDER_INSURE.Add(ent);
                    }
                    db.SaveChanges();
                    UserWriteLog(loginKey, MethodBase.GetCurrentMethod(), StatusType.UserLogType.Edit);
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

        public ProInterface.Models.YlOrderInsure OrderInsureSingleId(string loginKey, ref ErrorInfo err, int keyId)
        {
            using (DBEntities db = new DBEntities())
            {
                var order = OrderSingleId(loginKey, ref err, keyId);
                YlOrderInsure reEnt = Fun.ClassToCopy<YlOrder, YlOrderInsure>(order);
                var ent = db.YL_ORDER_INSURE.SingleOrDefault(x => x.ID == keyId);
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_ORDER_INSURE, YlOrderInsure>(ent, reEnt);

                    reEnt = Fun.ClassToCopy<YL_ORDER, YlOrderInsure>(ent.YL_ORDER, reEnt);

                    if (ent.YL_INSURER!=null)
                    {
                        reEnt.Insure = Fun.ClassToCopy<YL_INSURER, YlInsure>(ent.YL_INSURER);
                    }

                    #region 图片
                    if (ent.DRIVER_PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVER_PIC_ID);
                        if (image != null) reEnt.driverPicUrl = image.URL;
                    }
                    if (ent.DRIVING_PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVING_PIC_ID);
                        if (image != null) reEnt.DrivingPicUrl = image.URL;
                    }
                    if (ent.ID_NO_PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC_ID);
                        if (image != null) reEnt.idNoUrl = image.URL;
                    }

                    if (ent.DRIVER_PIC_ID1 != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVER_PIC_ID1);
                        if (image != null) reEnt.driverPicUrl1 = image.URL;
                    }
                    if (ent.DRIVING_PIC_ID1 != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVING_PIC_ID1);
                        if (image != null) reEnt.DrivingPicUrl1 = image.URL;
                    }
                    if (ent.ID_NO_PIC_ID1 != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC_ID1);
                        if (image != null) reEnt.idNoUrl1 = image.URL;
                    }

                    if (ent.RECOGNIZEE_PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.RECOGNIZEE_PIC_ID);
                        if (image != null) reEnt.recognizeePicUrl = image.URL;
                    }

                    if (ent.RECOGNIZEE_PIC_ID1 != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.RECOGNIZEE_PIC_ID1);
                        if (image != null) reEnt.recognizeePicUrl1 = image.URL;
                    }
                    #endregion

                    var nowAllPrice = ent.YL_ORDER_INSURE_PRODUCT.ToList();
                    var allInsuer = nowAllPrice.Select(x => x.INSURER_ID).ToList();

                    #region 计算选项
                    foreach (var t in db.YL_INSURER.OrderBy(x => x.ID).ToList())
                    {
                        var tmp = Fun.ClassToCopy<YL_INSURER, YlInsure>(t);
                        foreach (var x in db.YL_INSURER_PRODUCT.Where(x=>x.PARENT_ID==null).ToList())
                        {
                            var product = Fun.ClassToCopy<YL_INSURER_PRODUCT, YlInsureProduct>(x);
                            var nowProduct = nowAllPrice.SingleOrDefault(y => y.INSURER_ID == t.ID && y.PRODUCT_ID == x.ID);
                            product.isCheck = false;
                            if (nowProduct != null)
                            {
                                product.isCheck = true;
                                product.Price = nowProduct.COST;
                                product.maxPay = nowProduct.MAX_PAY;
                            }
                            else {
                                product.NAME = x.NAME;
                                product.InsureName = t.NAME;
                            }

                            foreach (var child in x.YL_INSURER_PRODUCT1.ToList())
                            {
                                var Childproduct = Fun.ClassToCopy<YL_INSURER_PRODUCT, YlInsureProduct>(child);
                                var ChildNowProduct = nowAllPrice.SingleOrDefault(y => y.INSURER_ID == t.ID && y.PRODUCT_ID == child.ID);
                                Childproduct.isCheck = false;
                                if (ChildNowProduct != null)
                                {
                                    Childproduct.isCheck = true;
                                    Childproduct.Price = ChildNowProduct.COST;
                                    Childproduct.maxPay = ChildNowProduct.MAX_PAY;
                                }
                                Childproduct.NAME = child.NAME;
                                Childproduct.InsureName = t.NAME;
                                product.ChildItem.Add(Childproduct);
                            }

                            product.InsureName = t.NAME;
                            tmp.AllProductPrice.Add(product);
                            
                        }
                        if (t.ID == ent.INSURER_ID)
                        {
                            foreach (var a in tmp.AllProductPrice)
                            {
                                var saveId = Fun.ClassToCopy<YlInsureProduct, YlOrderInsureProduct>(a);
                                saveId.InsurerName = t.NAME;
                                if (a.PARENT_ID == null)
                                {
                                    saveId.InsurerName += "主险";
                                }
                                else
                                {
                                    saveId.InsurerName += "附加险";
                                }
                                saveId.InsurerProductName = a.NAME;
                                saveId.INSURE_ID = ent.ID;
                                saveId.INSURER_ID = t.ID;
                                saveId.PRODUCT_ID = a.ID;
                                saveId.MAX_PAY = a.maxPay;
                                saveId.COST = a.Price;
                                foreach (var a0 in a.ChildItem)
                                {
                                    var saveId1 = Fun.ClassToCopy<YlInsureProduct, YlOrderInsureProduct>(a0);
                                    saveId1.InsurerName = t.NAME;
                                    if (a0.PARENT_ID == null)
                                    {
                                        saveId1.InsurerName += "主险";
                                    }
                                    else
                                    {
                                        saveId1.InsurerName += "附加险";
                                    }
                                    saveId1.PRODUCT_ID = a0.ID;
                                    saveId1.INSURE_ID = ent.ID;
                                    saveId1.INSURER_ID = t.ID;
                                    saveId1.InsurerProductName = a0.NAME;
                                    saveId1.MAX_PAY = a0.maxPay;
                                    saveId1.COST = a0.Price;
                                    reEnt.SaveProductId.Add(saveId1);
                                }
                                reEnt.SaveProductId.Add(saveId);
                            }
                        }
                        reEnt.AllInsurePrice.Add(tmp);

                        reEnt.SaveProductIdStr = JSON.DecodeToStr(reEnt.SaveProductId);
                    } 
                    #endregion
                }
                return reEnt;
            }
        }



    }
}
