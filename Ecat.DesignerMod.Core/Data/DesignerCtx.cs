using System.Data.Entity;
using Ecat.Shared.DbMgr.Context;

namespace Ecat.DesignerMod.Core.Data
{
    public class DesignerCtx: ContextBase<DesignerCtx>
    {
        protected override void OnModelCreating(DbModelBuilder mb)
        {

            //mb.Entity<>()
            //base.OnModelCreating(mb);
        }

    }
}
