import {UserBean} from "./User";
/**
 * Created by wengzhilai on 2017/1/18.
 */
export class SalesmanUserBean extends UserBean {
  public allOrder:string[] = [];
  public expireInsureList:string[] = [];
  public  REQUEST_CODE:string="";
}
