using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
//using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Data.Models.Designer;
//using Ecat.Shared.Core.ModelLibrary.Cognitive;
//using Ecat.Shared.Core.Provider;
//using Ecat.Shared.Core.Utility;
using Ecat.Business.Utilities;
//using Ecat.Shared.DbMgr.Context;
using Ecat.Data.Contexts;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Static;
using Ecat.Business.Guards;
using LtiLibrary.Core.Lti1;
using Newtonsoft.Json.Linq;

namespace Ecat.Business.Repositories
{
    public class UserLogic : IUserRepo
    {
        public Person User { get; set; }

        private readonly EFContextProvider<EcatContext> _efCtx;
    

        public UserLogic(EFContextProvider<EcatContext> efCtx)
        {
            _efCtx = efCtx;
        }

        public string Metadata
        {
            get
            {
                       //return  @"{""schema"":{""namespace"":""Ecat.Data.Contexts"",""alias"":""Self"",""annotation:UseStrongSpatialTypes"":""false"",""xmlns:annotation"":""http://schemas.microsoft.com/ado/2009/02/edm/annotation"",""xmlns:customannotation"":""http://schemas.microsoft.com/ado/2013/11/edm/customannotation"",""xmlns"":""http://schemas.microsoft.com/ado/2009/11/edm"",""cSpaceOSpaceMapping"":""[[\""Ecat.Data.Contexts.CogEcmspeResult\"",\""Ecat.Data.Models.Cognitive.CogEcmspeResult\""],[\""Ecat.Data.Contexts.CogInstrument\"",\""Ecat.Data.Models.Designer.CogInstrument\""],[\""Ecat.Data.Contexts.CogInventory\"",\""Ecat.Data.Models.Designer.CogInventory\""],[\""Ecat.Data.Contexts.Person\"",\""Ecat.Data.Models.User.Person\""],[\""Ecat.Data.Contexts.ProfileFaculty\"",\""Ecat.Data.Models.User.ProfileFaculty\""],[\""Ecat.Data.Contexts.RoadRunner\"",\""Ecat.Data.Models.User.RoadRunner\""],[\""Ecat.Data.Contexts.Security\"",\""Ecat.Data.Models.User.Security\""],[\""Ecat.Data.Contexts.ProfileStudent\"",\""Ecat.Data.Models.User.ProfileStudent\""],[\""Ecat.Data.Contexts.CogEcpeResult\"",\""Ecat.Data.Models.Cognitive.CogEcpeResult\""],[\""Ecat.Data.Contexts.CogEsalbResult\"",\""Ecat.Data.Models.Cognitive.CogEsalbResult\""],[\""Ecat.Data.Contexts.CogEtmpreResult\"",\""Ecat.Data.Models.Cognitive.CogEtmpreResult\""],[\""Ecat.Data.Contexts.CogResponse\"",\""Ecat.Data.Models.Cognitive.CogResponse\""]]"",""entityType"":[{""name"":""CogEcmspeResult"",""customannotation:ClrType"":""Ecat.Data.Models.Cognitive.CogEcmspeResult, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":[{""name"":""PersonId""},{""name"":""InstrumentId""},{""name"":""Attempt""}]},""property"":[{""name"":""PersonId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""InstrumentId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Attempt"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Accommodate"",""type"":""Edm.Double"",""nullable"":""false""},{""name"":""Avoid"",""type"":""Edm.Double"",""nullable"":""false""},{""name"":""Collaborate"",""type"":""Edm.Double"",""nullable"":""false""},{""name"":""Compete"",""type"":""Edm.Double"",""nullable"":""false""},{""name"":""Compromise"",""type"":""Edm.Double"",""nullable"":""false""}],""navigationProperty"":[{""name"":""Instrument"",""relationship"":""Self.CogEcmspeResult_Instrument"",""fromRole"":""CogEcmspeResult_Instrument_Source"",""toRole"":""CogEcmspeResult_Instrument_Target""},{""name"":""Person"",""relationship"":""Self.CogEcmspeResult_Person"",""fromRole"":""CogEcmspeResult_Person_Source"",""toRole"":""CogEcmspeResult_Person_Target""}]},{""name"":""CogInstrument"",""customannotation:ClrType"":""Ecat.Data.Models.Designer.CogInstrument, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":{""name"":""Id""}},""property"":[{""name"":""Id"",""type"":""Edm.Int32"",""nullable"":""false"",""annotation:StoreGeneratedPattern"":""Identity""},{""name"":""Version"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""IsActive"",""type"":""Edm.Boolean"",""nullable"":""false""},{""name"":""CogInstructions"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""MpCogInstrumentType"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""ModifiedById"",""type"":""Edm.Int32""},{""name"":""ModifiedDate"",""type"":""Edm.DateTime""}],""navigationProperty"":{""name"":""InventoryCollection"",""relationship"":""Self.CogInventory_Instrument"",""fromRole"":""CogInventory_Instrument_Target"",""toRole"":""CogInventory_Instrument_Source""}},{""name"":""CogInventory"",""customannotation:ClrType"":""Ecat.Data.Models.Designer.CogInventory, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":{""name"":""Id""}},""property"":[{""name"":""Id"",""type"":""Edm.Int32"",""nullable"":""false"",""annotation:StoreGeneratedPattern"":""Identity""},{""name"":""InstrumentId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""DisplayOrder"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""IsScored"",""type"":""Edm.Boolean"",""nullable"":""false""},{""name"":""IsDisplayed"",""type"":""Edm.Boolean"",""nullable"":""false""},{""name"":""AdaptiveDescription"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""InnovativeDescription"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""ItemType"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""ItemDescription"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""IsReversed"",""type"":""Edm.Boolean""},{""name"":""ModifiedById"",""type"":""Edm.Int32""},{""name"":""ModifiedDate"",""type"":""Edm.DateTime""}],""navigationProperty"":{""name"":""Instrument"",""relationship"":""Self.CogInventory_Instrument"",""fromRole"":""CogInventory_Instrument_Source"",""toRole"":""CogInventory_Instrument_Target""}},{""name"":""Person"",""customannotation:ClrType"":""Ecat.Data.Models.User.Person, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":{""name"":""PersonId""}},""property"":[{""name"":""PersonId"",""type"":""Edm.Int32"",""nullable"":""false"",""annotation:StoreGeneratedPattern"":""Identity""},{""name"":""IsActive"",""type"":""Edm.Boolean"",""nullable"":""false""},{""name"":""BbUserId"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""BbUserName"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""LastName"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true"",""nullable"":""false""},{""name"":""FirstName"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true"",""nullable"":""false""},{""name"":""AvatarLocation"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""GoByName"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""MpGender"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true"",""nullable"":""false""},{""name"":""MpAffiliation"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true"",""nullable"":""false""},{""name"":""MpPaygrade"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true"",""nullable"":""false""},{""name"":""MpComponent"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true"",""nullable"":""false""},{""name"":""Email"",""type"":""Edm.String"",""maxLength"":""80"",""fixedLength"":""false"",""unicode"":""true"",""nullable"":""false""},{""name"":""RegistrationComplete"",""type"":""Edm.Boolean"",""nullable"":""false""},{""name"":""MpInstituteRole"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true"",""nullable"":""false""},{""name"":""ModifiedById"",""type"":""Edm.Int32""},{""name"":""ModifiedDate"",""type"":""Edm.DateTime""}],""navigationProperty"":[{""name"":""Faculty"",""relationship"":""Self.Person_Faculty"",""fromRole"":""Person_Faculty_Source"",""toRole"":""Person_Faculty_Target""},{""name"":""RoadRunnerAddresses"",""relationship"":""Self.RoadRunner_Person"",""fromRole"":""RoadRunner_Person_Target"",""toRole"":""RoadRunner_Person_Source""},{""name"":""Security"",""relationship"":""Self.Security_Person"",""fromRole"":""Security_Person_Target"",""toRole"":""Security_Person_Source""},{""name"":""Student"",""relationship"":""Self.Person_Student"",""fromRole"":""Person_Student_Source"",""toRole"":""Person_Student_Target""}]},{""name"":""ProfileFaculty"",""customannotation:ClrType"":""Ecat.Data.Models.User.ProfileFaculty, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":{""name"":""PersonId""}},""property"":[{""name"":""PersonId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Bio"",""type"":""Edm.String"",""maxLength"":""6000"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""HomeStation"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""IsCourseAdmin"",""type"":""Edm.Boolean"",""nullable"":""false""},{""name"":""IsReportViewer"",""type"":""Edm.Boolean"",""nullable"":""false""},{""name"":""AcademyId"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""}],""navigationProperty"":{""name"":""Person"",""relationship"":""Self.Person_Faculty"",""fromRole"":""Person_Faculty_Target"",""toRole"":""Person_Faculty_Source""}},{""name"":""RoadRunner"",""customannotation:ClrType"":""Ecat.Data.Models.User.RoadRunner, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":{""name"":""Id""}},""property"":[{""name"":""Id"",""type"":""Edm.Int32"",""nullable"":""false"",""annotation:StoreGeneratedPattern"":""Identity""},{""name"":""Location"",""type"":""Edm.String"",""maxLength"":""200"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""PhoneNumber"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""LeaveDate"",""type"":""Edm.DateTime"",""nullable"":""false""},{""name"":""ReturnDate"",""type"":""Edm.DateTime"",""nullable"":""false""},{""name"":""SignOut"",""type"":""Edm.Boolean"",""nullable"":""false""},{""name"":""PrevSignOut"",""type"":""Edm.Boolean"",""nullable"":""false""},{""name"":""PersonId"",""type"":""Edm.Int32"",""nullable"":""false""}],""navigationProperty"":{""name"":""Person"",""relationship"":""Self.RoadRunner_Person"",""fromRole"":""RoadRunner_Person_Source"",""toRole"":""RoadRunner_Person_Target""}},{""name"":""Security"",""customannotation:ClrType"":""Ecat.Data.Models.User.Security, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":{""name"":""PersonId""}},""property"":[{""name"":""PersonId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""BadPasswordCount"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""PasswordHash"",""type"":""Edm.String"",""maxLength"":""400"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""ModifiedById"",""type"":""Edm.Int32""},{""name"":""ModifiedDate"",""type"":""Edm.DateTime""}],""navigationProperty"":{""name"":""Person"",""relationship"":""Self.Security_Person"",""fromRole"":""Security_Person_Source"",""toRole"":""Security_Person_Target""}},{""name"":""ProfileStudent"",""customannotation:ClrType"":""Ecat.Data.Models.User.ProfileStudent, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":{""name"":""PersonId""}},""property"":[{""name"":""PersonId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Bio"",""type"":""Edm.String"",""maxLength"":""6000"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""HomeStation"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""ContactNumber"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""Commander"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""Shirt"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""CommanderEmail"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""},{""name"":""ShirtEmail"",""type"":""Edm.String"",""maxLength"":""Max"",""fixedLength"":""false"",""unicode"":""true""}],""navigationProperty"":{""name"":""Person"",""relationship"":""Self.Person_Student"",""fromRole"":""Person_Student_Target"",""toRole"":""Person_Student_Source""}},{""name"":""CogEcpeResult"",""customannotation:ClrType"":""Ecat.Data.Models.Cognitive.CogEcpeResult, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":[{""name"":""PersonId""},{""name"":""InstrumentId""},{""name"":""Attempt""}]},""property"":[{""name"":""PersonId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""InstrumentId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Attempt"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Outcome"",""type"":""Edm.Int32"",""nullable"":""false""}],""navigationProperty"":[{""name"":""Instrument"",""relationship"":""Self.CogEcpeResult_Instrument"",""fromRole"":""CogEcpeResult_Instrument_Source"",""toRole"":""CogEcpeResult_Instrument_Target""},{""name"":""Person"",""relationship"":""Self.CogEcpeResult_Person"",""fromRole"":""CogEcpeResult_Person_Source"",""toRole"":""CogEcpeResult_Person_Target""}]},{""name"":""CogEsalbResult"",""customannotation:ClrType"":""Ecat.Data.Models.Cognitive.CogEsalbResult, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":[{""name"":""PersonId""},{""name"":""InstrumentId""},{""name"":""Attempt""}]},""property"":[{""name"":""PersonId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""InstrumentId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Attempt"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""LaissezFaire"",""type"":""Edm.Double"",""nullable"":""false""},{""name"":""Contingent"",""type"":""Edm.Double"",""nullable"":""false""},{""name"":""Management"",""type"":""Edm.Double"",""nullable"":""false""},{""name"":""Idealized"",""type"":""Edm.Double"",""nullable"":""false""},{""name"":""Individual"",""type"":""Edm.Double"",""nullable"":""false""},{""name"":""Inspirational"",""type"":""Edm.Double"",""nullable"":""false""},{""name"":""IntellectualStim"",""type"":""Edm.Double"",""nullable"":""false""}],""navigationProperty"":[{""name"":""Instrument"",""relationship"":""Self.CogEsalbResult_Instrument"",""fromRole"":""CogEsalbResult_Instrument_Source"",""toRole"":""CogEsalbResult_Instrument_Target""},{""name"":""Person"",""relationship"":""Self.CogEsalbResult_Person"",""fromRole"":""CogEsalbResult_Person_Source"",""toRole"":""CogEsalbResult_Person_Target""}]},{""name"":""CogEtmpreResult"",""customannotation:ClrType"":""Ecat.Data.Models.Cognitive.CogEtmpreResult, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":[{""name"":""PersonId""},{""name"":""InstrumentId""},{""name"":""Attempt""}]},""property"":[{""name"":""PersonId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""InstrumentId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Attempt"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Creator"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Advancer"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Refiner"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Executor"",""type"":""Edm.Int32"",""nullable"":""false""}],""navigationProperty"":[{""name"":""Instrument"",""relationship"":""Self.CogEtmpreResult_Instrument"",""fromRole"":""CogEtmpreResult_Instrument_Source"",""toRole"":""CogEtmpreResult_Instrument_Target""},{""name"":""Person"",""relationship"":""Self.CogEtmpreResult_Person"",""fromRole"":""CogEtmpreResult_Person_Source"",""toRole"":""CogEtmpreResult_Person_Target""}]},{""name"":""CogResponse"",""customannotation:ClrType"":""Ecat.Data.Models.Cognitive.CogResponse, Ecat.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""key"":{""propertyRef"":[{""name"":""CogInventoryId""},{""name"":""PersonId""},{""name"":""Attempt""}]},""property"":[{""name"":""CogInventoryId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""PersonId"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""Attempt"",""type"":""Edm.Int32"",""nullable"":""false""},{""name"":""ItemScore"",""type"":""Edm.Double"",""nullable"":""false""}],""navigationProperty"":[{""name"":""InventoryItem"",""relationship"":""Self.CogResponse_InventoryItem"",""fromRole"":""CogResponse_InventoryItem_Source"",""toRole"":""CogResponse_InventoryItem_Target""},{""name"":""Person"",""relationship"":""Self.CogResponse_Person"",""fromRole"":""CogResponse_Person_Source"",""toRole"":""CogResponse_Person_Target""}]}],""association"":[{""name"":""CogInventory_Instrument"",""end"":[{""role"":""CogInventory_Instrument_Source"",""type"":""Edm.Self.CogInventory"",""multiplicity"":""*""},{""role"":""CogInventory_Instrument_Target"",""type"":""Edm.Self.CogInstrument"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogInventory_Instrument_Target"",""propertyRef"":{""name"":""Id""}},""dependent"":{""role"":""CogInventory_Instrument_Source"",""propertyRef"":{""name"":""InstrumentId""}}}},{""name"":""CogEcmspeResult_Instrument"",""end"":[{""role"":""CogEcmspeResult_Instrument_Source"",""type"":""Edm.Self.CogEcmspeResult"",""multiplicity"":""*""},{""role"":""CogEcmspeResult_Instrument_Target"",""type"":""Edm.Self.CogInstrument"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogEcmspeResult_Instrument_Target"",""propertyRef"":{""name"":""Id""}},""dependent"":{""role"":""CogEcmspeResult_Instrument_Source"",""propertyRef"":{""name"":""InstrumentId""}}}},{""name"":""Person_Faculty"",""end"":[{""role"":""Person_Faculty_Source"",""type"":""Edm.Self.Person"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}},{""role"":""Person_Faculty_Target"",""type"":""Edm.Self.ProfileFaculty"",""multiplicity"":""0..1""}],""referentialConstraint"":{""principal"":{""role"":""Person_Faculty_Source"",""propertyRef"":{""name"":""PersonId""}},""dependent"":{""role"":""Person_Faculty_Target"",""propertyRef"":{""name"":""PersonId""}}}},{""name"":""RoadRunner_Person"",""end"":[{""role"":""RoadRunner_Person_Source"",""type"":""Edm.Self.RoadRunner"",""multiplicity"":""*""},{""role"":""RoadRunner_Person_Target"",""type"":""Edm.Self.Person"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""RoadRunner_Person_Target"",""propertyRef"":{""name"":""PersonId""}},""dependent"":{""role"":""RoadRunner_Person_Source"",""propertyRef"":{""name"":""PersonId""}}}},{""name"":""Security_Person"",""end"":[{""role"":""Security_Person_Source"",""type"":""Edm.Self.Security"",""multiplicity"":""0..1""},{""role"":""Security_Person_Target"",""type"":""Edm.Self.Person"",""multiplicity"":""1""}],""referentialConstraint"":{""principal"":{""role"":""Security_Person_Target"",""propertyRef"":{""name"":""PersonId""}},""dependent"":{""role"":""Security_Person_Source"",""propertyRef"":{""name"":""PersonId""}}}},{""name"":""Person_Student"",""end"":[{""role"":""Person_Student_Source"",""type"":""Edm.Self.Person"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}},{""role"":""Person_Student_Target"",""type"":""Edm.Self.ProfileStudent"",""multiplicity"":""0..1""}],""referentialConstraint"":{""principal"":{""role"":""Person_Student_Source"",""propertyRef"":{""name"":""PersonId""}},""dependent"":{""role"":""Person_Student_Target"",""propertyRef"":{""name"":""PersonId""}}}},{""name"":""CogEcmspeResult_Person"",""end"":[{""role"":""CogEcmspeResult_Person_Source"",""type"":""Edm.Self.CogEcmspeResult"",""multiplicity"":""*""},{""role"":""CogEcmspeResult_Person_Target"",""type"":""Edm.Self.Person"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogEcmspeResult_Person_Target"",""propertyRef"":{""name"":""PersonId""}},""dependent"":{""role"":""CogEcmspeResult_Person_Source"",""propertyRef"":{""name"":""PersonId""}}}},{""name"":""CogEcpeResult_Instrument"",""end"":[{""role"":""CogEcpeResult_Instrument_Source"",""type"":""Edm.Self.CogEcpeResult"",""multiplicity"":""*""},{""role"":""CogEcpeResult_Instrument_Target"",""type"":""Edm.Self.CogInstrument"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogEcpeResult_Instrument_Target"",""propertyRef"":{""name"":""Id""}},""dependent"":{""role"":""CogEcpeResult_Instrument_Source"",""propertyRef"":{""name"":""InstrumentId""}}}},{""name"":""CogEcpeResult_Person"",""end"":[{""role"":""CogEcpeResult_Person_Source"",""type"":""Edm.Self.CogEcpeResult"",""multiplicity"":""*""},{""role"":""CogEcpeResult_Person_Target"",""type"":""Edm.Self.Person"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogEcpeResult_Person_Target"",""propertyRef"":{""name"":""PersonId""}},""dependent"":{""role"":""CogEcpeResult_Person_Source"",""propertyRef"":{""name"":""PersonId""}}}},{""name"":""CogEsalbResult_Instrument"",""end"":[{""role"":""CogEsalbResult_Instrument_Source"",""type"":""Edm.Self.CogEsalbResult"",""multiplicity"":""*""},{""role"":""CogEsalbResult_Instrument_Target"",""type"":""Edm.Self.CogInstrument"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogEsalbResult_Instrument_Target"",""propertyRef"":{""name"":""Id""}},""dependent"":{""role"":""CogEsalbResult_Instrument_Source"",""propertyRef"":{""name"":""InstrumentId""}}}},{""name"":""CogEsalbResult_Person"",""end"":[{""role"":""CogEsalbResult_Person_Source"",""type"":""Edm.Self.CogEsalbResult"",""multiplicity"":""*""},{""role"":""CogEsalbResult_Person_Target"",""type"":""Edm.Self.Person"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogEsalbResult_Person_Target"",""propertyRef"":{""name"":""PersonId""}},""dependent"":{""role"":""CogEsalbResult_Person_Source"",""propertyRef"":{""name"":""PersonId""}}}},{""name"":""CogEtmpreResult_Instrument"",""end"":[{""role"":""CogEtmpreResult_Instrument_Source"",""type"":""Edm.Self.CogEtmpreResult"",""multiplicity"":""*""},{""role"":""CogEtmpreResult_Instrument_Target"",""type"":""Edm.Self.CogInstrument"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogEtmpreResult_Instrument_Target"",""propertyRef"":{""name"":""Id""}},""dependent"":{""role"":""CogEtmpreResult_Instrument_Source"",""propertyRef"":{""name"":""InstrumentId""}}}},{""name"":""CogEtmpreResult_Person"",""end"":[{""role"":""CogEtmpreResult_Person_Source"",""type"":""Edm.Self.CogEtmpreResult"",""multiplicity"":""*""},{""role"":""CogEtmpreResult_Person_Target"",""type"":""Edm.Self.Person"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogEtmpreResult_Person_Target"",""propertyRef"":{""name"":""PersonId""}},""dependent"":{""role"":""CogEtmpreResult_Person_Source"",""propertyRef"":{""name"":""PersonId""}}}},{""name"":""CogResponse_InventoryItem"",""end"":[{""role"":""CogResponse_InventoryItem_Source"",""type"":""Edm.Self.CogResponse"",""multiplicity"":""*""},{""role"":""CogResponse_InventoryItem_Target"",""type"":""Edm.Self.CogInventory"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogResponse_InventoryItem_Target"",""propertyRef"":{""name"":""Id""}},""dependent"":{""role"":""CogResponse_InventoryItem_Source"",""propertyRef"":{""name"":""CogInventoryId""}}}},{""name"":""CogResponse_Person"",""end"":[{""role"":""CogResponse_Person_Source"",""type"":""Edm.Self.CogResponse"",""multiplicity"":""*""},{""role"":""CogResponse_Person_Target"",""type"":""Edm.Self.Person"",""multiplicity"":""1"",""onDelete"":{""action"":""Cascade""}}],""referentialConstraint"":{""principal"":{""role"":""CogResponse_Person_Target"",""propertyRef"":{""name"":""PersonId""}},""dependent"":{""role"":""CogResponse_Person_Source"",""propertyRef"":{""name"":""PersonId""}}}}],""entityContainer"":{""name"":""UserMetadata"",""customannotation:UseClrTypes"":""true"",""entitySet"":[{""name"":""CogECMSPEResult"",""entityType"":""Self.CogEcmspeResult""},{""name"":""CogInstrument"",""entityType"":""Self.CogInstrument""},{""name"":""CogInventory"",""entityType"":""Self.CogInventory""},{""name"":""People"",""entityType"":""Self.Person""},{""name"":""Facilitators"",""entityType"":""Self.ProfileFaculty""},{""name"":""RoadRunnerAddresses"",""entityType"":""Self.RoadRunner""},{""name"":""Securities"",""entityType"":""Self.Security""},{""name"":""Students"",""entityType"":""Self.ProfileStudent""},{""name"":""CogECPEResult"",""entityType"":""Self.CogEcpeResult""},{""name"":""CogESALBResult"",""entityType"":""Self.CogEsalbResult""},{""name"":""CogETMPREResult"",""entityType"":""Self.CogEtmpreResult""},{""name"":""CogResponse"",""entityType"":""Self.CogResponse""}],""associationSet"":[{""name"":""CogInventory_Instrument"",""association"":""Self.CogInventory_Instrument"",""end"":[{""role"":""CogInventory_Instrument_Source"",""entitySet"":""CogInventory""},{""role"":""CogInventory_Instrument_Target"",""entitySet"":""CogInstrument""}]},{""name"":""CogEcmspeResult_Instrument"",""association"":""Self.CogEcmspeResult_Instrument"",""end"":[{""role"":""CogEcmspeResult_Instrument_Source"",""entitySet"":""CogECMSPEResult""},{""role"":""CogEcmspeResult_Instrument_Target"",""entitySet"":""CogInstrument""}]},{""name"":""Person_Faculty"",""association"":""Self.Person_Faculty"",""end"":[{""role"":""Person_Faculty_Source"",""entitySet"":""People""},{""role"":""Person_Faculty_Target"",""entitySet"":""Facilitators""}]},{""name"":""RoadRunner_Person"",""association"":""Self.RoadRunner_Person"",""end"":[{""role"":""RoadRunner_Person_Source"",""entitySet"":""RoadRunnerAddresses""},{""role"":""RoadRunner_Person_Target"",""entitySet"":""People""}]},{""name"":""Security_Person"",""association"":""Self.Security_Person"",""end"":[{""role"":""Security_Person_Source"",""entitySet"":""Securities""},{""role"":""Security_Person_Target"",""entitySet"":""People""}]},{""name"":""Person_Student"",""association"":""Self.Person_Student"",""end"":[{""role"":""Person_Student_Source"",""entitySet"":""People""},{""role"":""Person_Student_Target"",""entitySet"":""Students""}]},{""name"":""CogEcmspeResult_Person"",""association"":""Self.CogEcmspeResult_Person"",""end"":[{""role"":""CogEcmspeResult_Person_Source"",""entitySet"":""CogECMSPEResult""},{""role"":""CogEcmspeResult_Person_Target"",""entitySet"":""People""}]},{""name"":""CogEcpeResult_Instrument"",""association"":""Self.CogEcpeResult_Instrument"",""end"":[{""role"":""CogEcpeResult_Instrument_Source"",""entitySet"":""CogECPEResult""},{""role"":""CogEcpeResult_Instrument_Target"",""entitySet"":""CogInstrument""}]},{""name"":""CogEcpeResult_Person"",""association"":""Self.CogEcpeResult_Person"",""end"":[{""role"":""CogEcpeResult_Person_Source"",""entitySet"":""CogECPEResult""},{""role"":""CogEcpeResult_Person_Target"",""entitySet"":""People""}]},{""name"":""CogEsalbResult_Instrument"",""association"":""Self.CogEsalbResult_Instrument"",""end"":[{""role"":""CogEsalbResult_Instrument_Source"",""entitySet"":""CogESALBResult""},{""role"":""CogEsalbResult_Instrument_Target"",""entitySet"":""CogInstrument""}]},{""name"":""CogEsalbResult_Person"",""association"":""Self.CogEsalbResult_Person"",""end"":[{""role"":""CogEsalbResult_Person_Source"",""entitySet"":""CogESALBResult""},{""role"":""CogEsalbResult_Person_Target"",""entitySet"":""People""}]},{""name"":""CogEtmpreResult_Instrument"",""association"":""Self.CogEtmpreResult_Instrument"",""end"":[{""role"":""CogEtmpreResult_Instrument_Source"",""entitySet"":""CogETMPREResult""},{""role"":""CogEtmpreResult_Instrument_Target"",""entitySet"":""CogInstrument""}]},{""name"":""CogEtmpreResult_Person"",""association"":""Self.CogEtmpreResult_Person"",""end"":[{""role"":""CogEtmpreResult_Person_Source"",""entitySet"":""CogETMPREResult""},{""role"":""CogEtmpreResult_Person_Target"",""entitySet"":""People""}]},{""name"":""CogResponse_InventoryItem"",""association"":""Self.CogResponse_InventoryItem"",""end"":[{""role"":""CogResponse_InventoryItem_Source"",""entitySet"":""CogResponse""},{""role"":""CogResponse_InventoryItem_Target"",""entitySet"":""CogInventory""}]},{""name"":""CogResponse_Person"",""association"":""Self.CogResponse_Person"",""end"":[{""role"":""CogResponse_Person_Source"",""entitySet"":""CogResponse""},{""role"":""CogResponse_Person_Target"",""entitySet"":""People""}]}]}}}";

                var metaCtx = new EFContextProvider<UserMetadataCtx>();
                return metaCtx.Metadata();
            }
        }

        public IEnumerable<Person> GetUsers()
        {
            return _efCtx.Context.People;
        }

        public async Task<List<object>> GetProfile() {
            var userWithProfiles = await _efCtx.Context.People.Where(p => p.PersonId == User.PersonId)
                .Include(p => p.Student)
                .Include(p => p.Faculty)
                .Include(p => p.Designer)
                .Include(p => p.External)
                .Include(p => p.HqStaff).SingleAsync();

            var profiles = new List<object>();

            if (userWithProfiles.Student != null) { profiles.Add(userWithProfiles.Student); }
            if (userWithProfiles.Faculty != null) { profiles.Add(userWithProfiles.Faculty); }
            if (userWithProfiles.External != null) { profiles.Add(userWithProfiles.External); }
            if (userWithProfiles.Designer != null) { profiles.Add(userWithProfiles.Designer); }
            if (userWithProfiles.HqStaff != null) { profiles.Add(userWithProfiles.HqStaff); }

            return profiles;
        }

        public async Task<Person> LoginUser(string userName, string password)
        {
            var user = await _efCtx.Context.People
                .Include(s => s.Security)
                .Include(s => s.Faculty)
                .Include(s => s.Student)
                .SingleOrDefaultAsync(person => person.Email.ToLower() == userName.ToLower());

            if (user == default(Person))
            {
                throw new UnauthorizedAccessException("Invalid Username/Password Combination");
            }

            var isValid = PasswordHash.ValidatePassword(password, user.Security.PasswordHash);

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Invalid Username/Password Combination");
            }

            return user;
        }

        public async Task<Person> ProcessLtiUser(ILtiRequest parsedRequest)
        {
            var user = await _efCtx.Context.People
             .Include(s => s.Security)
             .Include(s => s.Faculty)
             .Include(s => s.Student)
             .SingleOrDefaultAsync(person => person.BbUserId == parsedRequest.UserId);

            var emailChecker = new ValidEmailChecker();
            if (user != null)
            {
                if (user.Email.ToLower() != parsedRequest.LisPersonEmailPrimary.ToLower()) {
                    if (!emailChecker.IsValidEmail(parsedRequest.LisPersonEmailPrimary)) {
                        throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is not a valid email address.");
                    }
                    if (!await UniqueEmailCheck(parsedRequest.LisPersonEmailPrimary)) {
                        throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is already being used in ECAT.");
                    }
                }
                user.ModifiedById = user.PersonId;
            }
            else
            {
                if (!emailChecker.IsValidEmail(parsedRequest.LisPersonEmailPrimary))
                {
                    throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is not a valid email address.");
                }
                if (!await UniqueEmailCheck(parsedRequest.LisPersonEmailPrimary)) {
                    throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is already being used in ECAT.");
                }

                user = new Person
                {
                    IsActive = true,
                    MpGender = MpGender.Unk,
                    MpAffiliation = MpAffiliation.Unk,
                    MpComponent = MpComponent.Unk,
                    MpPaygrade = MpPaygrade.Unk,
                    MpInstituteRole = MpInstituteRoleId.Undefined,
                    RegistrationComplete = false
                };

                _efCtx.Context.People.Add(user);
            }

            var userIsCourseAdmin = false;

            switch (parsedRequest.Parameters["Roles"].ToLower())
            {
                case "instructor":
                    userIsCourseAdmin = true;
                    user.MpInstituteRole = MpInstituteRoleId.Faculty;
                    break;
                case "teachingassistant":
                    user.MpInstituteRole = MpInstituteRoleId.Faculty;
                    break;
                case "contentdeveloper":
                    user.MpInstituteRole = MpInstituteRoleId.Designer;
                    break;
                default:
                    user.MpInstituteRole = MpInstituteRoleId.Student;
                    break;
            }

            switch (user.MpInstituteRole)
            {
                case MpInstituteRoleId.Faculty:
                    user.Faculty = user.Faculty ?? new ProfileFaculty();
                    user.Faculty.IsCourseAdmin = userIsCourseAdmin;
                    user.Faculty.AcademyId = parsedRequest.Parameters["custom_ecat_school"];
                    break;
                case MpInstituteRoleId.Designer:
                    user.Designer = user.Designer ?? new ProfileDesigner();
                    user.Designer.AssociatedAcademyId = parsedRequest.Parameters["custom_ecat_school"];
                    break;
                default:
                    user.Student = user.Student ?? new ProfileStudent();
                    break;
            }

            user.IsActive = true;
            user.Email = parsedRequest.LisPersonEmailPrimary.ToLower();
            user.LastName = parsedRequest.LisPersonNameFamily;
            user.FirstName = parsedRequest.LisPersonNameGiven;
            user.BbUserId = parsedRequest.UserId;
            user.ModifiedDate = DateTime.Now;

            if (await _efCtx.Context.SaveChangesAsync() > 0)
            {
                return user;
            }

            throw new UserUpdateException("Save User Changes did not succeed!");
        }

        public async Task<bool> UniqueEmailCheck(string email) => await _efCtx.Context.People.CountAsync(user => user.Email.ToLower() == email.ToLower()) == 0;

        public async Task<CogInstrument> GetCogInst(string type)
        {
            return await _efCtx.Context.CogInstruments
                .Where(cog => cog.MpCogInstrumentType == type && cog.IsActive == true)
                .Include(cog => cog.InventoryCollection)
                .FirstOrDefaultAsync();
        }

        public async Task<List<object>> GetCogResults(bool? all)
        {
            var results = new List<object>();

            var ecpe = await _efCtx.Context.CogEcpeResult
                .Where(cog => cog.PersonId == User.PersonId)
                .Include(cog => cog.Instrument)
                .OrderByDescending(cog => cog.Attempt)
                .ToListAsync();

            var etmpre = await _efCtx.Context.CogEtmpreResult
                .Where(cog => cog.PersonId == User.PersonId)
                .Include(cog => cog.Instrument)
                .OrderByDescending(cog => cog.Attempt)
                .ToListAsync();

            var esalb = await _efCtx.Context.CogEsalbResult
                .Where(cog => cog.PersonId == User.PersonId)
                .Include(cog => cog.Instrument)
                .OrderByDescending(cog => cog.Attempt)
                .ToListAsync();

            var ecmspe = await _efCtx.Context.CogEcmspeResult
                .Where(cog => cog.PersonId == User.PersonId)
                .Include(cog => cog.Instrument)
                .OrderByDescending(cog => cog.Attempt)
                .ToListAsync();

            if (all == null || all == false)
            {
                if (ecpe.Any()) { results.Add(ecpe[0]); }
                if (etmpre.Any()) { results.Add(etmpre[0]); }
                if (esalb.Any()) { results.Add(esalb[0]); }
                if (ecmspe.Any()) { results.Add(ecmspe[0]); }
            }
            else {
                if (ecpe.Any()) { results.AddRange(ecpe); }
                if (etmpre.Any()) { results.AddRange(etmpre); }
                if (esalb.Any()) { results.AddRange(esalb); }
                if (ecmspe.Any()) { results.AddRange(ecmspe); }
            }
            
            return results;
        }

        public SaveResult ClientSave(JObject saveBundle)
        {
            var guardian = new UserGuardian(_efCtx, User);
            _efCtx.BeforeSaveEntitiesDelegate += guardian.BeforeSaveEntities;
            return _efCtx.SaveChanges(saveBundle);
        }

        //retrieves all roadrunner info on user that is logged in
        public async Task<List<RoadRunner>> GetRoadRunnerInfo(){
            var personList = new List<RoadRunner>();
            personList = await _efCtx.Context.RoadRunnerAddresses.Where(e => e.PersonId == User.PersonId).ToListAsync();
            return personList;
        }
    }
}
