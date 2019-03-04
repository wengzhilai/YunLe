
import { Pipe, PipeTransform } from '@angular/core';
import {CarIn} from "../Classes/CarIn";
@Pipe({name: 'ImgUrl'})
export class ImgUrlPipe implements PipeTransform {
  transform(url: string): string {
    if (url == null || url == '' || url == undefined) {
      return null;
    }
    if(url.indexOf('http://')==-1){
      return CarIn.imgUrl + url.replace("~", "").replace("/YL", "");
    }
    else{
      return url;
    }
  }
}
