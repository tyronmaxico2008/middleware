using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace webui
{
    public class checkLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (g.parseInt(ui.oTier.getCookie("userid")) == 0)
            {
                filterContext.Result = new RedirectResult(ui.getAppSettings("loginPage"));
                base.OnActionExecuting(filterContext);
                return;
            }


            //if (_oBL.isAuthorized(ModuleName, OperationName) == false)
            //{
            //    filterContext.Result = new RedirectResult("~/System/UnAuthorized");
            //    base.OnActionExecuting(filterContext);
            //    return;
            //}
            

            /*
            var cls1 = new myAppConfig();
            
            if (cls1.LoggedInUserID == 0)
            {
                filterContext.Result = new RedirectResult("~/System/Login2");
                return;
            }
            
            */


        }
    }
}
