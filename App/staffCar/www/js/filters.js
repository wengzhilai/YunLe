/**
 * Created by Administrator on 2016/5/7.
 */
angular.module("filters", [])
  .filter("toBool", function () {
    return function (obj, param1) {
      // console.log(param1);
      if (obj == null || obj == '') {
        if (param1 == 0) {
          return 0;
        } else if (param1 == 1) {
          return '是';
        } else if (param1 == 2) {
          return '无';
        }

      } else {
        if (param1 == 0) {
          return 1;
        } else if (param1 == 1) {
          return '否';
        } else if (param1 == 2) {
          return '有';
        }
      }
    }
  })
  .filter("FmtDateTime", function (common) {
    return function (timestamp, fmt) {
      if (timestamp == null || timestamp == '' || timestamp == undefined) {
        return '';
      }
      console.log(fmt);
      var d = new Date(timestamp);
      console.log(d);
      if (d != null) {
        return common.dateFormat(d, fmt);
      }
      return '';
    }
  })
  .filter("UnixNum", function () {
    return function (timestamp) {
      if (timestamp == null || timestamp == '' || timestamp == undefined) {
        return '';
      }
      // console.log("UnixNum");
      return timestamp.replace("/Date(", "").replace(")/", "");
    }
  })
  .filter("diffUnixNum", function () {
    return function (timestamp) {
      if (timestamp == null || timestamp == '' || timestamp == undefined) {
        return '';
      }
      var inTime=new Date(parseFloat(timestamp.replace("/Date(", "").replace(")/", "")));
      inTime.setFullYear(inTime.getFullYear()+1);
      inTime.setDate(inTime.getDate()-1);
      var inTimesTick=inTime.getTime();
      var nowTimesTick=new Date().getTime();
      var dayNum=Math.floor((inTimesTick-nowTimesTick)/(24*3600*1000))
      return dayNum;
    }
  })

  .filter("imgUrl", function (CarIn) {
    return function (url) {
      if (url == null || url == '' || url == undefined) {
        return "img/noPic.png";
      }
      return CarIn.imgUrl + url.replace("~", "").replace("/YL", "");
    }
  });

