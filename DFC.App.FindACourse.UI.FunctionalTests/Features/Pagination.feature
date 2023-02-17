Feature: Pagination
	In order to find a suitable course
	As someone looking to attend a new course
	I want to search for courses relevant to me
	And view the course details

@Smoke @FindACourse
Scenario: Search a course and click on next and previous page links
	Given I am on the find a course landing page
	When I enter the search term maths in the search box
	And I click the search button
	Then search results are displayed
	When i click on the next page link
	Then next page of results are displayed
	When i click on the previous page link
	When I click on the first search result
	Then the course details page is displayed