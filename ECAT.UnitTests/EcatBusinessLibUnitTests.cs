using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using FluentAssertions;
using Ecat.Business.Repositories;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Contexts;
using Breeze.ContextProvider.EF6;
using Telerik.JustMock.EntityFramework;

namespace ECAT.UnitTests
{
    public class LmsAdminCourseRepoBuilder
    {
       
        private EcatContext _context = EntityFrameworkMock.Create<EcatContext>();
        private BbWsCnet _bbws = Mock.Create<BbWsCnet>();

    }

    [TestClass]
    public class EcatBusinessLibUnitTests
    {
        [TestMethod]
        public void GetCourses_WithUserInCourses_ReturnCourses()
        {
            // Arrange
            //var courseOps = Mock.Create<CourseOps>();

            // Act

            // Assert

       
        }

        [TestMethod]
        public void GetCourses_WithOutUserInCourses_ReturnNoCourses()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void GetCourses_WithUserInCourses_RaisesException()
        {
            // Arrange

            // Act

            // Assert
        }

        

    }
}
