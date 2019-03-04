angular.module('config', [])
  .constant("CarIn", {
    weixinAppId: "wx3b8877cd39306fe6",
    pageSize: 10,
    allPageSize: 100,
    api: 'http://139.129.194.140/YL/WebApi/',
    baseUrl: 'http://139.129.194.140/YL',
    imgUrl: 'http://139.129.194.140/YL',
    // api: 'http://127.0.0.1/YL/WebApi/',
    // imgUrl: 'http://127.0.0.1/YL',
    version: '1.0.0',
    /**
     * 文件上传参数
     */
    maximumImagesCount: 1,
    imgWidth: 800,
    imgHeight: 800,
    quality: 50,
    isAllowEdit: true,   //拍照后是否允许简单编辑
    isSaveToPhotoAlbum: false,    //是否要保存到相册
    isCorrectOrientation: true   //拍摄方向限制
  });
