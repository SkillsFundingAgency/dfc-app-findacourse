Feature: CheckResultsAreDisplayed
	When I search for results
	And I filter by 5 miles
	I should not see any rogue courses not outside the distance chosen


@FindACourse
Scenario: Check results are within the distance range selected
	Given I am on the find a course landing page
	When I enter the search term maths in the search box
	And I click the search button
	When I enter CV1 2WT in the location filter
	And I filter by 5 miles
	And I select Classroom based in the learning method filter
	And I select Distance in the sort by filter
	Then all results are under 5 miles