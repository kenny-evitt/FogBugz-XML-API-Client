## Overview of the FogBugz API Client Library

FogBugz API Client is a .NET (4.5) class library for using the [FogBugz XML API](http://fogbugz.stackexchange.com/fogbugz-xml-api).

The code was copied and heavily modified from [FogLampz](http://foglampz.codeplex.com/), particularly parts of the class `FogBugzClient`.

Basically, the static class `FogBugzClient` provides two methods `Logon` and `SubmitCommand`. The 
`SubmitCommand` returns an [`XElement`](http://msdn.microsoft.com/en-us/library/system.xml.linq.xelement.aspx) 
object with the response from the command.

Example usage:

    FogBugzClient.Logon("https://recp.fogbugz.com/api.asp", "Kenny Evitt", "this-isn't-really-my-password");

	XElement listProjectsResult = FogBugzClient.SubmitCommand("listProjects");
	// The value of 'listProjectsResult' should be something like the following:
	//
	//	<response>
	//		<projects>
	//			<project>
	//				<ixProject>2</ixProject>
	//				<sProject><![CDATA[@Inbox]]></sProject>
	//				<ixPersonOwner>2</ixPersonOwner>
	//				<sPersonOwner><![CDATA[Kenny Evitt]]></sPersonOwner>
	//				<sEmail><![CDATA[kevitt@example.com]]></sEmail>
	//				<sPhone></sPhone>
	//				<fInbox>true</fInbox>
	//				<ixWorkflow>1</ixWorkflow>
	//				<fDeleted>false</fDeleted>
	//			</project>
	//			<project>
	//				<ixProject>3</ixProject>
	//				<sProject><![CDATA[@Misc]]></sProject>
	//				<ixPersonOwner>2</ixPersonOwner>
	//				<sPersonOwner><![CDATA[Kenny Evitt]]></sPersonOwner>
	//				<sEmail><![CDATA[kevitt@example.com]]></sEmail>
	//				<sPhone></sPhone>
	//				<fInbox>false</fInbox>
	//				<ixWorkflow>1</ixWorkflow>
	//				<fDeleted>false</fDeleted>
	//			</project>
	//			...

	XElement editMilestoneResult =
		FogBugzClient.SubmitCommand(
			"editFixFor",
			new Dictionary<string, string> { 
				{ "ixFixFor", "7" },
				{ "sFixFor", "1.6" },
				{ "fAssignable", "0" } });
	// The value of 'editMilestoneResult' should be something like the following:
	//
	//	<response>
	//		<fixfor>
	//			<ixFixFor>7</ixFixFor>
	//			<sFixFor><![CDATA[1.6]]></sFixFor>
	//			<fInactive>true</fInactive>
	//			<dt></dt>
	//			<ixProject>11</ixProject>
	//			<dtStart></dtStart>
	//			<sStartNote></sStartNote>
	//			<setixFixForDependency></setixFixForDependency>
	//		</fixfor>
	//	</response>