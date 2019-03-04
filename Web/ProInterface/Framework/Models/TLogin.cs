using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProInterface.Models
{
    public class TLogin:LOGIN
    {
        /// <summary>
        /// md5加密用户名
        /// </summary>
        public string Md5LoginName { get; set; }
        /// <summary>
        /// md5加密密码
        /// </summary>
        public string Md5PassWord { get; set; }
        /// <summary>
        /// ase加密用户名
        /// </summary>
        public string AseLoginName { get; set; }
        /// <summary>
        /// ase加密密码
        /// </summary>
        public string AsePassWord { get; set; }
        /// <summary>
        /// ase加密手机号
        /// </summary>
        public string AsePhoneNo { get; set; }
    }
}
