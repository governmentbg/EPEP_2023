using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eCase.Web.Helpers
{
    public enum MenuItems
    {
        Home = 1,
        Cases = 2,
        Summons = 3,
        Users = 4,
        CourtsList = 5,
        AccessRules = 6,
        Feedback = 7,
        CasesCount = 8,
        Lawyers = 9,
        CourtUser = 10,

        Other = 100
    }

    public static partial class MenuManager
    {
        public static MenuItems GetCurrentMenuItem(string actionName, string controllerName, bool isAuthenticated)
        {
            // Home
            if (controllerName.Equals(MVC.Home.Name) && actionName.Equals(MVC.Home.ActionNames.Index))
                return MenuItems.Home;

            // Cases
            if (controllerName.Equals(MVC.Act.Name) || controllerName.Equals(MVC.Case.Name) || controllerName.Equals(MVC.Hearing.Name))
                return MenuItems.Cases;

            // Summons
            if (controllerName.Equals(MVC.Summon.Name))
                return MenuItems.Summons;

            // Users
            if (controllerName.Equals(MVC.User.Name))
                return MenuItems.Users;

            // CourtsList
            if (controllerName.Equals(MVC.Home.Name) && actionName.Equals(MVC.Home.ActionNames.CourtsList))
                return MenuItems.CourtsList;

            // AccessRules
            if (controllerName.Equals(MVC.Home.Name) && actionName.Equals(MVC.Home.ActionNames.ElectronicCasesAccessRules))
                return MenuItems.AccessRules;

            // Feedback
            if (controllerName.Equals(MVC.Feedback.Name))
                return MenuItems.Feedback;

            // CaseCount
            if (controllerName.Equals(MVC.Statistics.Name) && actionName.Equals(MVC.Statistics.ActionNames.Cases))
                return MenuItems.CasesCount;

            // Lawyers
            if (controllerName.Equals(MVC.Lawyer.Name))
                return MenuItems.Lawyers;

            // CourtUser
            if (controllerName.Equals(MVC.CourtUser.Name))
                return MenuItems.CourtUser;

            return MenuItems.Other;
        }
    }
}