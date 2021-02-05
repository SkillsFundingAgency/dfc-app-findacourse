using DFC.App.FindACourse.Repository;
using DFC.CompositeInterfaceModels.FindACourseClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    public class FindACourseService : IFindACourseService
    {
        private readonly IFindACourseRepository repository;

        public FindACourseService(IFindACourseRepository repository)
        {
            this.repository = repository;
        }

        public bool PingAsync()
        {
            return repository.PingAsync();
        }

        public IList<T> GetFilterByName<T>()
        {
            return repository.GetFilter<T>();
        }

        public async Task<CourseSearchResult> GetFilteredData(CourseSearchFilters filters, CourseSearchOrderBy orderBy, int pageSize)
        {
            var criteriaProperties = new CourseSearchProperties
            {
                Filters = filters,
                OrderedBy = orderBy,
                Page = pageSize,
            };

            return await repository.RetrieveData(criteriaProperties).ConfigureAwait(false);
        }

        public async Task<CourseDetails> GetCourseDetails(string courseId, string runId)
        {
            return await repository.GetCourseDetails(courseId, runId).ConfigureAwait(false);
        }

        public async Task<TLevelDetails> GetTLevelDetails(string tlevelId, string tlevelLocationId)
        {
            var tlevelDetails = await repository.GetTLevelDetails(tlevelId).ConfigureAwait(false);

            //put the requested venue at the top if possible
            tlevelDetails.Venues = tlevelDetails.Venues?.OrderByDescending(m => m.Id == tlevelLocationId).ToList();
            return tlevelDetails;
        }

        //To remove once we get real data
        private static Venue GetDummyVenue(string name, string id)
        {
            var venue = new Venue() { VenueName = name, PhoneNumber = "12345 678", Website = "https://bbc.com", Id = id };
            venue.EmailAddress = "g@fr.com";
            venue.Location = new Address()
            {
                AddressLine1 = "AddressLine1",
                AddressLine2 = "AddressLine2",
                Town = "Town1",
                Postcode = "P11 5DF",
                County = "West Midlands",
                Latitude = "52.54715579704134",
                Longitude = "-1.8226955723337404",
            };

            return venue;
        }
    }
}