Feature: SearchForACourseUsingCourseTypeFilter
	In order to find a suitable course
	As someone looking to attend a new course
	I want to search for courses relevant to me
	And filter the results by Learning method

@FindACourse
Scenario: Search for a course using Learning method search filter
	Given I am on the find a course landing page
	When I enter the search term computer in the search box
	And I click the search button
	Then search results are displayed
	When I select Work based in the Learning method filter
	Then search results are updated