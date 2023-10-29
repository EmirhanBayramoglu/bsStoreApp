using Entities.LogModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.ActionFilters
{
    public class LogFilterAttribute : ActionFilterAttribute
    {
        private readonly ILoggerService _logger;

        public LogFilterAttribute(ILoggerService logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInfo(Log("OnActionExecuting", context.RouteData));
        }

        private string Log(string modelName, RouteData RouteData)
        {
            var logDetails = new LogDetails()
            {
                ModelName = modelName,
                Controller = RouteData.Values["controller"],
                Action = RouteData.Values["action"]
            };

            if (RouteData.Values.Count >= 3)
            {
                logDetails.Id = RouteData.Values["id"];
            }
            return logDetails.ToString();
        }
    }
}
