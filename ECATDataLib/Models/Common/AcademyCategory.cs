﻿using TypeLite;

namespace Ecat.Data.Models.Common
{
    [TsClass(Module = "ecat.entity.s.common")]
    public class AcademyCategory
    {
        public string Id { get; set; }
        public string BbCatId { get; set; }
        public string BbCatName { get; set; }
        public int RelatedCoursesCount { get; set; }
    }
}