/**
 * Created by wengzhilai on 2017/1/12.
 */
import {Headers, Http,Response } from '@angular/http';
import {CarIn} from "../Classes/CarIn";
import 'rxjs/add/operator/toPromise';
import {Injectable} from '@angular/core';
import {CommonService} from "./Common.Service";
import {AppGlobal} from "../Classes/AppGlobal";

@Injectable()
export class ToPostService {
  private headers = new Headers({'Content-Type': 'application/json'});

  constructor(private http:Http,
              private commonService:CommonService) {

  }

  list(apiName:string, postBean, callback) {
    postBean.authToken = AppGlobal.getUserAuthToken();
    postBean.userId = 0;
    if (postBean.pageSize == null || postBean.pageSize == 0) {
      postBean.pageSize = CarIn.pageSize;
    }
    if (postBean.currentPage == null || postBean.currentPage == 0) {
      postBean.currentPage = 1;
    }
    this.Post(apiName, postBean, callback)
  }

  ListFun(apiName:string, postBean) {
    postBean.authToken = AppGlobal.getUserAuthToken();
    postBean.userId = 0;
    if (postBean.pageSize == null || postBean.pageSize == 0) {
      postBean.pageSize = CarIn.pageSize;
    }
    if (postBean.currentPage == null || postBean.currentPage == 0) {
      postBean.currentPage = 1;
    }
    return this.PostFun(apiName, postBean)
  }

  SaveOrUpdate(apiName, bean, callback) {
    var saveKeyStr = this.commonService.getBeanNameStr(bean);
    if (saveKeyStr == "") {
      this.commonService.hint("保存参数saveKeys不能为空");
      return;
    }
    var postBean = {
      authToken: AppGlobal.getUserAuthToken(),
      saveKeys: saveKeyStr,
      entity: bean
    };
    this.Post(apiName, postBean, callback)
  }

  SaveOrUpdateFun(apiName, bean) {
    var saveKeyStr = this.commonService.getBeanNameStr(bean);
    if (saveKeyStr == "") {
      this.commonService.hint("保存参数saveKeys不能为空");
      return;
    }
    var postBean = {
      authToken: AppGlobal.getUserAuthToken(),
      saveKeys: saveKeyStr,
      entity: bean
    };
    return this.PostFun(apiName, postBean);
  }



  Post(apiName, postBean, callback) {
    this.commonService.showLoading();
    console.log("请求[" + apiName + "]参数：");
    console.log(postBean);
    return this.http
      .post(CarIn.api + apiName, JSON.stringify(postBean), {headers: this.headers})
      .toPromise()
      .then((res:Response) => {
        this.commonService.hideLoading();
        var response = res.json();
        console.log("返回结果：");
        console.log(response);
        if (response['IsError']) {
          // this.commonService.showError(response);
        }
        if (callback) {
          console.log("回调方法：" + callback);
          callback(response);
        }
      })
      .catch(this.handleError);
  }
  PostFun(apiName, postBean) {
    this.commonService.showLoading();
    console.log("请求[" + apiName + "]参数：");
    console.log(postBean);
    return this.http
      .post(CarIn.api + apiName, JSON.stringify(postBean), {headers: this.headers})
      .toPromise()
      .then((res:Response) => {
        this.commonService.hideLoading();
        var response = res.json();
        console.log("返回结果：");
        console.log(response);
        if (response['IsError']) {
          if (response.Message == '登录超时') {
            // this.navCtrl.push(UserLoginPage)
          }
          else {
            // this.commonService.showError(response);
          }
        }
        return response;
      })
      .catch(this.handleError);
  }

  single(apiName, id, callback) {
    if (id == null && id == 0) {
      alert("查询id不能为空");
      return;
    }
    var postBean = {
      userId: 0,
      authToken: AppGlobal.getUserAuthToken(),
      id: id
    };
    this.Post(apiName, postBean, callback)
  }

  SingleFun(apiName, id) {
    if (id == null && id == 0) {
      alert("查询id不能为空");
      return;
    }
    var postBean = {
      userId: 0,
      authToken: AppGlobal.getUserAuthToken(),
      id: id
    };
    return this.PostFun(apiName, postBean)
  }

  handleError(error:any):Promise<any> {
    console.error('An error occurred', error); // for demo purposes only
    return Promise.reject(error.message || error);
  }
}
