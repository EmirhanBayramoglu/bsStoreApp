﻿using Entities.LogModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Contract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

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

        private string Log(string modelName, RouteData routeData)
        {
            var logDetails = new LogDetails()
            {
                ModelModel = modelName,
                Controller = routeData.Values["controller"],
                Action = routeData.Values["action"]
            };

            if (routeData.Values.Count >= 3)
                logDetails.Id = routeData.Values["Id"];

            return logDetails.ToString();
        }
    }
}
