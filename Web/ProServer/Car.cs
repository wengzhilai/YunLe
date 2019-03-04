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
    public partial class Service : ICar
    {
        public ProInterface.Models.YL_CAR CarGetAndSave(string loginKey, ref ErrorInfo err, ProInterface.Models.YL_CAR inEnt,int userId)
        {
            ProInterface.Models.YL_CAR reEnt = new ProInterface.Models.YL_CAR();
            using (DBEntities db = new DBEntities())
            {
                if (inEnt==null || inEnt.PLATE_NUMBER==null || inEnt.PLATE_NUMBER.Length != 7)
                {
                    err.IsError = true;
                    err.Message = "车牌格式有误";
                    return null;
                }

                inEnt.PLATE_NUMBER = inEnt.PLATE_NUMBER.ToUpper();
                
                var carList = db.YL_CAR.Where(x => x.PLATE_NUMBER == inEnt.PLATE_NUMBER).ToList();
                YL_CAR car = new YL_CAR();
                YL_CLIENT user = new YL_CLIENT();
                if (carList.Count() == 0)
                {
                    car = Fun.ClassToCopy<ProInterface.Models.YL_CAR, YL_CAR>(inEnt);
                    car.ID = Fun.GetSeqID<YL_CAR>();
                    db.YL_CAR.Add(car);
                    user = db.YL_CLIENT.SingleOrDefault(x => x.ID == userId);
                    user.YL_CAR.Add(car);
                }
                else {
                    car = carList[0];
                    if (car.YL_CLIENT.SingleOrDefault(x => x.ID == userId) == null)
                    {
                        car.YL_CLIENT.Add(db.YL_CLIENT.SingleOrDefault(x => x.ID == userId));
                    }
                }
                db.SaveChanges();
                reEnt = Fun.ClassToCopy<YL_CAR, ProInterface.Models.YL_CAR>(car);
                return reEnt;
            }
        }

        public ProInterface.Models.YlCar CarSingleId(string loginKey, ref ErrorInfo err, int keyId)
        {
            if (!UserCheckFunctioAuthority(loginKey, ref err, MethodBase.GetCurrentMethod())) return null;
            using (DBEntities db = new DBEntities())
            {
                var ent = db.YL_CAR.SingleOrDefault(x => x.ID == keyId);
                var reEnt = new ProInterface.Models.YlCar();
                if (ent != null)
                {
                    reEnt = Fun.ClassToCopy<YL_CAR, ProInterface.Models.YlCar>(ent);
                    if (ent.BILL_PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.BILL_PIC_ID);
                        if (image != null) reEnt.billUrl = image.URL;
                    }
                    if (ent.CERTIFICATE_PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.CERTIFICATE_PIC_ID);
                        if (image != null) reEnt.certificatePicUrl = image.URL;
                    }
                    if (ent.DRIVING_PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVING_PIC_ID);
                        if (image != null) reEnt.DrivingPicUrl = image.URL;
                    }
                    if (ent.DRIVING_PIC_ID1 != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.DRIVING_PIC_ID1);
                        if (image != null) reEnt.DrivingPicUrl1 = image.URL;
                    }
                    if (ent.ID_NO_PIC_ID != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC_ID);
                        if (image != null) reEnt.idNoUrl = image.URL;
                    }
                    if (ent.ID_NO_PIC_ID1 != null)
                    {
                        var image = db.YL_FILES.SingleOrDefault(x => x.ID == ent.ID_NO_PIC_ID1);
                        if (image != null) reEnt.idNoUrl1 = image.URL;
                    }
                }
                return reEnt;
            }
        }
    }
}
