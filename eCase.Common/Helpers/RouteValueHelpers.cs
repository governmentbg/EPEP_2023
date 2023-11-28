using System.Collections.Generic;
using System.Web.Routing;

namespace eCase.Common.Helpers
{
    /// <summary>
    /// Клас, съдържащ помощни методи за хештаблицата RouteValueDictionary
    /// </summary>
    public static class RouteValuesHelpers
    {
        /// <summary>
        /// Връща записите от хештаблицата
        /// </summary>
        /// <param name="routeValues">хештаблица</param>
        /// <returns></returns>
        public static RouteValueDictionary GetRouteValues(RouteValueDictionary routeValues)
        {
            return (routeValues != null) ? new RouteValueDictionary(routeValues) : new RouteValueDictionary();
        }

        /// <summary>
        /// Добавя записи към хештаблицата
        /// </summary>
        /// <param name="actionName">име на метод</param>
        /// <param name="controllerName">име на котролер</param>
        /// <param name="implicitRouteValues">хештаблица с допълнителни записи</param>
        /// <param name="routeValues">хештаблица</param>
        /// <param name="includeImplicitMvcValues">флаг, показващ дали се добавят допълнителните записи</param>
        /// <returns></returns>
        public static RouteValueDictionary MergeRouteValues(string actionName, string controllerName, RouteValueDictionary implicitRouteValues, RouteValueDictionary routeValues, bool includeImplicitMvcValues)
        {
            // Create a new dictionary containing implicit and auto-generated values
            RouteValueDictionary mergedRouteValues = new RouteValueDictionary();

            if (includeImplicitMvcValues)
            {
                // We only include MVC-specific values like 'controller' and 'action' if we are generating an action link.
                // If we are generating a route link [as to MapRoute("Foo", "any/url", new { controller = ... })], including
                // the current controller name will cause the route match to fail if the current controller is not the same
                // as the destination controller.

                object implicitValue;
                if (implicitRouteValues != null && implicitRouteValues.TryGetValue("action", out implicitValue))
                {
                    mergedRouteValues["action"] = implicitValue;
                }

                if (implicitRouteValues != null && implicitRouteValues.TryGetValue("controller", out implicitValue))
                {
                    mergedRouteValues["controller"] = implicitValue;
                }
            }

            // Merge values from the user's dictionary/object
            if (routeValues != null)
            {
                foreach (KeyValuePair<string, object> routeElement in GetRouteValues(routeValues))
                {
                    mergedRouteValues[routeElement.Key] = routeElement.Value;
                }
            }

            // Merge explicit parameters when not null
            if (actionName != null)
            {
                mergedRouteValues["action"] = actionName;
            }

            if (controllerName != null)
            {
                mergedRouteValues["controller"] = controllerName;
            }

            return mergedRouteValues;
        }
    }
}
