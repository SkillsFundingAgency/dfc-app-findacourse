Feature: GoBackToSearchResultsFromCourseDetails
	In order to find a suitable course
	As someone looking to attend a new course
	I want to search for courses relevant to me
	And view the course details
	Then I go back to the search results page

@FindACourse
Scenario: Go back to search results page from course details
	Given I am on the find a course landing page
	When I enter the search term computing in the search box
	And I click the search button
	Then search results are displayed
	When I click on the first search result
	Then the course details page is displayed
	When I click the back to search results link
	Then I am returned to same search results page