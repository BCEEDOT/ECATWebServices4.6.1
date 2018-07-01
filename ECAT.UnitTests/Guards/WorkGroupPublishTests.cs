using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ecat.Business.Guards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Business.BbWs.BbCourseMembership;
using Ecat.Data.Models.Common;
using Ecat.Data.Models.Student;
using FluentAssertions;
using Telerik.JustMock;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Reflection;
using System.Globalization;
using System.Threading;
using Ecat.Data.Models.User;

namespace Ecat.Business.Guards.Tests
{
    public class CsvSpResponse : SpResponse
    {

       
        public static SpResponse FromCsv(string line)
        {
            var split = line.Split(',');

            SpResponse csvSpResponse = new SpResponse
            {
                AssessorPersonId = int.Parse(split[0]),
                AssesseePersonId = int.Parse(split[1]),
                CourseId = int.Parse(split[2]),
                WorkGroupId = int.Parse(split[3]),
                InventoryItemId = int.Parse(split[4]),
                MpItemResponse = split[5],
                ItemModelScore = int.Parse(split[6]),
                ModifiedById = int.Parse(split[7]),
          
                ModifiedDate = DateTime.ParseExact(split[8], "M/dd/yyyy H:mm", Thread.CurrentThread.CurrentCulture)
        };


            return csvSpResponse;
        }
    }


    public class CsvPerson : Person {

        public static Person FromCsv(string line) {

            var split = line.Split(',');

            Person csvPerson = new Person {
                PersonId = int.Parse(split[0]),
                IsActive = bool.Parse(split[1]),
                LastName = split[4],
                FirstName = split[5]
            };

            return csvPerson;
        }
    }

    public class WorkGroupPublishBuilder
    {

        public List<Person> Persons;
        public List<SpResponse> SpResponses;


        public WorkGroupPublishBuilder(string personsFileName, string spResponsesFileName)
        {
            Persons = createPersons(personsFileName);
            SpResponses = CreateSpResponses(spResponsesFileName);

        }


        public PubWg BuildPubWg()
        {
            var pubWorkGroup = Mock.Create<PubWg>();

            pubWorkGroup.Id = 1;
            pubWorkGroup.CourseId = 1;
            //AssignedSpInstr.InventoryCollection.Count;
            pubWorkGroup.CountInventory = 10;
            pubWorkGroup.InstrumentId = 1;
            pubWorkGroup.WgSpTopStrat = 10;
            pubWorkGroup.WgFacTopStrat = 10;
            pubWorkGroup.StratDivisor = 10;


            //Made from CrseStudentInGroup
            var groupMembers = new List<PubWgMember>();

            Persons.ForEach(person => {
                groupMembers.Add(CreatePubWgMember(person));

            });

            pubWorkGroup.PubWgMembers = groupMembers;
      


            return pubWorkGroup;
        }

        //private PubWgBreakOut CreatePubWgBreakOut()
        //{
        //    return new PubWgBreakOut
        //    {
        //        NotDisplayed = gm.AssesseeSpResponses
        //            .Where(response => response.AssessorPersonId != response.AssesseePersonId)
        //            .Count(response => response.MpItemResponse == MpSpItemResponse.Nd),
        //        IneffA = gm.AssesseeSpResponses
        //            .Where(response => response.AssessorPersonId != response.AssesseePersonId)
        //            .Count(response => response.MpItemResponse == MpSpItemResponse.Iea),
        //        IneffU = gm.AssesseeSpResponses
        //            .Where(response => response.AssessorPersonId != response.AssesseePersonId)
        //            .Count(response => response.MpItemResponse == MpSpItemResponse.Ieu),
        //        EffA = gm.AssesseeSpResponses
        //            .Where(response => response.AssessorPersonId != response.AssesseePersonId)
        //            .Count(response => response.MpItemResponse == MpSpItemResponse.Ea),
        //        EffU = gm.AssesseeSpResponses
        //            .Where(response => response.AssessorPersonId != response.AssesseePersonId)
        //            .Count(response => response.MpItemResponse == MpSpItemResponse.Eu),
        //        HighEffA = gm.AssesseeSpResponses
        //            .Where(response => response.AssessorPersonId != response.AssesseePersonId)
        //            .Count(response => response.MpItemResponse == MpSpItemResponse.Hea),
        //        HighEffU = gm.AssesseeSpResponses
        //            .Where(response => response.AssessorPersonId != response.AssesseePersonId)
        //            .Count(response => response.MpItemResponse == MpSpItemResponse.Heu)
        //    };
        //}

        private PubWgMember CreatePubWgMember(Person person)
        {

        
            var pubWgMember = Mock.Create<PubWgMember>();

            pubWgMember.StudentId = person.PersonId;
            pubWgMember.Name = person.FirstName + person.LastName;

            //pubWgMember.CountSpResponses = gm.AssesseeSpResponses
            //            .Count(response => response.AssessorPersonId != gm.StudentId),

            //pubWgMember.SpResponseTotalScore = gm.AssesseeSpResponses
            //            .Where(response => response.AssessorPersonId != gm.StudentId)
            //            .Sum(response => response.ItemModelScore),

            //pubWgMember.FacStratPosition = facStratPosition;

            //pubWgMember.HasSpResult = wg.SpResults.Any(result => result.StudentId == gm.StudentId),

            //pubWgMember.HasStratResult = wg.SpStratResults.Any(result => result.StudentId == gm.StudentId),

            //pubWgMember.BreakOut = CreatePubWgBreakOut();

            //pubWgMember.SelfStratPosition = gm.AssesseeStratResponse
            //    .Where(strat => strat.AssessorPersonId == gm.StudentId)
            //    .Select(strat => strat.StratPosition).FirstOrDefault();

            //pubWgMember.PubStratResponses =
            //    gm.AssessorStratResponse.Where(strat => strat.AssesseePersonId != gm.StudentId)
            //        .Select(strat => new PubStratResponse
            //        {
            //            AssesseeId = strat.AssesseePersonId,
            //            StratPosition = strat.StratPosition
            //        });

            //pubWgMember.PeersDidNotAssessMe = prundedGm.Where(peer => peer.AssessorSpResponses
            //                                                              .Count(response =>
            //                                                                  response.AssesseePersonId ==
            //                                                                  gm.StudentId) == 0)
            //    .Select(peer => peer.StudentId);

            //pubWgMember.PeersIdidNotAssess = prundedGm.Where(peer => peer.AssesseeSpResponses
            //                                                             .Count(response =>
            //                                                                 response.AssessorPersonId ==
            //                                                                 gm.StudentId) == 0)
            //    .Select(peer => peer.StudentId);

            //pubWgMember.PeersDidNotStratMe = prundedGm.Where(peer => peer.AssessorStratResponse
            //                                                 .Count(strat =>
            //                                                     strat.AssesseePersonId == gm.StudentId) == 0)
            //    .Select(peer => peer.StudentId);

            //pubWgMember.PeersIdidNotStrat = prundedGm.Where(peer => peer.AssesseeStratResponse
            //                                                .Count(strat =>
            //                                                    strat.AssessorPersonId == gm.StudentId) == 0)
            //    .Select(peer => peer.StudentId);

            return pubWgMember;

        }

        private List<SpResponse> CreateSpResponses(string fileName)
        {

            var csvSpRespones = File.ReadAllLines(getPath(fileName))
                .Select(CsvSpResponse.FromCsv).ToList();

   
            return csvSpRespones;
        }

        private List<Person> createPersons(string fileName) {

            var csvPersons = File.ReadAllLines(getPath(fileName))
                .Select(CsvPerson.FromCsv).ToList();


            return csvPersons;

        }

        private string getPath(string fileName) {

            string currentDirectory = Directory.GetCurrentDirectory();
            return Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\DataSets\", fileName));
        }

        //private ICollection<SpResponse> CreateStratResponses()
        //{

        //}


        



    }

    


    [TestClass()]
    public class WorkGroupPublishTests
    {
        [TestMethod()]
        public void RecieveWorkGroups_AllWorkGroupsArePublishiable_ReturnTrue()
        {

            // Arrange

            var workGroup = new WorkGroupPublishBuilder("Persons.csv", "SpResponses.csv");

            var pubWorkGroup = workGroup.BuildPubWg();




            // Act

            //Guards.WorkGroupPublish.AllWorkGroupsArePublished()

            // Assert
            string username = "dennis";
            Assert.AreEqual("dennis", "jones");
            username.Should().Be("jones");
        }

        [TestMethod()]
        public void RecieveWorkGroups_NotAllWorkGroupsArePublishiable_ThrowEntityErrorsException()
        {
            // Arrange

            // Act

            // Assert
            string username = "dennis";
            username.Should().Be("dennis");
        }

        [TestMethod()]
        public void RecieveWorkGroup_ReturnCorrectAssesseeStratDictionaryForGroup()
        {
            // Arrange

            // Act

            // Assert
            string username = "dennis";
            username.Should().Be("dennis");
        }
    }
}