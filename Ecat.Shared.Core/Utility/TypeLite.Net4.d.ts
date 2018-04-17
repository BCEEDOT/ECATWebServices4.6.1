
 
 

 

/// <reference path="serverEnums.ts" />

declare module ecat.entity.s.user {
	interface Person {
		personId: number;
		isActive: boolean;
		lastName: string;
		firstName: string;
		avatarLocation: string;
		goByName: string;
		mpGender: string;
		mpAffiliation: string;
		mpPaygrade: string;
		mpComponent: string;
		email: string;
		registrationComplete: boolean;
		mpInstituteRole: string;
		student: ecat.entity.s.user.ProfileStudent;
		faculty: ecat.entity.s.user.ProfileFaculty;
		external: ecat.entity.s.user.ProfileExternal;
		hqStaff: ecat.entity.s.user.ProfileStaff;
		profile: ecat.entity.s.user.ProfileBase;
	}
	interface ProfileStudent extends ecat.entity.s.user.ProfileBase {
		contactNumber: string;
		commander: string;
		shirt: string;
		commanderEmail: string;
		shirtEmail: string;
		courses: ecat.entity.s.school.StudentInCourse[];
		courseGroupMemberships: ecat.entity.s.school.CrseStudentInGroup[];
	}
	interface ProfileBase {
		personId: number;
		bio: string;
		homeStation: string;
		person: ecat.entity.s.user.Person;
	}
	interface ProfileFaculty extends ecat.entity.s.user.ProfileBase {
		isCourseAdmin: boolean;
		isReportViewer: boolean;
		academyId: string;
		courses: ecat.entity.s.school.FacultyInCourse[];
	}
	interface ProfileExternal extends ecat.entity.s.user.ProfileBase {
	}
	interface ProfileStaff extends ecat.entity.s.user.ProfileBase {
		meetingAttendances: ecat.entity.s.staff.MeetingAttendee[];
	}
}
declare module ecat.entity.s.school {
	interface StudentInCourse {
		entityId: string;
		courseId: number;
		studentPersonId: number;
		bbCourseMemId: string;
		course: ecat.entity.s.school.Course;
		student: ecat.entity.s.user.ProfileStudent;
		workGroupEnrollments: ecat.entity.s.school.CrseStudentInGroup[];
		kcResponses: ecat.entity.s.learner.KcResponse[];
		reconResultId: System.Guid;
		reconResult: ecat.entity.s.lmsAdmin.MemReconResult;
	}
	interface Course {
		id: number;
		academyId: string;
		bbCourseId: string;
		name: string;
		classNumber: string;
		term: string;
		gradReportPublished: boolean;
		startDate: Date;
		gradDate: Date;
		spResults: ecat.entity.s.learner.SpResult[];
		stratResults: ecat.entity.s.learner.StratResult[];
		students: ecat.entity.s.school.StudentInCourse[];
		studentInCrseGroups: ecat.entity.s.school.CrseStudentInGroup[];
		spResponses: ecat.entity.s.learner.SpResponse[];
		faculty: ecat.entity.s.school.FacultyInCourse[];
		workGroups: ecat.entity.s.school.WorkGroup[];
		reconResultId: System.Guid;
		reconResult: ecat.entity.s.lmsAdmin.CourseReconResult;
	}
	interface CrseStudentInGroup {
		entityId: string;
		studentId: number;
		courseId: number;
		workGroupId: number;
		hasAcknowledged: boolean;
		numOfStratIncomplete: number;
		bbCrseStudGroupId: string;
		reconResultId: System.Guid;
		workGroup: ecat.entity.s.school.WorkGroup;
		studentProfile: ecat.entity.s.user.ProfileStudent;
		course: ecat.entity.s.school.Course;
		studentInCourse: ecat.entity.s.school.StudentInCourse;
		reconResult: ecat.entity.s.lmsAdmin.GroupMemReconResult;
		groupPeers: ecat.entity.s.school.CrseStudentInGroup[];
		assessorSpResponses: ecat.entity.s.learner.SpResponse[];
		assesseeSpResponses: ecat.entity.s.learner.SpResponse[];
		authorOfComments: ecat.entity.s.learner.StudSpComment[];
		recipientOfComments: ecat.entity.s.learner.StudSpComment[];
		assessorStratResponse: ecat.entity.s.learner.StratResponse[];
		assesseeStratResponse: ecat.entity.s.learner.StratResponse[];
		facultySpResponses: ecat.entity.s.faculty.FacSpResponse[];
		spResult: ecat.entity.s.learner.SpResult;
		stratResult: ecat.entity.s.learner.StratResult;
		facultyComment: ecat.entity.s.faculty.FacSpComment;
		facultyStrat: ecat.entity.s.faculty.FacStratResponse;
		numberOfAuthorComments: number;
		isDeleted: boolean;
	}
	interface WorkGroup {
		workGroupId: number;
		courseId: number;
		wgModelId: number;
		mpCategory: string;
		groupNumber: string;
		reconResultId: System.Guid;
		assignedSpInstrId: number;
		assignedKcInstrId: number;
		customName: string;
		bbGroupId: string;
		defaultName: string;
		mpSpStatus: string;
		isPrimary: boolean;
		course: ecat.entity.s.school.Course;
		wgModel: Ecat.Shared.Core.ModelLibrary.Designer.WorkGroupModel;
		reconResult: ecat.entity.s.lmsAdmin.GroupReconResult;
		facSpResponses: ecat.entity.s.faculty.FacSpResponse[];
		facStratResponses: ecat.entity.s.faculty.FacStratResponse[];
		facSpComments: ecat.entity.s.faculty.FacSpComment[];
		groupMembers: ecat.entity.s.school.CrseStudentInGroup[];
		spComments: ecat.entity.s.learner.StudSpComment[];
		spResponses: ecat.entity.s.learner.SpResponse[];
		spResults: ecat.entity.s.learner.SpResult[];
		spStratResponses: ecat.entity.s.learner.StratResponse[];
		spStratResults: ecat.entity.s.learner.StratResult[];
		assignedSpInstr: ecat.entity.s.designer.SpInstrument;
		assignedKcInstr: ecat.entity.s.designer.KcInstrument;
		canPublish: boolean;
		modifiedById: number;
		modifiedDate: Date;
	}
	interface FacultyInCourse {
		entityId: string;
		courseId: number;
		facultyPersonId: number;
		bbCourseMemId: string;
		course: ecat.entity.s.school.Course;
		facultyProfile: ecat.entity.s.user.ProfileFaculty;
		facSpResponses: ecat.entity.s.faculty.FacSpResponse[];
		facSpComments: ecat.entity.s.faculty.FacSpComment[];
		facStratResponse: ecat.entity.s.faculty.FacStratResponse[];
		flaggedSpComments: ecat.entity.s.learner.StudSpCommentFlag[];
		reconResultId: System.Guid;
		reconResult: ecat.entity.s.lmsAdmin.MemReconResult;
	}
	interface Academy {
		id: string;
		longName: string;
		shortName: string;
		mpEdLevel: string;
		base: Ecat.Shared.Core.Utility.AcademyBase;
		bbCategoryId: string;
		parentBbCategoryId: string;
	}
}
declare module ecat.entity.s.learner {
	interface SpResult {
		entityId: string;
		courseId: number;
		workGroupId: number;
		studentId: number;
		assignedInstrumentId: number;
		mpAssessResult: string;
		compositeScore: number;
		breakOut: Ecat.Shared.Core.ModelLibrary.Learner.SpResultBreakOut;
		resultFor: ecat.entity.s.school.CrseStudentInGroup;
		assignedInstrument: ecat.entity.s.designer.SpInstrument;
		workGroup: ecat.entity.s.school.WorkGroup;
		course: ecat.entity.s.school.Course;
		facultyResponses: ecat.entity.s.faculty.FacSpResponse[];
		spResponses: ecat.entity.s.learner.SpResponse[];
		sanitizedResponses: ecat.entity.s.learner.SanitizedSpResponse[];
		sanitizedComments: ecat.entity.s.learner.SanitizedSpComment[];
		modifiedById: number;
		modifiedDate: Date;
	}
	interface SpResponse {
		entityId: string;
		assessorPersonId: number;
		assesseePersonId: number;
		workGroupId: number;
		courseId: number;
		inventoryItemId: number;
		mpItemResponse: string;
		itemModelScore: number;
		inventoryItem: ecat.entity.s.designer.SpInventory;
		workGroup: ecat.entity.s.school.WorkGroup;
		course: ecat.entity.s.school.Course;
		assessor: ecat.entity.s.school.CrseStudentInGroup;
		assessee: ecat.entity.s.school.CrseStudentInGroup;
		modifiedById: number;
		modifiedDate: Date;
	}
	interface KcResult {
		inventoryId: number;
		courseId: number;
		studentId: number;
		version: number;
		instrumentId: number;
		numberCorrect: number;
		score: number;
		instrument: ecat.entity.s.designer.KcInstrument;
		responses: ecat.entity.s.learner.KcResponse[];
	}
	interface KcResponse {
		entityId: string;
		inventoryId: number;
		courseId: number;
		studentId: number;
		resultId: number;
		isCorrect: boolean;
		version: number;
		allowNewAttempt: boolean;
		student: ecat.entity.s.school.StudentInCourse;
		result: ecat.entity.s.learner.KcResult;
		inventory: ecat.entity.s.designer.KcInventory;
	}
	interface StudSpCommentFlag {
		mpAuthor: string;
		mpRecipient: string;
		mpFaculty: string;
		authorPersonId: number;
		recipientPersonId: number;
		flaggedByFacultyId: number;
		courseId: number;
		workGroupId: number;
		comment: ecat.entity.s.learner.StudSpComment;
		flaggedByFaculty: ecat.entity.s.school.FacultyInCourse;
	}
	interface StudSpComment {
		entityId: string;
		authorPersonId: number;
		recipientPersonId: number;
		workGroupId: number;
		facultyPersonId: number;
		courseId: number;
		requestAnonymity: boolean;
		commentText: string;
		createdDate: Date;
		modifiedById: number;
		modifiedDate: Date;
		author: ecat.entity.s.school.CrseStudentInGroup;
		recipient: ecat.entity.s.school.CrseStudentInGroup;
		workGroup: ecat.entity.s.school.WorkGroup;
		course: ecat.entity.s.school.Course;
		flag: ecat.entity.s.learner.StudSpCommentFlag;
	}
	interface StratResponse {
		entityId: string;
		assessorPersonId: number;
		assesseePersonId: number;
		courseId: number;
		workGroupId: number;
		stratPosition: number;
		assessor: ecat.entity.s.school.CrseStudentInGroup;
		assessee: ecat.entity.s.school.CrseStudentInGroup;
		modifiedById: number;
		modifiedDate: Date;
	}
	interface StratResult {
		entityId: string;
		courseId: number;
		studentId: number;
		workGroupId: number;
		originalStratPosition: number;
		finalStratPosition: number;
		stratCummScore: number;
        studStratAwardedScore: number;
        facStratAwardedScore: number;
		course: ecat.entity.s.school.Course;
		resultFor: ecat.entity.s.school.CrseStudentInGroup;
		facStrat: ecat.entity.s.faculty.FacStratResponse;
		stratResponses: ecat.entity.s.learner.StratResponse[];
		modifiedById: number;
		modifiedDate: Date;
	}
	interface SanitizedSpResponse {
		id: System.Guid;
		courseId: number;
		assesseeId: number;
		workGroupId: number;
		isSelfResponse: boolean;
		peerGenericName: string;
		mpItemResponse: string;
		itemModelScore: number;
		inventoryItemId: number;
		inventoryItem: ecat.entity.s.designer.SpInventory;
		result: ecat.entity.s.learner.SpResult;
	}
	interface SanitizedSpComment {
		id: System.Guid;
		recipientId: number;
		courseId: number;
		workGroupId: number;
		authorName: string;
		commentText: string;
		flag: ecat.entity.s.learner.StudSpCommentFlag;
		facFlag: ecat.entity.s.faculty.FacSpCommentFlag;
		mpCommentFlagRecipient: string;
		result: ecat.entity.s.learner.SpResult;
	}
}
declare module Ecat.Shared.Core.ModelLibrary.Learner {
	interface SpResultBreakOut {
		ineffA: number;
		ineffU: number;
		effA: number;
		effU: number;
		highEffU: number;
		highEffA: number;
		notDisplay: number;
	}
}
declare module System {
	interface Guid {
	}
}
declare module Ecat.Shared.Core.ModelLibrary.Designer {
	interface WorkGroupModel {
		id: number;
		assignedSpInstrId: number;
		assignedKcInstrId: number;
		mpEdLevel: string;
		mpWgCategory: string;
		maxStratStudent: number;
		maxStratFaculty: number;
		isActive: boolean;
		stratDivisor: number;
		assignedSpInstr: ecat.entity.s.designer.SpInstrument;
		assignedKcInstr: ecat.entity.s.designer.KcInstrument;
		workGroups: ecat.entity.s.school.WorkGroup[];
	}
}
declare module ecat.entity.s.designer {
	interface SpInstrument {
		id: number;
		name: string;
		isActive: boolean;
		version: string;
		studentInstructions: string;
		facultyInstructions: string;
		modifiedDate: Date;
		modifiedById: number;
		inventoryCollection: ecat.entity.s.designer.SpInventory[];
		assignedGroups: ecat.entity.s.school.WorkGroup[];
		spAssessMaps: ecat.entity.s.designer.SpAssessMap[];
	}
	interface SpInventory {
		id: number;
		instrumentId: number;
		displayOrder: number;
		isScored: boolean;
		isDisplayed: boolean;
		behavior: string;
		modifiedById: number;
		modifiedDate: Date;
		instrument: ecat.entity.s.designer.SpInstrument;
		itemResponses: ecat.entity.s.learner.SpResponse[];
	}
	interface SpAssessMap extends ecat.entity.s.designer.AssessMap {
		spInstrumentId: number;
		spInstrument: ecat.entity.s.designer.SpInstrument;
	}
	interface AssessMap {
		id: number;
		isActive: boolean;
		academyId: string;
		groupType: string;
		assignedData: Date;
	}
	interface KcInstrument {
		id: number;
		modifiedById: number;
		instructions: string;
		version: string;
		isActive: boolean;
		modifiedDate: Date;
		results: ecat.entity.s.learner.KcResult[];
		assignedGroups: ecat.entity.s.school.WorkGroup[];
	}
	interface KcInventory {
		id: number;
		modifiedById: number;
		instrumentId: number;
		displayOrder: number;
		isScored: boolean;
		isDisplayed: boolean;
		knowledgeArea: string;
		questionText: string;
		itemWeight: number;
		answer: string;
		modifiedDate: Date;
		instrument: ecat.entity.s.designer.KcInstrument;
		responses: ecat.entity.s.learner.KcResponse[];
	}
	interface CogInstrument {
		id: number;
		modifiedById: number;
		version: string;
		isActive: boolean;
		cogInstructions: string;
		mpCogInstrumentType: string;
		cogResultRange: string;
		modifiedDate: Date;
	}
	interface CogInventory {
		id: number;
		instrumentId: number;
		displayOrder: number;
		isScored: boolean;
		isDisplayed: boolean;
		adaptiveDescription: string;
		innovativeDescription: string;
		itemDescription: string;
		isReversed: boolean;
		instrument: ecat.entity.s.designer.CogInstrument;
		modifiedById: number;
		modifiedDate: Date;
	}
}
declare module ecat.entity.s.lmsAdmin {
	interface GroupReconResult extends Ecat.Shared.Core.ModelLibrary.Common.ReconcileResult {
		groups: ecat.entity.s.school.WorkGroup[];
	}
	interface MemReconResult extends Ecat.Shared.Core.ModelLibrary.Common.ReconcileResult {
		courseId: number;
		numOfAccountCreated: number;
		faculty: ecat.entity.s.school.FacultyInCourse[];
		students: ecat.entity.s.school.StudentInCourse[];
		removedIds: number[];
	}
	interface GroupMemReconResult extends Ecat.Shared.Core.ModelLibrary.Common.ReconcileResult {
		courseId: number;
		workGroupId: number;
		workGroupName: string;
		groupType: string;
		groupMembers: ecat.entity.s.school.CrseStudentInGroup[];
	}
	interface CourseReconResult extends Ecat.Shared.Core.ModelLibrary.Common.ReconcileResult {
		courses: ecat.entity.s.school.Course[];
	}
}
declare module Ecat.Shared.Core.ModelLibrary.Common {
	interface ReconcileResult {
		id: System.Guid;
		academyId: string;
		numAdded: number;
		numRemoved: number;
	}
}
declare module ecat.entity.s.faculty {
	interface FacSpResponse {
		entityId: string;
		inventoryItemId: number;
		workGroupId: number;
		facultyPersonId: number;
		assesseePersonId: number;
		courseId: number;
		mpItemResponse: string;
		itemModelScore: number;
		scoreModelVersion: number;
		workGroup: ecat.entity.s.school.WorkGroup;
		assessee: ecat.entity.s.school.CrseStudentInGroup;
		facultyAssessor: ecat.entity.s.school.FacultyInCourse;
		inventoryItem: ecat.entity.s.designer.SpInventory;
	}
	interface FacSpComment {
		entityId: string;
		recipientPersonId: number;
		workGroupId: number;
		facultyPersonId: number;
		courseId: number;
		createdDate: Date;
		commentText: string;
		facultyCourse: ecat.entity.s.school.FacultyInCourse;
		recipient: ecat.entity.s.school.CrseStudentInGroup;
		workGroup: ecat.entity.s.school.WorkGroup;
		course: ecat.entity.s.school.Course;
		flag: ecat.entity.s.faculty.FacSpCommentFlag;
		modifiedById: number;
		modifiedDate: Date;
	}
	interface FacSpCommentFlag {
		facultyId: number;
		mpAuthor: string;
		mpRecipient: string;
		recipientPersonId: number;
		courseId: number;
		workGroupId: number;
		comment: ecat.entity.s.faculty.FacSpComment;
	}
	interface FacStratResponse {
		entityId: string;
		stratPosition: number;
		stratResultId: number;
		courseId: number;
		facultyPersonId: number;
		assesseePersonId: number;
		workGroupId: number;
		studentAssessee: ecat.entity.s.school.CrseStudentInGroup;
		workGroup: ecat.entity.s.school.WorkGroup;
		facultyAssessor: ecat.entity.s.school.FacultyInCourse;
		modifiedById: number;
		modifiedDate: Date;
	}
}
declare module ecat.entity.s.staff {
	interface MeetingAttendee {
		meetingId: number;
		attendeeId: number;
		isOrganizer: number;
		attendee: ecat.entity.s.user.ProfileStaff;
		meeting: ecat.entity.s.staff.Meeting;
	}
	interface Meeting {
		id: number;
		meetingReason: string;
		abstract: string;
		background: string;
		purpose: string;
		meetingDate: Date;
		attendees: ecat.entity.s.staff.MeetingAttendee[];
		actionItemses: ecat.entity.s.staff.ActionItem[];
		decisions: ecat.entity.s.staff.Decision[];
		discussions: ecat.entity.s.staff.Discussion[];
	}
	interface ActionItem {
		id: number;
		meetingId: number;
		todo: string;
		opr: string;
		mpActionStatus: string;
		resolution: string;
		dueDate: Date;
		resolutionDate: Date;
		meeting: ecat.entity.s.staff.Meeting;
		modifiedById: number;
		modifiedDate: Date;
	}
	interface Decision {
		id: number;
		meetingId: number;
		status: string;
		decisionItem: string;
		approvalAuthority: string;
		approvedDate: Date;
		meeting: ecat.entity.s.staff.Meeting;
		modifiedById: number;
		modifiedDate: Date;
	}
	interface Discussion {
		id: number;
		meetingId: number;
		discussionItem: string;
		meeting: ecat.entity.s.staff.Meeting;
	}
}
declare module ecat.entity.s.cog {
	interface CogResponse {
		id: number;
		cogInventoryItem: number;
		itemScore: number;
	}
	interface CogResult {
		id: number;
		mpCogOutcome: string;
		mpCogScore: number;
	}
}
declare module ecat.entity.s.common {
	interface AcademyCategory {
		id: string;
		bbCatId: string;
		bbCatName: string;
		relatedCoursesCount: number;
	}
	interface SpCommentBase {
		createdDate: Date;
		anonymity: boolean;
		commentText: string;
		modifiedById: number;
		modifiedDate: Date;
	}
	interface LoginToken {
		personId: number;
		authToken: string;
		tokenExpireWarning: Date;
		tokenExpire: Date;
		person: ecat.entity.s.user.Person;
	}
}


