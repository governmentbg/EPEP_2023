﻿using System;
using System.Text;
using System.Web;
using NLog;
using NLog.LayoutRenderers;

namespace eCase.Common.NLog
{
    [LayoutRenderer("requestId")]
    public class RequestIdLayoutRenderer : LayoutRenderer
    {
        private static readonly string RequestIdKey = "__NLogRequestIdKey__";

        protected override void Append(StringBuilder builder, LogEventInfo ev)
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                builder.Append(Guid.Empty);
            }
            else
            {
                if (!context.Items.Contains(RequestIdKey))
                {
                    context.Items.Add(RequestIdKey, Guid.NewGuid());
                }

                builder.Append(context.Items[RequestIdKey]);
            }
        }
    }
}
