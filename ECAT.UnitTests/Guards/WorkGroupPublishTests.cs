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
using System.Web.Razor.Parser;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.Faculty;
using Ecat.Data.Models.User;
using Ecat.Data.Models.School;
using Ecat.Data.Static;
using Ecat.Business.Utilities;

namespace Ecat.Business.Guards.Tests
{

    public class CsvFacStratResponse : FacStratResponse
    {
        public static FacStratResponse FromCsv(string line)
        {
            var split = line.Split(',');

            FacStratResponse csvFacSratResponse = new FacStratResponse
            {
                AssesseePersonId = int.Parse(split[0]),
                CourseId = int.Parse(split[1]),
                WorkGroupId = int.Parse(split[2]),
                StratPosition = int.Parse(split[3]),
                FacultyPersonId = int.Parse(split[5]),
                ModifiedById = int.Parse(split[6]),
                ModifiedDate = DateTime.ParseExact(split[7], "M/dd/yyyy H:mm", Thread.CurrentThread.CurrentCulture)

            };

            return csvFacSratResponse;
        }
    }

    public class CsvStratResponse : StratResponse
    {
        public static StratResponse FromCsv(string line)
        {
            var split = line.Split(',');

            StratResponse csvStratResponse = new StratResponse
            {
                AssessorPersonId = int.Parse(split[0]),
                AssesseePersonId = int.Parse(split[1]),
                CourseId = int.Parse(split[2]),
                WorkGroupId = int.Parse(split[3]),
                StratPosition = int.Parse(split[4]),
                ModifiedById = int.Parse(split[5]),
                ModifiedDate = DateTime.ParseExact(split[6], "M/dd/yyyy H:mm", Thread.CurrentThread.CurrentCulture)

            };

            return csvStratResponse;
        }
    }

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

        private List<Person> _persons;
        private List<SpResponse> _spResponses;
        private List<WorkGroup> _workGroups;
        private List<FacStratResponse> _facStratResponses;
        private List<StratResponse> _stratResponses;

        public WorkGroupPublishBuilder(string personsFileName, string spResponsesFileName, string facStratResponsesFileName, string stratResponsesFileName)
        {
            _persons = CreatePersons(personsFileName);
            _spResponses = CreateSpResponses(spResponsesFileName);
            _workGroups = CreateWorkGroups();
            _facStratResponses = CreateFacStratResponses(facStratResponsesFileName);
            _stratResponses = CreateStratResponses(stratResponsesFileName);

        }

        public PubWg BuildPubWg(int peersDidNotAssessMe, int peersIdidNotAssess, int peersDidNotStratMe, int peersIdidNotStrat)
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

            var testPersons = new List<Person>();

            testPersons = _persons.Where(person =>
                _spResponses.Any(response => response.AssessorPersonId == person.PersonId)).ToList();

            testPersons.ForEach(person => {
                groupMembers.Add(CreatePubWgMember(person, peersDidNotAssessMe, peersIdidNotAssess, peersDidNotStratMe, peersIdidNotStrat));

            });

            pubWorkGroup.PubWgMembers = groupMembers;
      


            return pubWorkGroup;
        }

        public Dictionary<int, Dictionary<int, int>> BuildStratDictionary()
        {
            var stratDictionary = new Dictionary<int, Dictionary<int, int>>
            {
                //member id, dictionary of strat position count
                { 1, new Dictionary<int, int>
                    {
                        //position, strat position count
                        { 1, 1 },
                        { 3, 2 }
                    }
                }, 

                //member id, dictionary of strat position count
                { 3, new Dictionary<int, int>
                    {
                        //position, strat position count
                        { 2, 2 },
                        { 3, 1 }
                    }
                },

                //member id, dictionary of strat position count
                { 5, new Dictionary<int, int>
                    {
                        //position, strat position count
                        { 1, 1 },
                        { 2, 1 },
                        { 3, 1}
                    }
                },

                //member id, dictionary of strat position count
                { 12, new Dictionary<int, int>
                    {
                        //position, strat position count
                        { 1, 2 },
                        { 2, 1 }
                    }
                }

            };

            return stratDictionary;
        }

        private PubWgBreakOut CreatePubWgBreakOut(int personId, int workGroupId)
        {
            return new PubWgBreakOut
            {
                NotDisplayed = _spResponses
                    .Where(response => response.AssesseePersonId == personId && response.AssessorPersonId != personId &&  response.WorkGroupId == workGroupId)
                    .Count(response => response.MpItemResponse == MpSpItemResponse.Nd),
                IneffA = _spResponses
                    .Where(response => response.AssesseePersonId == personId && response.AssessorPersonId != personId && response.WorkGroupId == workGroupId)
                    .Count(response => response.MpItemResponse == MpSpItemResponse.Iea),
                IneffU = _spResponses
                    .Where(response => response.AssesseePersonId == personId && response.AssessorPersonId != personId && response.WorkGroupId == workGroupId)
                    .Count(response => response.MpItemResponse == MpSpItemResponse.Ieu),
                EffA = _spResponses
                    .Where(response => response.AssesseePersonId == personId && response.AssessorPersonId != personId && response.WorkGroupId == workGroupId)
                    .Count(response => response.MpItemResponse == MpSpItemResponse.Ea),
                EffU = _spResponses
                    .Where(response => response.AssesseePersonId == personId && response.AssessorPersonId != personId && response.WorkGroupId == workGroupId)
                    .Count(response => response.MpItemResponse == MpSpItemResponse.Eu),
                HighEffA = _spResponses
                    .Where(response => response.AssesseePersonId == personId && response.AssessorPersonId != personId && response.WorkGroupId == workGroupId)
                    .Count(response => response.MpItemResponse == MpSpItemResponse.Hea),
                HighEffU = _spResponses
                    .Where(response => response.AssesseePersonId == personId && response.AssessorPersonId != personId && response.WorkGroupId == workGroupId)
                    .Count(response => response.MpItemResponse == MpSpItemResponse.Heu),

            };
        }

        private List<FacStratResponse> CreateFacStratResponses(string fileName)
        {
            var csvFacStratRespones = File.ReadAllLines(GetPath(fileName))
                .Select(CsvFacStratResponse.FromCsv).ToList();


            return csvFacStratRespones;
        }

        private List<StratResponse> CreateStratResponses(string fileName)
        {
            var csvStatRespones = File.ReadAllLines(GetPath(fileName))
                .Select(CsvStratResponse.FromCsv).ToList();


            return csvStatRespones;
        }

        private List<WorkGroup> CreateWorkGroups()
        {

            var workGroups = new List<WorkGroup>();
            var wg = Mock.Create<WorkGroup>();
            

            wg.WorkGroupId = 1;
            wg.CourseId = 1;
            wg.WorkGroupId = 1;
            wg.MpCategory = "BC1";
            wg.GroupNumber = "1";
            wg.AssignedSpInstr = Mock.Create<SpInstrument>();

            workGroups.Add((wg));

            return workGroups;
        }

        private PubWgMember CreatePubWgMember(Person person, int peersDidNotAssessMe, int peersIdidNotAssess, int peersDidNotStratMe, int peersIdidNotStrat)
        {

        
            var pubWgMember = Mock.Create<PubWgMember>();

            pubWgMember.StudentId = person.PersonId;
            pubWgMember.Name = person.FirstName + person.LastName;
            //Count all responses given to member in workgroup. Filter out self assessments
            pubWgMember.CountSpResponses = _spResponses
                .Where(response => response.AssesseePersonId == person.PersonId && response.AssessorPersonId != person.PersonId && response.WorkGroupId == _workGroups[0].WorkGroupId)
                .Count(response => response.AssessorPersonId != person.PersonId);

            pubWgMember.SpResponseTotalScore = _spResponses
                .Where(response => response.AssesseePersonId == person.PersonId && response.AssessorPersonId != person.PersonId && response.WorkGroupId == _workGroups[0].WorkGroupId)
                .Sum(response => response.ItemModelScore );

            pubWgMember.FacStratPosition = _facStratResponses
                .Where(response => response.AssesseePersonId == person.PersonId)
                .Select(strat => strat.StratPosition).FirstOrDefault();

            pubWgMember.HasSpResult = false;

            pubWgMember.HasStratResult = false;

            pubWgMember.BreakOut = CreatePubWgBreakOut(person.PersonId, _workGroups[0].WorkGroupId);

            pubWgMember.SelfStratPosition = _stratResponses
                .Where(response => response.AssessorPersonId == person.PersonId && response.AssesseePersonId == person.PersonId &&
                                   response.WorkGroupId == _workGroups[0].WorkGroupId)
                .Select(strat => strat.StratPosition).FirstOrDefault();

            pubWgMember.PubStratResponses = _stratResponses
                .Where((strat =>
                    strat.AssessorPersonId == person.PersonId && strat.AssesseePersonId != person.PersonId && strat.WorkGroupId == _workGroups[0].WorkGroupId))
                .Select(strat => new PubStratResponse
                {
                    AssesseeId = strat.AssesseePersonId,
                    StratPosition = strat.StratPosition
                });


            if (peersDidNotAssessMe == 0) {
                pubWgMember.PeersDidNotAssessMe = new List<int> { };
            }
            else
            {
                var peerIds = new List<int>();
                var rnd = new Random();

                for (int i = 0; i < peersDidNotAssessMe; i++)
                {
                 peerIds.Add(rnd.Next(1, 10));   
                }

                pubWgMember.PeersDidNotAssessMe = peerIds;
            }



            if (peersIdidNotAssess == 0)
            {
                pubWgMember.PeersIdidNotAssess = new List<int> { };
            }
            else
            {
                var peerIds = new List<int>();
                var rnd = new Random();

                for (int i = 0; i < peersIdidNotAssess; i++)
                {
                    peerIds.Add(rnd.Next(1, 10));
                }

                pubWgMember.PeersIdidNotAssess = peerIds;
            }

            if (peersDidNotStratMe == 0)
            {
                pubWgMember.PeersDidNotStratMe = new List<int> { };
            }
            else
            {
                var peerIds = new List<int>();
                var rnd = new Random();

                for (int i = 0; i < peersDidNotStratMe; i++)
                {
                    peerIds.Add(rnd.Next(1, 10));
                }

                pubWgMember.PeersDidNotStratMe = peerIds;
            }

            if (peersIdidNotStrat == 0)
            {
                pubWgMember.PeersIdidNotStrat = new List<int> { };
            }
            else
            {
                var peerIds = new List<int>();
                var rnd = new Random();

                for (int i = 0; i < peersIdidNotStrat; i++)
                {
                    peerIds.Add(rnd.Next(1, 10));
                }

                pubWgMember.PeersIdidNotStrat = peerIds;
            }

            return pubWgMember;

        }

        private List<SpResponse> CreateSpResponses(string fileName)
        {

            var csvSpRespones = File.ReadAllLines(GetPath(fileName))
                .Select(CsvSpResponse.FromCsv).ToList();

   
            return csvSpRespones;
        }

        private List<Person> CreatePersons(string fileName) {

            var csvPersons = File.ReadAllLines(GetPath(fileName))
                .Select(CsvPerson.FromCsv).ToList();


            return csvPersons;

        }

        private static string GetPath(string fileName) {

            var currentDirectory = Directory.GetCurrentDirectory();
            return Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\DataSets\", fileName));
        }

    }

    [TestClass()]
    public class WorkGroupPublishTests
    {

        [TestMethod()]
        public void CalculateResults_NotAllWorkGroupsArePublishiable_ThrowWorkGroupPublishException()
        {
            // Arrange

            var workGroup = new WorkGroupPublishBuilder("Persons.csv", "SpResponses.csv", "FacStratResponses.csv", "StratResponses.csv");

            var pubWorkGroup = workGroup.BuildPubWg(0, 2, 0, 0);
            var pubWgs = new List<PubWg> { pubWorkGroup };
            var svrWgIds = new List<int> { 1, 2, 3 };

            // Act

            Action action = () => WorkGroupPublish.CalculateResults(pubWgs, svrWgIds, 1);

            // Assert
            action.Should().Throw<WorkGroupPublishException>();
        }

        [TestMethod()]
        public void CalculateResults_MembersMissingPublishData_MemberMissingPublishDataException()
        {
            // Arrange

            var workGroup = new WorkGroupPublishBuilder("Persons.csv", "SpResponses.csv", "FacStratResponses.csv", "StratResponses.csv");

            var pubWorkGroup = workGroup.BuildPubWg(0, 2, 0, 0);
            var pubWgs = new List<PubWg> { pubWorkGroup };
            var svrWgIds = new List<int> { 1 };

            // Act

            Action action = () => WorkGroupPublish.CalculateResults(pubWgs, svrWgIds, 1);

            // Assert
            action.Should().Throw<MemberMissingPublishDataException>();
        }

        // [TestMethod()]
        // public void RecieveWorkGroup_ReturnCorrectAssesseeStratDictionaryForGroup()
        // {
        //     // Arrange

        //     var workGroup = new WorkGroupPublishBuilder("Persons.csv", "SpResponses.csv", "FacStratResponses.csv", "StratResponses.csv");

        //     var pubWorkGroup = workGroup.BuildPubWg(0, 2, 0, 0);

        //     // Act

        //     var dictionary = new Dictionary<int, Dictionary<int, int>>();
        //     var correctDictionary = workGroup.BuildStratDictionary();

        //     dictionary = WorkGroupPublish.CreateAssesseeStratDictionaryForGroup(pubWorkGroup);


        //     // Assert

        //     //check that the dictonary is the right length
        //     dictionary.Count().Should().Be(4);
        //     dictionary[1].Count().Should().Be(2);
        //     dictionary[3].Count().Should().Be(2);
        //     dictionary[5].Count().Should().Be(3);
        //     dictionary[12].Count().Should().Be(2);

        //     //Check that the values are correct
        //     dictionary.Should().BeEquivalentTo(correctDictionary);

        // }

        // [TestMethod]
        // public void RecievePubWgMemberandTotalStratPos_MemberIsNotMissingData_ReturnTrue()
        // {
        //     // Arrange
        //     var workGroup = new WorkGroupPublishBuilder("Persons.csv", "SpResponses.csv", "FacStratResponses.csv", "StratResponses.csv");

        //     var pubWorkGroup = workGroup.BuildPubWg(0, 0, 0, 0);
        //     var groupMemberWithMissingData = pubWorkGroup.PubWgMembers.ToList()[0];
        //     var countOfGrp = pubWorkGroup.PubWgMembers.Count();
        //     var totalStratPos = 0;


        //     for (var i = 1; i <= countOfGrp; i++)
        //     {
        //         totalStratPos += i;
        //     }

        //     // Act

        //     var result = WorkGroupPublish.MemberHasAllData(groupMemberWithMissingData, totalStratPos);

        //     // Assert

        //     result.Should().BeTrue();

        // }

        // [TestMethod]
        // public void RecievePubWgMemberandTotalStratPos_MemberIsMissingData_ReturnFalse()
        // {
        //     // Arrange
        //     var workGroup = new WorkGroupPublishBuilder("Persons.csv", "SpResponses.csv", "FacStratResponses.csv", "StratResponses.csv");

        //     var pubWorkGroup = workGroup.BuildPubWg(2, 3, 2, 2);
        //     var groupMemberWithMissingData = pubWorkGroup.PubWgMembers.ToList()[0];
        //     var countOfGrp = pubWorkGroup.PubWgMembers.Count();
        //     var totalStratPos = 0;


        //     for (var i = 1; i <= countOfGrp; i++)
        //     {
        //         totalStratPos += i;
        //     }

        //     // Act

        //     var result = WorkGroupPublish.MemberHasAllData(groupMemberWithMissingData, totalStratPos);

        //     // Assert

        //     result.Should().BeFalse();


        // }

        // [TestMethod]
        // public void RecievePubWgMemberandTotalStratPos_MemberIsMissingDataFacStrat_ReturnTrue()
        // {
        //     // Arrange
        //     var workGroup = new WorkGroupPublishBuilder("Persons.csv", "SpResponses.csv", "FacStratResponsesWithMissingData.csv", "StratResponses.csv");

        //     var pubWorkGroup = workGroup.BuildPubWg(0, 0, 0, 0);
        //     var groupMemberWithMissingData = pubWorkGroup.PubWgMembers.ToList()[1];
        //     var countOfGrp = pubWorkGroup.PubWgMembers.Count();
        //     var totalStratPos = 0;


        //     for (var i = 1; i <= countOfGrp; i++)
        //     {
        //         totalStratPos += i;
        //     }

        //     // Act

        //     var result = WorkGroupPublish.MemberHasAllData(groupMemberWithMissingData, totalStratPos);

        //     // Assert

        //     result.Should().BeTrue();


        // }

        //[TestMethod]
        // //internal static StratResult CreateStratResultForMember(PubWgMember member, decimal stratScoreInterval,  int courseId, int workGroupId, int loggedInPersonId)
        // public void CreateStratResultForMember_ReturnCorrectStratResultForMember()
        // {
        //     //Arrange
        //     var workGroup = new WorkGroupPublishBuilder("Persons.csv", "SpResponses.csv",
        //         "FacStratResponsesWithMissingData.csv", "StratResponses.csv");
        //     var pubWorkGroup = workGroup.BuildPubWg(0, 0, 0, 0);
        //     var members = pubWorkGroup.PubWgMembers;
        //     var stratDictionary = workGroup.BuildStratDictionary();

        //     var stratScoreInterval = 1m / members.Count();
        //     stratScoreInterval = decimal.Round(stratScoreInterval, 4);

        //     foreach (var gm in members)
        //     {
        //         var myResponses = stratDictionary[gm.StudentId];
        //         gm.StratTable = myResponses.Select(mr => new PubWgStratTable
        //         {
        //             Position = mr.Key,
        //             Count = mr.Value
        //         });
        //     }

        //     //Act

        //     foreach (var pubWgMember in members)
        //     {
        //         var stratResult = WorkGroupPublish.CreateStratResultForMember(pubWgMember, stratScoreInterval, 1, 1, 1);

        //         pubWgMember.StratResult = stratResult;
        //     }

        //     //Assert
        // }

        // [TestMethod]
        // //internal static SpResult CreateSpResultForMember(PubWgMember member, int countOfGrp, int courseId, int workGroupId, int? instrumentId)
        // public void CreateSpResultForMember_ReturnCorrectSpResultForMember()
        // {
        //     //Arrange

        //     //Act

        //     //Assert
        // }

        // [TestMethod]
        // //internal static PubWgMember CalculateAwardedScoreForMember(PubWgMember member, PubWg wg, decimal spInterval, decimal facInterval, int fi )
        // public void CalculateAwardedScoreForMember_ReturnCorrectScoreForMember()
        // {
        //     //Arrange
        //     //var workGroup = new WorkGroupPublishBuilder("Persons.csv", "SpResponses.csv", "FacStratResponses.csv", "StratResponses.csv");
        //     //var pubWorkGroup = workGroup.BuildPubWg(0, 0, 0, 0);
        //     //var membersStratKeeper = new List<PubWgMember>();

        //     //var groupMembers = pubWorkGroup.PubWgMembers;

        //     //foreach (var pubWgMember in groupMembers)
        //     //{
        //     //    var stratResult = WorkGroupPublish.CreateStratResultForMember(pubWgMember, stratScoreInterval, wg.CourseId, wg.Id, loggedInPersonId);

        //     //    member.StratResult = stratResult;
        //     //    membersStratKeeper.Add(member)

        //     //}



        //     //var fi = 0;

        //     //var correctFacStratList = new Dictionary<int, decimal>
        //     //{
        //     //    { 1, 10},
        //     //    { 3, 9},
        //     //    { 5, 8},
        //     //    { 12, 7}
        //     //};

        //     ////Act



        //     //foreach (var pubWgMember in groupMembers)
        //     //{
        //     //    //WgSpTopStrat = 10
        //     //    //WgFacTopStrat = 10
        //     //    WorkGroupPublish.CalculateAwardedScoreForMember(pubWgMember, pubWorkGroup, 1, 1, fi);

        //     //    fi += 1;
        //     //}

        //     ////Assert
        //     //foreach (var pubWgMember in groupMembers)
        //     //{
        //     //    pubWgMember.StratResult.FacStratAwardedScore.Should().Be(correctFacStratList[pubWgMember.StudentId]);
        //     //}
        // }

    }
}