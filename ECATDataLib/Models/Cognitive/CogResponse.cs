﻿using System;
using TypeLite;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
//using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Data.Models.Designer;

namespace Ecat.Data.Models.Cognitive
{
    [TsClass(Module ="ecat.entity.s.cog")]
    public class CogResponse
    {
        public int PersonId { get; set; }
        public int CogInventoryId { get; set; }
        public double ItemScore { get; set; }
        public int Attempt { get; set; }
        //[TsIgnore]
        //public bool IsDeleted { get; set; }
        //[TsIgnore]
        //public int? DeletedById { get; set; }
        //[TsIgnore]
        //public DateTime? DeletedDate { get; set; }
        public CogInventory InventoryItem { get; set; }
        public Person Person { get; set; }
    }
}
