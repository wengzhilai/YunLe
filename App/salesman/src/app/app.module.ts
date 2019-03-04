import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler } from '@angular/core';
import { HttpModule, JsonpModule } from '@angular/http';
import { IonicApp, IonicModule, IonicErrorHandler } from 'ionic-angular';
import { Geolocation } from '@ionic-native/geolocation';
import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';
import { PhotoViewer } from '@ionic-native/photo-viewer';
import { Transfer } from '@ionic-native/transfer';
import { AppVersion } from '@ionic-native/app-version';
import { Camera } from '@ionic-native/camera';
import { FileOpener } from '@ionic-native/file-opener';
import { IonicStorageModule } from '@ionic/storage';
import { ImagePicker } from '@ionic-native/image-picker';
 
import { MyApp } from './app.component';
import { HomePage } from '../pages/home/home';
import { UserLoginPage } from '../pages/user/login/login';
import { UserMenuPage } from '../pages/user/menu/menu';
import {UserEditPage} from "../pages/user/edit/edit";
import {ImgUrlPipe } from "../pipe/imgUrl";
import {DateToStringPipe} from "../pipe/DateToString";

import {CapitalizeDirective} from "../Directive/capitalize.directive";

import {CustomerListPage} from "../pages/customer/list/list";
import {CustomerInfoPage} from "../pages/customer/info/info";
import {CustomerEditPage} from "../pages/customer/edit/edit";
import {InsureListPage} from "../pages/insure/list/list";
import {InsureEditPage} from "../pages/insure/edit/edit";
import {InsureInfoPage} from "../pages/insure/info/info";
import {TaskHandlePage} from "../pages/task/handle/handle";
import {HelpInfoPage} from "../pages/help/info/info";
import {HelpEditPage} from "../pages/help/edit/edit";
import {MapMylatPage} from "../pages/map/mylat/mylat";
import {MapGaragePage} from "../pages/map/garage/garage";
import {OrderListPage} from "../pages/order/list/list";
import {RunlistListPage} from "../pages/order/runlist/runlist";
import {ListGrabPage} from "../pages/help/listgrab/listgrab";
import {TeamTabsPage} from "../pages/team/teamtabs";
import {TeamTeaminfoPage} from "../pages/team/teaminfo/teaminfo";
import {SalesmanEditPage} from "../pages/salesman/edit/edit";
import {SalesmanInfoPage} from "../pages/salesman/info/info";
import {SalesmanListPage} from "../pages/salesman/list/list";
import {InsureExpirePage} from "../pages/insure/expire/expire";
import {UserFindPwdPage} from "../pages/user/findPwd/findPwd";
import {UserRegisterPage} from "../pages/user/register/register";
import {GrageInfoPage} from "../pages/garage/info/info";
import {MessageListPage} from "../pages/message/list/list";


@NgModule({
  declarations: [
    ImgUrlPipe,
    DateToStringPipe,
    CapitalizeDirective,
    
    MyApp,
    HomePage,
    UserLoginPage,
    UserMenuPage,
    UserEditPage,
    CustomerListPage,
    CustomerInfoPage,
    CustomerEditPage,
    InsureListPage,
    InsureEditPage,
    InsureInfoPage,
    TaskHandlePage,
    HelpInfoPage,
    HelpEditPage,
    MapMylatPage,
    MapGaragePage,
    OrderListPage,
    RunlistListPage,
    InsureExpirePage,
    ListGrabPage,
    TeamTabsPage,
    TeamTeaminfoPage,
    SalesmanEditPage,
    SalesmanInfoPage,
    SalesmanListPage,
    UserFindPwdPage,
    UserRegisterPage,
    GrageInfoPage,
    MessageListPage
  ],
  imports: [
    IonicModule.forRoot(MyApp),
    IonicStorageModule.forRoot(),
    HttpModule, 
    JsonpModule,
    BrowserModule
  ],
  bootstrap: [IonicApp],
  entryComponents: [
    MyApp,
    HomePage,
    UserLoginPage,
    UserEditPage,
    UserMenuPage,
    CustomerListPage,
    CustomerInfoPage,
    CustomerEditPage,
    InsureListPage,
    InsureEditPage,
    InsureInfoPage,
    TaskHandlePage,
    HelpInfoPage,
    HelpEditPage,
    MapMylatPage,
    MapGaragePage,
    OrderListPage,
    RunlistListPage,
    InsureExpirePage,
    ListGrabPage,
    TeamTabsPage,
    TeamTeaminfoPage,
    SalesmanEditPage,
    SalesmanInfoPage,
    SalesmanListPage,
    UserFindPwdPage,
    UserRegisterPage,
    GrageInfoPage,
    MessageListPage
  ],
  providers: [
    StatusBar,
    SplashScreen,
    Geolocation,
    Transfer,
    AppVersion,
    PhotoViewer,
    Camera,
    SplashScreen,
    FileOpener,
    ImagePicker,
    {provide: ErrorHandler, useClass: IonicErrorHandler}
    ]
})
export class AppModule {}
