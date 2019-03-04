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

  .filter("imgUrl", function (CarIn) {
    return function (url) {
      if (url == null || url == '' || url == undefined) {
        return "img/noPic.png";
      }
      return CarIn.imgUrl + url.replace("~", "").replace("/YL", "");
    }
  });

