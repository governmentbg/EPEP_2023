using System;
using System.Linq;
using System.Web.Mvc;
using eCase.Data.Core;
using eCase.Data.Repositories;
using eCase.Web.Helpers;
using eCase.Common.Crypto;
using eCase.Web.Models.CourtUser;
using eCase.Domain.Entities;
using PagedList;
using eCase.Data.Core.Nomenclatures;
using System.Data.Entity;

namespace eCase.Web.Controllers
{
    [AuthorizeUser(AllowedRoleId = UserGroup.CourtAdmin)]
    public partial class CourtUserController : BaseController
    {
        public CourtUserController(IUnitOfWork unitOfWork
            , IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        #region Search

        [HttpGet]
        [DecryptParametersAttribute(IdsParamName =
            new string[]
            {
                    "name",
                    "username",
                    "userGroupId",
                    "page"
            })]
        public virtual ActionResult Search(
                string name = "",
                string username = "",
                string userGroupId = "",

                string page = "",

                UsersOrder order = UsersOrder.Username,
                bool isAsc = true
            )
        {
            ModelState.Clear();

            CourtUserSearchVM vm = new CourtUserSearchVM()
            {
                Name = name,
                Username = username,
                UserGroupId = userGroupId,

                Order = order,
                IsAsc = isAsc
            };

            FillSelectListItems(ref vm);

            #region show results

            IQueryable<User> users;


            if (!string.IsNullOrEmpty(vm.UserGroupId))
            {
                users = _userRepository.GetUsersForCourt(long.Parse(CurrentUser.CourtId), long.Parse(vm.UserGroupId));
            }
            else
            {
                users = _userRepository.GetUsersForCourt(long.Parse(CurrentUser.CourtId));
            }

            if (!string.IsNullOrEmpty(vm.Name))
            {
                string[] words = vm.Name.Split(' ');

                for (int i = 0; i < words.Count(); i++)
                {
                    if (!string.IsNullOrWhiteSpace(words[i]))
                    {
                        string term = words[i];
                        users = users.Where(e => e.Name.ToLower().Contains(term.ToLower()));
                    }
                }
            }

            if (!string.IsNullOrEmpty(vm.Username))
            {
                users = users.Where(e => e.Username.ToLower().Contains(vm.Username.ToLower()));
            }

            var enumerableUsers = users
                .Include(e => e.UserGroup)
                .Include(e => e.Court)
                .ToList();

            #region Order

            if (order == UsersOrder.Username)
                enumerableUsers = isAsc ? enumerableUsers.OrderBy(e => e.Username).ToList()
                                : enumerableUsers.OrderByDescending(e => e.Username).ToList();
            else if (order == UsersOrder.UserGroup)
                enumerableUsers = isAsc ? enumerableUsers.OrderBy(e => e.UserGroup.Description).ToList()
                                : enumerableUsers.OrderByDescending(e => e.UserGroup.Description).ToList();
            else if (order == UsersOrder.Name)
                enumerableUsers = isAsc ? enumerableUsers.OrderBy(e => e.Name).ToList()
                                : enumerableUsers.OrderByDescending(e => e.Name).ToList();
            else if (order == UsersOrder.Court)
                enumerableUsers = isAsc ? enumerableUsers.OrderBy(e => e.Court != null ? e.Court.Name : "").ToList()
                                : enumerableUsers.OrderByDescending(e => e.Court != null ? e.Court.Name : "").ToList();
            else if (order == UsersOrder.IsActive)
                enumerableUsers = isAsc ? enumerableUsers.OrderBy(e => e.IsActive).ToList()
                                : enumerableUsers.OrderByDescending(e => e.IsActive).ToList();

            #endregion

            int innerPage = string.IsNullOrEmpty(page) ? 1 : int.Parse(page);

            vm.SearchResults = enumerableUsers.ToPagedList(innerPage, Statics.MaxUserItemsPerPage);

            #endregion

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Search(CourtUserSearchVM vm)
        {
            if (!ModelState.IsValid)
            {
                FillSelectListItems(ref vm);

                return View(vm);
            }

            CourtUserSearchVM.EncryptProperties(vm);

            return RedirectToAction(ActionNames.Search, vm);
        }

        #endregion
        
        #region Private

        private void FillSelectListItems(ref CourtUserSearchVM vm)
        {
            if (vm == null)
                vm = new CourtUserSearchVM();

            vm.UserGroups = UserGroupNomenclature.GetValues().Where(e => e.Value == UserGroup.Person.ToString() || e.Value == UserGroup.Lawyer.ToString());
        }
        
        private IUnitOfWork _unitOfWork;
        private IUserRepository _userRepository;

        #endregion

    }
}