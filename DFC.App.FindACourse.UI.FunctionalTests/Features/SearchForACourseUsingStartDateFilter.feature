Feature: SearchForACourseUsingStartDateFilter
	In order to find a suitable course
	As someone looking to attend a new course
	I want to search for courses relevant to me
	And filter the results by start date

@mytag
Scenario: Search for a course using start date search filter
	Given I am on the find a course landing page
	When I enter the search term chemistry in the search box
	And I click the search button
	Then search results are displayed
	When I select Next 3 months in the start date filter
	Then search results are updated