using ProInterface.Models;
using ProInterface.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface
{

   
    public interface IApi
    {

        ProInterface.Models.YL_APP_VERSION CheckUpdate(ref ErrorInfo err, ApiRequesEntityBean inEnt);

        ErrorInfo LoginReg(ref ErrorInfo err, ApiLogingBean inEnt);
        ErrorInfo LoginOut(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ApiClientBean ClientLogin(ref ErrorInfo err, ApiLogingBean inEnt);
        ApiSalesmanBean SalesmanLogin(ref ErrorInfo err, ApiLogingBean inEnt);
        YlSalesman GarageUserLogin(ref ErrorInfo err, ApiLogingBean inEnt);
        ApiPagingDataBean FileList(ref ErrorInfo err, ApiRequesPageBean inEnt);
        ApiFileBean FileUp(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo FileDel(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo SendCode(string loginKey, ref ErrorInfo err, string phone);
        ErrorInfo ResetPassword(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        IList<ApiKeyValueBean> GetAllDistrict(ref ErrorInfo err);


        ApiSalesmanBean SalesmanSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        YlSalesman GarageUserSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ApiClientBean ClientSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ApiClientBean ClientSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiClientBean> inEnt);
        ApiSalesmanBean SalesmanSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiSalesmanBean> inEnt);
        ErrorInfo UserEditPwd(ref ErrorInfo err, ApiRequesSaveEntityBean<string> inEnt);
        ApiSalesmanBean ClientSingleByPhone(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ApiClientBean SalesmanSingleByPhone(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo SalesmanJoinTeam(ref ErrorInfo err, ApiRequesSaveEntityBean<string> inEnt);
        ErrorInfo ClientPeccancy(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ApiPagingDataBean<ApiPeccancyBean> ClientPeccancy1(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo ClientPeccancy2(ref ErrorInfo err, ApiRequesEntityBean inEnt);


        ApiCarBean CarSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiCarBean> inEnt);
        ApiPagingDataBean CarList(ref ErrorInfo err, ApiRequesPageBean inEnt);
        ApiCarBean CarSetDefault(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo CarDelete(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ApiCarBean CarSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt);


        ApiCarBean QueryCar(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        IList<ApiInsureBean> QueryInsure(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiCardBean> inEnt);
        ApiPagingDataBean<ApiInsureProductBean> QueryInsureProduct(ref ErrorInfo err, ApiRequesPageBean inEnt);
        YlOrderInsure OrderInsureSave(ref ErrorInfo err, ApiRequesSaveEntityBean<YlOrderInsure> inEnt);
        YlOrderInsure OrderInsureSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ApiInsuranceExpenseBean QueryInsurePrice(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiCardBean> inEnt);
        ApiPagingDataBean<YlOrderInsure> OrderInsureList(ref ErrorInfo err, ApiRequesPageBean inEnt);
        ApiPagingDataBean<ApiOrderRescueBean> OrderRescueList(ref ErrorInfo err, ApiRequesPageBean inEnt);


        ApiPagingDataBean<ApiGarageBean> RescueQuery(ref ErrorInfo err, ApiRequesPageBean inEnt);
        ApiOrderRescueBean RescueSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiOrderRescueBean> inEnt);
        ApiOrderRescueBean RescueSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ApiGarageBean GarageSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt);

        YlOrder OrderSave(ref ErrorInfo err, ApiRequesSaveEntityBean<YlOrder> inEnt);
        ApiPagingDataBean<ApiOrderBean> OrderList(ref ErrorInfo err, ApiRequesPageBean inEnt);
        YlOrder OrderSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ApiPagingDataBean<ApiOrderBean> OrderGrabList(ref ErrorInfo err, ApiRequesPageBean inEnt);
        ApiOrderRescueBean OrderGrab(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo OrderSaveStatus(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo OrderSaveVital(ref ErrorInfo err, ApiRequesEntityBean inEnt);


        ApiClientBean SalesmanClientAdd(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiClientBean> inEnt);
        ApiPagingDataBean<ApiClientBean> SalesmanClientList(ref ErrorInfo err, ApiRequesPageBean inEnt);
        ErrorInfo SalesmanClientRestPwd(ref ErrorInfo err, ApiRequesEntityBean inEnt);

        

        ApiCardBean CardSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiCardBean> inEnt);
        ApiPagingDataBean CardList(ref ErrorInfo err, ApiRequesPageBean inEnt);
        ApiWithdrawBean WithdrawAdd(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiWithdrawBean> inEnt);
        ApiPagingDataBean WithdrawList(ref ErrorInfo err, ApiRequesPageBean inEnt);
        ApiPagingDataBean CostList(ref ErrorInfo err, ApiRequesPageBean inEnt);


        ApiPagingDataBean MessageGetAll(ref ErrorInfo err, ApiRequesPageBean inEnt);
        ErrorInfo MessageReply(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo MessageSend(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo MessageDelete(ref ErrorInfo err, ApiRequesEntityBean inEnt);


        ApiAddressBean AddressSave(ref ErrorInfo err, ApiRequesSaveEntityBean<ApiAddressBean> inEnt);
        ApiPagingDataBean AddressList(ref ErrorInfo err, ApiRequesPageBean inEnt);
        ErrorInfo AddressSetDefault(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo AddressDelete(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ApiAddressBean AddressSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt);

        ApiWeiXinJSSDKBean WenXinJSSDKSign(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo WeixinSendMsg(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo WeixinGetOpenid(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        YL_WEIXIN_USER WeixinGetUser(ref ErrorInfo err, ApiRequesEntityBean inEnt);

        ApiPagingDataBean<YlTeam> TeamMyAll(ref ErrorInfo err, ApiRequesPageBean inEnt);
        YlTeam TeamSingle(ref ErrorInfo err, ApiRequesPageBean inEnt);


        IList<YL_MESSAGE_TYPE> MessageTypeAll(ref ErrorInfo err, ApiRequesEntityBean inEnt);

        ApiPagingDataBean BbsList(ref ErrorInfo err, ApiRequesPageBean inEnt);
        Ylbbs BbsSingle(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        ErrorInfo BbsGood(ref ErrorInfo err, ApiRequesEntityBean inEnt);
        Ylbbs BbsSave<T>(ref ErrorInfo err, ApiRequesSaveEntityBean<T> inEnt);
        ErrorInfo BbsDelete(ref ErrorInfo err, ApiRequesEntityBean inEnt);
    }
}
