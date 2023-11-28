using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;
using eCase.Data.Core;
using eCase.Data.Repositories;
using eCase.Domain.Entities;
using eCase.Data.Core.Nomenclatures;
using System.Data.Entity;

namespace eCase.Web.Controllers
{
    public class PublicController : ApiController
    {
        private IHearingRepository _hearingRepository;
        private IActRepository _actRepository;
        private IEntityCodeNomsRepository<Court, EntityCodeNomVO> _courtRepository;

        public PublicController(IActRepository actRepository, IHearingRepository hearingRepository,
            IEntityCodeNomsRepository<Court, EntityCodeNomVO> courtRepository)
        {
            _actRepository = actRepository;
            _hearingRepository = hearingRepository;
            _courtRepository = courtRepository;
        }

        public List<HearingVO> GetHearings(DateTime from, DateTime to, string courtCode, string hearingType)
        {
            if (string.IsNullOrEmpty(courtCode))
                throw new ArgumentNullException("courtCode");

            if (string.IsNullOrEmpty(hearingType))
                throw new ArgumentNullException("hearingType");

            if (!_courtRepository.HasCode(courtCode))
                throw new InvalidEnumArgumentException("courtCode");

            var courtId = _courtRepository.GetNomIdByCode(courtCode);

            return _hearingRepository.SetWithoutIncludes()
                .Include(e => e.Case)
                .Include(e => e.Case.CaseKind)
                .Include(e => e.Case.CaseType)
                .Include(e => e.Case.Court)
                .Where
               (
                   e =>
                       e.Date >= from &&
                       e.Date <= to &&
                       e.Case.CourtId == courtId &&
                       e.HearingType.ToLower().Contains(hearingType.ToLower())
               )
                .Select(e => new HearingVO
                {
                    HearingGid = e.Gid,
                    Case = new CaseVO()
                    {
                        CourtName = e.Case.Court.Name,
                        CaseKindName = e.Case.CaseKind.Name,
                        CaseTypeName = e.Case.CaseType.Name,
                        IncomingNumber = e.Case.IncomingDocument.IncomingNumber,
                        Number = e.Case.Number,
                        CaseYear = e.Case.CaseYear,
                        FormationDate = e.Case.FormationDate,
                        DepartmentName = e.Case.DepartmentName,
                        PanelName = e.Case.PanelName,
                    },
                    HearingType = e.HearingType,
                    HearingResult = e.HearingResult,
                    Date = e.Date,
                    SecretaryName = e.SecretaryName,
                    ProsecutorName = e.ProsecutorName
                })
                .ToList();
        }

        public List<ActVO> GetActs(DateTime from, DateTime to, string courtCode)
        {
            if (string.IsNullOrEmpty(courtCode))
                throw new ArgumentNullException("courtCode");

            if (!_courtRepository.HasCode(courtCode))
                throw new InvalidEnumArgumentException("courtCode");

            var courtId = _courtRepository.GetNomIdByCode(courtCode);

            return _actRepository.SetWithoutIncludes()
                .Include(e => e.ActKind)
                .Include(e => e.Case)
                .Include(e => e.Case.CaseKind)
                .Include(e => e.Case.CaseType)
                .Include(e => e.Case.Court)
                .Where
                (
                    e =>
                        e.DateSigned >= from &&
                        e.DateSigned <= to &&
                        e.Case.CourtId == courtId
                )
                .Select(e => new ActVO
                {
                    ActGid = e.Gid,
                    Case = new CaseVO()
                    {
                        CourtName = e.Case.Court.Name,
                        CaseKindName = e.Case.CaseKind.Name,
                        CaseTypeName = e.Case.CaseType.Name,
                        IncomingNumber = e.Case.IncomingDocument.IncomingNumber,
                        Number = e.Case.Number,
                        CaseYear = e.Case.CaseYear,
                        FormationDate = e.Case.FormationDate,
                        DepartmentName = e.Case.DepartmentName,
                        PanelName = e.Case.PanelName,
                    },
                    ActKindName = e.ActKind.Name,
                    Number = e.Number,
                    DateSigned = e.DateSigned,
                    DateInPower = e.DateInPower,
                    MotiveDate = e.MotiveDate
                })
                .ToList();
        }
    }

    public class CaseVO
    {
        public string CourtName { get; set; }
        public string CaseKindName { get; set; }
        public string CaseTypeName { get; set; }
        public int IncomingNumber { get; set; }
        public int Number { get; set; }
        public int CaseYear { get; set; }
        public DateTime FormationDate { get; set; }
        public string DepartmentName { get; set; }
        public string PanelName { get; set; }
    }

    public class HearingVO
    {
        public Guid HearingGid { get; set; }
        public CaseVO Case { get; set; }
        public string HearingType { get; set; }
        public string HearingResult { get; set; }
        public DateTime Date { get; set; }
        public string SecretaryName { get; set; }
        public string ProsecutorName { get; set; }
    }

    public class ActVO
    {
        public Guid ActGid { get; set; }
        public CaseVO Case { get; set; }
        public string ActKindName { get; set; }
        //public Nullable<Guid> HearingId { get; set; }
        public Nullable<int> Number { get; set; }
        public DateTime DateSigned { get; set; }
        public Nullable<DateTime> DateInPower { get; set; }
        public Nullable<DateTime> MotiveDate { get; set; }
    }
}
