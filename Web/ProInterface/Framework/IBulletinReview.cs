
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProInterface.Models;

namespace ProInterface
{
    /// <summary>
    /// 公告回复内容
    /// </summary>
    public interface IBulletinReview
    {
        /// <summary>
        /// 修改公告回复
        /// </summary>
        /// <param name="loginKey">登录凭证</param>
        /// <param name="err">错误信息</param>
        /// <param name="inEnt">实体类</param>
        /// <param name="allPar">更新的参数</param>
        /// <returns>修改公告回复</returns>
        bool BulletinReviewSave(string loginKey, ref ErrorInfo err, BulletinReview inEnt, IList<string> allPar);
    }
}
