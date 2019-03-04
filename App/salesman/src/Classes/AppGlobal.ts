import { Storage } from "@ionic/storage"
import { SalesmanUserBean } from "./SalesmanUserBean";

/**
 * AppGlobal 全局定义 单例模式
 */
export class AppGlobal {
  private static instance: AppGlobal;
  private static _storage: Storage
  /**是否是调试状态 */
  isDebug: boolean = true;

  /**分页页数 */
  pageSize: number = 10;

  public static user: any;

  constructor(
  ) {
  }
  
  public static SetStorage(storage: Storage)
  {
    this._storage=storage;
  }
  /**
   * 获取当前实例
   *
   * @static
   * @returns {AppGlobal}
   */
  public static getInstance(): AppGlobal {
    return AppGlobal.instance;
  };

  public static setUser(ent) {
    this.user = ent;
    console.log('保存用户')
    console.log(ent)
    this._storage.set("user", ent);

    let value: any = JSON.stringify(ent);
    localStorage.setItem("LSuser", value);
  };

  public static getUserAuthToken() {
    if (this.user == null) return null;
    return this.user.authToken;
  }
  public static getUser() {
    if (this.user == null) return new SalesmanUserBean();
    return this.user
  }
  public static LoadUser(callBack) {
    this._storage.ready().then(() => {
      this._storage.get('user').then((val) => {
        if (val != null) {
          console.log('读取用户')
          console.log(val)
          this.user = val;
          callBack(val);
        }
        else {
          callBack(null);
        }
      })
    })
  }
  public static read<T>(): T {
    let value: string = localStorage.getItem("LSuser");

    if (value && value != "undefined" && value != "null") {
      return <T>JSON.parse(value);
    }
    return null;
  }

  public static removeUser() {
    this._storage.remove("user");
    localStorage.removeItem("LSuser");
  }
}
