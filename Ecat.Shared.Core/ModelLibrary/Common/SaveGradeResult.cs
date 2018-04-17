using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Common
{
    [TsClass(Module = "ecat.entity.s.lmsAdmin")]
    public class SaveGradeResult 
    {
        public int CourseId { get; set; }
        public string WgCategory { get; set; }
        public bool Success { get; set; }
        public int NumOfStudents { get; set; }
        public int SentScores { get; set; }
        public int ReturnedScores { get; set; }
        public string Message { get; set; }
    }
}
