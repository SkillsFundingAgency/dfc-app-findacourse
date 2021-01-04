Feature: SearchForACourseSortResults
	In order to find a suitable course
	As someone looking to attend a new course
	I want to search for courses relevant to me
	And sort the results using the sort by filter

@FindACourse
Scenario: Search for a course using sort by filter
	Given I am on the find a course landing page
	When I enter the search term computer in the search box
	And I click the search button
	Then search results are displayed
	When I select Start date in the sort by filter
	Then search results are updated