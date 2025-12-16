
using FINTCS.DTOs;
using FINTCS.Models;
using System.Collections.Generic;

namespace FINTCS.Repository
{
    public interface IAccountService
    {
        List<GroupMaster> GetAllGroups();
        GroupMaster? GetById(int id);
        void SaveGroup(GroupMaster model);
        void UpdateGroupWithNewCode(GroupMaster model, int? oldUnderId);
        bool IsDuplicateName(string name, int id);

        GroupMaster? GetGroupById(int id);

        List<GroupMaster> GetGroups();
        List<LEDGERAC> GetLedgers();

        List<LedgerListDto> GetLedgersWithGroup();
       
        LEDGERAC? GetLedgerById(int id);
        void SaveLedger(LEDGERAC ledger);
        void UpdateLedger(LEDGERAC ledger);

        string GenerateLedgerCode(int groupId);
        void UpdateLedgerWithCodeAdjust(LEDGERAC ledger);
    }
}
