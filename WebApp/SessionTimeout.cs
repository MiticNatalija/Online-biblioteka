using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KnjigeRedis;
using System.Web.SessionState;

namespace WebApp
{
    public class SessionTimeout : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["korisnik"] == null)
            {
                filterContext.Result = new RedirectResult("~/Korisnik/IndexLog");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
    
    
}