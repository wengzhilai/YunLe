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
import {ImgUrlPipe } from "../pipe/imgUrl";
import {DateToStringPipe} from "../pipe/DateToString";
import {CapitalizeDirective} from "../Directive/capitalize.directive";
import { MyApp } from './app.component';


import { HomePage,BlankPage } from '../pages/home/home';
import { UserLoginPage } from '../pages/user/login/login';
import {UserFindPwdPage} from "../pages/user/findPwd/findPwd";
import {UserRegisterPage} from "../pages/user/register/register";
import {UserMenuPage} from "../pages/user/menu/menu";
import {HelpEditPage} from "../pages/help/edit/edit";
import {InsureInfoPage} from "../pages/insure/info/info";
import {HelpInfoPage} from "../pages/help/info/info";
import {InsureEditPage} from "../pages/insure/edit/edit";
import {InsureListPage} from "../pages/insure/list/list";
import {MapGaragePage} from "../pages/map/garage/garage";
import {MapMylatPage} from "../pages/map/mylat/mylat";
import {OrderListPage} from "../pages/order/list/list";
import {TaskHandlePage} from "../pages/task/handle/handle";
import {UserEditPage} from "../pages/user/edit/edit";
import {CarEditPage} from "../pages/car/edit/edit";
import {CarListPage} from "../pages/car/list/list";
import {GrageInfoPage} from "../pages/garage/info/info";
import {MessageListPage} from "../pages/message/list/list";




@NgModule({
  declarations: [
    ImgUrlPipe,
    DateToStringPipe,
    CapitalizeDirective,

    MyApp,
    HomePage,
    BlankPage,
    UserLoginPage,
    UserFindPwdPage,
    UserRegisterPage,
    UserMenuPage,
    UserEditPage,
    HelpEditPage,
    HelpInfoPage,
    InsureEditPage,
    InsureInfoPage,
    InsureListPage,
    MapGaragePage,
    MapMylatPage,
    OrderListPage,
    TaskHandlePage,
    CarEditPage,
    CarListPage,
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
    BlankPage,
    UserLoginPage,
    UserFindPwdPage,
    UserRegisterPage,
    UserMenuPage,
    UserEditPage,
    HelpEditPage,
    HelpInfoPage,
    InsureEditPage,
    InsureInfoPage,
    InsureListPage,
    MapGaragePage,
    MapMylatPage,
    OrderListPage,
    TaskHandlePage,
    CarEditPage,
    CarListPage,
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
