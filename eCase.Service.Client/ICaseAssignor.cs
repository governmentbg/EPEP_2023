using System;
using System.Collections.Generic;

namespace eCase.Service.Client
{
    public interface ICaseAssignor
    {
        /// <summary>
        /// Register lawyer and assign him list of cases
        /// </summary>
        /// <param name="lawyerData">lawyer's unique identifier and e-mail</param>
        /// <param name="caseIds">list of case unique identifiers</param>
        void AssignLawyerToCases(Tuple<Guid, string> lawyerData, List<Guid> caseIds);

        /// <summary>
        /// Register person and assign him list of cases
        /// </summary>
        /// <param name="personData">person's name and e-mail</param>
        /// <param name="caseIds">list of case unique identifiers</param>
        void AssignPersonToCases(Tuple<string, string> personData, List<Guid> caseIds);
    }
}
