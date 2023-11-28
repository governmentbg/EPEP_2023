using eCase.Common.Crypto;
using eCase.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eCase.Web.Models.User
{
    public class UserSearchVM
    {
        #region Search parameters

        [Display(Name = "Име")]
        public string Name { get; set; }

        [Display(Name = "Електронна поща")]
        public string Username { get; set; }

        [Display(Name = "Потребителска група")]
        public string UserGroupId { get; set; }

        public UsersOrder Order { get; set; }
        public bool IsAsc { get; set; }

        #endregion

        #region Selects

        public IEnumerable<SelectListItem> UserGroups { get; set; }

        #endregion

        #region SearchResults

        public PagedList.IPagedList<eCase.Domain.Entities.User> SearchResults { get; set; }

        #endregion

        #region Statics

        public static void EncryptProperties(UserSearchVM vm)
        {
            vm.Username = ConfigurationBasedStringEncrypter.Encrypt(vm.Username);
            vm.Name = ConfigurationBasedStringEncrypter.Encrypt(vm.Name);
            vm.UserGroupId = ConfigurationBasedStringEncrypter.Encrypt(vm.UserGroupId);
        }

        #endregion

        #region RouteValues

        public object GetRouteValues(NameValueCollection queryString
            , string page = ""
            , UsersOrder? order = null)
        {
            string isAscString = queryString["isAsc"];

            if (order.HasValue)
            {
                if (this.Order == order.Value)
                    isAscString = (!this.IsAsc).ToString();
                else
                    isAscString = true.ToString();
            }

            var result = new
            {
                name = queryString["name"],
                username = queryString["username"],
                userGroupId = queryString["userGroupId"],

                page = page,

                order = order.HasValue ? order.ToString() : queryString["order"],
                isAsc = isAscString
            };

            return result;
        }

        #endregion
    }
}