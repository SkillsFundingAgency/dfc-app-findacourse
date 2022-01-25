Feature: ViewCourseDetails
	In order to find a suitable course
	As someone looking to attend a new course
	I want to search for courses relevant to me
	And view the course details

@Smoke @FindACourse
Scenario: View course details for search result
	Given I am on the find a course landing page
	When I enter the search term computing in the search box
	And I click the search button
	Then search results are displayed
	When I click on the first search result
	Then the course details page is displayed