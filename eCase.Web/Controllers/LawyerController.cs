using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

using PagedList;

using eCase.Common.Crypto;
using eCase.Data.Core;
using eCase.Data.Repositories;
using eCase.Domain.Entities;
using eCase.Web.Helpers;
using eCase.Web.Models.Lawyer;

namespace eCase.Web.Controllers
{
    [AuthorizeUser(AllowedRoleId = UserGroup.SuperAdmin)]
    public partial class LawyerController : BaseController
    {
        public LawyerController(IUnitOfWork unitOfWork,
            ILawyerRepository lawyerRepository)
        {
            _unitOfWork = unitOfWork;
            _lawyerRepository = lawyerRepository;
        }

        #region Search

        [HttpGet]
        [DecryptParameters(IdsParamName =
            new string[]
            {
                "number",
                "name",
                "lawyerTypeId",
                "page"
            })]
        public virtual ActionResult Search(
            string number = "",
            string name = "",
            string lawyerTypeId = "",

            string page = ""
            )
        {
            ModelState.Clear();

            LawyerSearchVM vm = new LawyerSearchVM()
            {
                Number = number,
                Name = name,
                LawyerTypeId = lawyerTypeId
            };

            FillSelectListItems(ref vm);

            #region show results

            IQueryable<Lawyer> lawyers = _lawyerRepository.SetWithoutIncludes();

            if (!string.IsNullOrWhiteSpace(vm.Number))
            {
                lawyers = lawyers.Where(e => e.Number == vm.Number);
            }

            if (!string.IsNullOrWhiteSpace(vm.Name))
            {
                string[] words = vm.Name.Split(' ');

                for (int i = 0; i < words.Count(); i++)
                {
                    if (!string.IsNullOrWhiteSpace(words[i]))
                    {
                        string term = words[i];
                        lawyers = lawyers.Where(e => e.Name.ToLower().Contains(term.ToLower()));
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(lawyerTypeId))
            {
                long pLawyerTypeId = 0;

                if (long.TryParse(vm.LawyerTypeId, out pLawyerTypeId))
                {
                    lawyers = lawyers.Where(e => e.LawyerTypeId == pLawyerTypeId);
                }
            }

            var enumerableLawyers = lawyers
                .Include(e => e.LawyerType)
                .OrderBy(e => e.Name)
                .ThenBy(e => e.Number)
                .ToList();

            int innerPage = string.IsNullOrWhiteSpace(page) ? 1 : int.Parse(page);

            vm.SearchResults = enumerableLawyers.ToPagedList(innerPage, Statics.MaxUserItemsPerPage);

            #endregion

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Search(LawyerSearchVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            LawyerSearchVM.EncryptProperties(vm);

            return RedirectToAction(ActionNames.Search, vm);
        }

        #endregion

        #region Edit

        [HttpGet]
        public virtual ActionResult Edit(Guid gid)
        {
            var lawyer = _lawyerRepository.SetWithoutIncludes().Where(e => e.Gid == gid)
                .Include(e => e.LawyerType)
                .Single();
            if (lawyer == null)
                return RedirectToAction(ActionNames.Search);

            ModelState.Clear();

            LawyerEditVM vm = new LawyerEditVM();
            vm.Gid = lawyer.Gid;
            vm.Number = lawyer.Number;
            vm.Name = lawyer.Name;
            vm.LawyerTypeId = lawyer.LawyerTypeId;
            vm.College = lawyer.College;

            FillSelectListItems(ref vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(LawyerEditVM vm)
        {
            if (!ModelState.IsValid)
            {
                FillSelectListItems(ref vm);
                return View(vm);
            }

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var lawyer = _lawyerRepository.FindByGid(vm.Gid);
                if (lawyer == null)
                    return RedirectToAction(ActionNames.Search);

                lawyer.Number = vm.Number;
                lawyer.Name = vm.Name;
                lawyer.LawyerTypeId = vm.LawyerTypeId;
                lawyer.College = vm.College;
                lawyer.ModifyDate = DateTime.Now;

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

            LawyerCreateVM vm = new LawyerCreateVM();

            FillSelectListItems(ref vm);
            return View(vm);
        }

        public virtual ActionResult Create(LawyerCreateVM vm)
        {
            var existingLawyer = _lawyerRepository.GetLawyerByNumber(vm.Number);
            if (existingLawyer != null)
                ModelState.AddModelError("Number", "Вече съществува адвокат с този номер.");

            if (!ModelState.IsValid)
            {
                FillSelectListItems(ref vm);
                return View(vm);
            }

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var lawyer = new Lawyer()
                {
                    Gid = Guid.NewGuid(),
                    Number = vm.Number,
                    Name = vm.Name,
                    LawyerTypeId = vm.LawyerTypeId,
                    College = vm.College,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now
                };

                _lawyerRepository.Add(lawyer);

                _unitOfWork.Save();
                transaction.Commit();
            }

            return RedirectToAction(ActionNames.Search);
        }

        #endregion

        #region Private

        private void FillSelectListItems(ref LawyerSearchVM vm)
        {
            if (vm == null)
                vm = new LawyerSearchVM();

            vm.LawyerTypes = _lawyerTypes;
        }

        private void FillSelectListItems(ref LawyerEditVM vm)
        {
            if (vm == null)
                vm = new LawyerEditVM();

            vm.LawyerTypes = _lawyerTypes;
        }

        private void FillSelectListItems(ref LawyerCreateVM vm)
        {
            if (vm == null)
                vm = new LawyerCreateVM();

            vm.LawyerTypes = _lawyerTypes;
        }

        
        private IUnitOfWork _unitOfWork;
        private ILawyerRepository _lawyerRepository;

        public IEnumerable<SelectListItem> _lawyerTypes
        {
            get
            {
                return ((UnitOfWork)_unitOfWork).DbContext.Set<LawyerType>().OrderBy(e => e.Name).Select(e => new SelectListItem() { Value = e.LawyerTypeId.ToString(), Text = e.Name });
            }
        }

        #endregion
    }
}