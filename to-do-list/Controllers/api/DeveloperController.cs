using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Controllers.api
{
    public class FeedbackResults
    {
        public int MaxPage { get; set; }
        public int? PageNumber { get; set; }
        public string UserSearchValue { get; set; }

        public string DetailsSearchValue { get; set; }
        public List<Suggestion> Feedback { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class DeveloperController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public DeveloperController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        [HttpGet]
        public FeedbackResults GetFeedback([FromQuery] int? pageNumber, [FromQuery] string userSearchValue, [FromQuery]string detailsSearchValue)
        {
            var feedbackResults = new FeedbackResults();

            var feedback = unitOfWork.Suggestions.GetAll().OrderByDescending(suggestion => suggestion.SuggestionId).ToList();

            if (!String.IsNullOrEmpty(userSearchValue))
                feedback = feedback.Where(p => p.User.ToLower().Contains(userSearchValue)).ToList();

            if (!String.IsNullOrEmpty(detailsSearchValue))
                feedback = feedback.Where(p => p.Details.ToLower().Contains(detailsSearchValue)).ToList();

            var pageSize = 13;
            var numberOfResults = feedback.Count();
            feedbackResults.MaxPage = (numberOfResults / pageSize) - (numberOfResults % pageSize == 0 ? 1 : 0) + 1;

            if (!pageNumber.HasValue)
                pageNumber = 1;

            feedbackResults.PageNumber = pageNumber;

            feedbackResults.Feedback = feedback.Skip((pageNumber.Value - 1) * pageSize).Take(pageSize).ToList();

            return feedbackResults;
        }
    }
}
