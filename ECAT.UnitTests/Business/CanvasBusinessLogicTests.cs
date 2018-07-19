using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ecat.Business.Business;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Canvas;
using Ecat.Data.Models.School;
using FluentAssertions;
using Telerik.JustMock;

namespace Ecat.Business.Business.Tests
{
    [TestClass()]
    public class CanvasBusinessLogicTests
    {
        [TestMethod()]
        // public static List<Course> ReconcileCourses(List<CanvasCourse> canvasCoursesReturned, List<Course> existingCourses, string academyId)
        public void ReconcileCourses_NewCourseInCanvas_ReturnListWithNewCourse()
        {
            //Arrange

            var canvasCourse = Mock.Create<CanvasCourse>();
            canvasCourse.id = 7016;
            canvasCourse.account_id = 374;
            canvasCourse.start_at = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            canvasCourse.end_at = DateTime.Now.AddDays(12).ToString(CultureInfo.InvariantCulture);
            canvasCourse.name = "Class 17B";
             
            var canvasCourses = new List<CanvasCourse> {(canvasCourse)};

            var existingCourse = Mock.Create<Course>();
            
            existingCourse.AcademyId = "AFSNCOA";
            existingCourse.BbCourseId = "2";
            existingCourse.Id = 2;
            existingCourse.Name = "Class 17A";
            existingCourse.StartDate = DateTime.Now;
            existingCourse.GradDate = DateTime.Now.AddDays(12);

            var existingCourses = new List<Course>() { (existingCourse) };

            //Act
            var courseReconcile = CanvasBusinessLogic.ReconcileCourses(canvasCourses, existingCourses, "1");
            //Assert

            courseReconcile.Count().Should().Be(1);
            courseReconcile[0].BbCourseId.Should().Be("7016");
            courseReconcile[0].Name.Should().Be("Class 17B");

        }

        [TestMethod()]
        // public static List<Course> ReconcileCourses(List<CanvasCourse> canvasCoursesReturned, List<Course> existingCourses, string academyId)
        public void ReconcileCourses_CanvasCoursesSameAsExistingCourses_ReturnEmptyList()
        {
            //Arrange

            var canvasCourse = Mock.Create<CanvasCourse>();
            canvasCourse.id = 2;
            canvasCourse.account_id = 374;
            canvasCourse.start_at = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            canvasCourse.end_at = DateTime.Now.AddDays(12).ToString(CultureInfo.InvariantCulture);
            canvasCourse.name = "Class 17A";

            var canvasCourses = new List<CanvasCourse> { (canvasCourse) };

            var existingCourse = Mock.Create<Course>();
            
            existingCourse.AcademyId = "AFSNCOA";
            existingCourse.BbCourseId = "2";
            existingCourse.Id = 2;
            existingCourse.Name = "Class 17A";
            existingCourse.StartDate = DateTime.Now;
            existingCourse.GradDate = DateTime.Now.AddDays(12);
            var existingCourses = new List<Course>() { (existingCourse) };

            //Act
            var courseReconcile = CanvasBusinessLogic.ReconcileCourses(canvasCourses, existingCourses, "1");
            //Assert

            courseReconcile.Count().Should().Be(0);

        }
        [TestMethod()]
        // public static List<Course> ReconcileCourses(List<CanvasCourse> canvasCoursesReturned, List<Course> existingCourses, string academyId)
        public void ReconcileCourses_MoreExistingCoursesThanCanvasCourses_ReturnEmptyList()
        {
            //Arrange

            var canvasCourse = Mock.Create<CanvasCourse>();
            canvasCourse.id = 7016;
            canvasCourse.account_id = 374;
            canvasCourse.start_at = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            canvasCourse.end_at = DateTime.Now.AddDays(12).ToString(CultureInfo.InvariantCulture);
            canvasCourse.name = "Class 17B";

            var canvasCourses = new List<CanvasCourse> { (canvasCourse) };

            var existingCourse1 = Mock.Create<Course>();
            var existingCourse2 = Mock.Create<Course>();

            existingCourse1.AcademyId = "AFSNCOA";
            existingCourse1.BbCourseId = "2";
            existingCourse1.Id = 2;
            existingCourse1.Name = "Class 17A";
            existingCourse1.StartDate = DateTime.Now;
            existingCourse1.GradDate = DateTime.Now.AddDays(12);

            existingCourse2.AcademyId = "AFSNCOA";
            existingCourse2.BbCourseId = "7016";
            existingCourse2.Id = 7016;
            existingCourse2.Name = "Class 17B";
            existingCourse2.StartDate = DateTime.Now;
            existingCourse2.GradDate = DateTime.Now.AddDays(12);

            var existingCourses = new List<Course>() { existingCourse1, existingCourse2};
            

            //Act
            var courseReconcile = CanvasBusinessLogic.ReconcileCourses(canvasCourses, existingCourses, "1");
            //Assert

            courseReconcile.Count().Should().Be(0);

        }
    }
}