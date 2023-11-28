using System;
using System.Linq;
using System.Web.Mvc;
using eCase.Data.Core;
using eCase.Data.Repositories;
using eCase.Web.Helpers;
using eCase.Common.Crypto;
using eCase.Web.Models.User;
using eCase.Domain.Entities;
using PagedList;
using eCase.Data.Core.Nomenclatures;
using System.Data.Entity;
using eCase.Domain.Core;
using System.Collections.Generic;
using eCase.Domain.Events;

namespace eCase.Web.Controllers
{
    [AuthorizeUser(AllowedRoleId = UserGroup.SuperAdmin)]
    public partial class UserController : BaseController
    {
        public UserController(IUnitOfWork unitOfWork
            , IUserRepository userRepository
            , IEntityCodeNomsRepository<Court, EntityCodeNomVO> courtRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _courtRepository = courtRepository;
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

            UserSearchVM vm = new UserSearchVM()
            {
                Name = name,
                Username = username,
                UserGroupId = userGroupId,

                Order = order,
                IsAsc = isAsc
            };

            FillSelectListItems(ref vm);

            #region show results

            IQueryable<User> users = _userRepository.SetWithoutIncludes().Where(e => e.UserGroupId != UserGroup.SystemAdmin && e.UserGroupId != UserGroup.SuperAdmin);

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

            if (!string.IsNullOrEmpty(vm.UserGroupId))
            {
                long ugId = long.Parse(vm.UserGroupId);
                users = users.Where(e => e.UserGroupId == ugId);
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
        public virtual ActionResult Search(UserSearchVM vm)
        {
            if (!ModelState.IsValid)
            {
                FillSelectListItems(ref vm);

                return View(vm);
            }

            UserSearchVM.EncryptProperties(vm);

            return RedirectToAction(ActionNames.Search, vm);
        }

        #endregion

        #region Edit

        [HttpGet]
        public virtual ActionResult Edit(Guid gid)
        {
            var user = _userRepository.SetWithoutIncludes().Where(e => e.Gid == gid)
                .Include(e => e.Court)
                .Single();
            if (user == null)
                return RedirectToAction(ActionNames.Search);

            ModelState.Clear();

            UserEditVM vm = new UserEditVM();
            vm.Gid = user.Gid;
            vm.Username = user.Username;
            vm.UserGroupId = user.UserGroupId;
            vm.Name = user.Name;
            vm.IsActive = user.IsActive;
            if (vm.HasCourt)
                vm.CourtCode = user.Court.Code;

            FillSelectListItems(ref vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(UserEditVM vm)
        {
            if (vm.HasCourt && String.IsNullOrWhiteSpace(vm.CourtCode))
                ModelState.AddModelError("CourtCode", "Полето Съд трябва да е попълнено.");

            if (!ModelState.IsValid)
            {
                FillSelectListItems(ref vm);
                return View(vm);
            }

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var user = _userRepository.FindByGid(vm.Gid);
                if (user == null)
                    return RedirectToAction(ActionNames.Search);

                bool isProfileChanged = false;

                if (user.Name != vm.Name || user.IsActive != vm.IsActive || user.Court.Code != vm.CourtCode)
                {
                    isProfileChanged = true;
                }

                user.Name = vm.Name;
                user.IsActive = vm.IsActive;

                if (vm.HasCourt)
                {
                    var courtId = _courtRepository.GetNomIdByCode(vm.CourtCode);
                    user.CourtId = courtId;
                }

                user.ModifyDate = DateTime.Now;

                if (isProfileChanged)
                {
                    string courtName = string.Empty;
                    if (user.CourtId != null)
                    {
                        courtName = _courtRepository.GetNom(user.CourtId.Value).Name;
                    }

                    user.UpdateUserProfile(user.Username, user.Name, user.IsActive.ToString(), courtName);
                }

                _unitOfWork.Save();
                transaction.Commit();
            }

            return RedirectToAction(ActionNames.Search);
        }

        #endregion

        #region Create

        [HttpGet]
        public virtual ActionResult Create()
        {
            ModelState.Clear();

            UserCreateVM vm = new UserCreateVM();
            vm.IsActive = true;

            FillSelectListItems(ref vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(UserCreateVM vm)
        {
            var existingUser = _userRepository.Find(vm.Username);
            if (existingUser != null)
                ModelState.AddModelError("Username", "Вече съществува потребител с тази електронна поща.");

            if (!ModelState.IsValid)
            {
                FillSelectListItems(ref vm);
                return View(vm);
            }

            var courtId = _courtRepository.GetNomIdByCode(vm.CourtCode);

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var user = new User(Guid.NewGuid(), UserGroup.CourtAdmin, vm.Name, vm.Username);

                user.IsActive = vm.IsActive;

                user.CourtId = courtId;

                _userRepository.Add(user);

                _unitOfWork.Save();
                transaction.Commit();
            }

            return RedirectToAction(ActionNames.Search);
        }

        #endregion



        #region Private

        private void FillSelectListItems(ref UserSearchVM vm)
        {
            if (vm == null)
                vm = new UserSearchVM();

            vm.UserGroups = UserGroupNomenclature.GetValues().Where(e => e.Value != UserGroup.SystemAdmin.ToString() && e.Value != UserGroup.SuperAdmin.ToString());
        }

        private void FillSelectListItems(ref UserEditVM vm)
        {
            if (vm == null)
                vm = new UserEditVM();

            if (vm.HasCourt)
                vm.Courts = _courtRepository.GetNoms(String.Empty).OrderBy(e => e.Name).Select(e => new SelectListItem() { Value = e.Code.ToString(), Text = e.Name });
        }

        private void FillSelectListItems(ref UserCreateVM vm)
        {
            if (vm == null)
                vm = new UserCreateVM();

            vm.Courts = _courtRepository.GetNoms(String.Empty).OrderBy(e => e.Name).Select(e => new SelectListItem() { Value = e.Code.ToString(), Text = e.Name });
        }

        private IUnitOfWork _unitOfWork;
        private IUserRepository _userRepository;
        private IEntityCodeNomsRepository<Court, EntityCodeNomVO> _courtRepository;

        #endregion

        #region SendMailToImportedUsers


        [HttpGet]
        public virtual ActionResult SendMailToImported(DateTime created, DateTime modified)
        {


            using (var transaction = _unitOfWork.BeginTransaction())
            {

                var users = _userRepository.FindByDateCreated(created);



                foreach (User user in users)
                {
                    if (user.CreateDate == user.ModifyDate)
                    {
                        ((IEventEmitter)user).Events = new List<IDomainEvent>();
                        ((IEventEmitter)user).Events.Add(new NewRegistrationEvent()
                        {
                            Email = user.Username,
                            ActivationCode = user.ActivationCode
                        });
                        user.ModifyDate = DateTime.Now;


                    }
                }

                _unitOfWork.Save();
                transaction.Commit();

            }
            return RedirectToAction(ActionNames.Search);
        }




        #endregion

        #region SendActivationMailToUser


        [HttpGet]
        public virtual ActionResult SendActivationMailToUser(string email)
        {


            using (var transaction = _unitOfWork.BeginTransaction())
            {

                var user = _userRepository.Find(email);

                if (user.IsActive != true)
                {
                    if (user.IsActivationCodeValid == false)
                    {
                        user.ActivationCode = Guid.NewGuid().ToString();
                        user.IsActivationCodeValid = true;
                    }
                    ((IEventEmitter)user).Events = new List<IDomainEvent>();
                    ((IEventEmitter)user).Events.Add(new NewRegistrationEvent()
                    {
                        Email = user.Username,
                        ActivationCode = user.ActivationCode
                    });
                    user.ModifyDate = DateTime.Now;

                    _unitOfWork.Save();
                    transaction.Commit();
                }
            }
            return RedirectToAction(ActionNames.Search);
        }




        #endregion
    }
}