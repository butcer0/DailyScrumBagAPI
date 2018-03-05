using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using DailyScrumBagAPI.API.Dtos;
using DailyScrumBagAPI.API.Entities;
using DailyScrumBagAPI.API.Helpers;
using DailyScrumBagAPI.API.Models;
using DailyScrumBagAPI.API.Repositories;

namespace DailyScrumBagAPI.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/Users")]
    public class UsersController : Controller
    {
        #region internal fields
        private readonly IUserRepository _userRepository;
        private readonly IUrlHelper _urlHelper;
        #endregion

        #region Constructors
        public UsersController(IUrlHelper urlHelper, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _urlHelper = urlHelper;
        }
        #endregion

        #region API Methods
        [HttpGet(Name = nameof(GetAllUsers))]
        public IActionResult GetAllUsers([FromQuery] QueryParameters queryParameters)
        {
            List<UserItem> userItems = _userRepository.GetAll(queryParameters).ToList();

            var allItemCount = _userRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForCollection(queryParameters, allItemCount);
            var toReturn = userItems.Select(x => ExpandSingleUserItem(x));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleUser))]
        public IActionResult GetSingleUser(int id)
        {
            UserItem userItem = _userRepository.GetSingle(id);
            if(userItem == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleUserItem(userItem));
        }

        [HttpPost(Name = nameof(AddUser))]
        public IActionResult AddUser([FromBody] UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserItem toAdd = Mapper.Map<UserItem>(userCreateDto);

            _userRepository.Add(toAdd);
            if (!_userRepository.Save())
            {
                throw new Exception("Creating a user failed on save.");
            }

            UserItem newUserItem = _userRepository.GetSingle(toAdd.Id);

            return CreatedAtRoute(nameof(GetSingleUser), new { id = newUserItem.Id },
                Mapper.Map<UserItemDto>(newUserItem));

        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateUser))]
        public IActionResult PartiallyUpdateUser(int id, [FromBody] JsonPatchDocument<UserUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            UserItem existingEntity = _userRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            UserUpdateDto userUpdateDto = Mapper.Map<UserUpdateDto>(existingEntity);
            patchDoc.ApplyTo(userUpdateDto, ModelState);

            TryValidateModel(userUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(userUpdateDto, existingEntity);
            UserItem updated = _userRepository.Update(id, existingEntity);

            if (!_userRepository.Save())
            {
                throw new Exception("Updating a user failed on save.");
            }

            return Ok(Mapper.Map<UserItemDto>(updated));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveUser))]
        public IActionResult RemoveUser(int id)
        {
            UserItem userItem = _userRepository.GetSingle(id);
            if (userItem == null)
            {
                return NotFound();
            }

            _userRepository.Delete(id);
            
            if (!_userRepository.Save())
            {
                throw new Exception("Deleting a user failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateUser))]
        public IActionResult UpdateUser(int id, [FromBody]UserUpdateDto userUpdateDto)
        {
            if (userUpdateDto == null)
            {
                return BadRequest();
            }

            var existingUserItem = _userRepository.GetSingle(id);

            if (existingUserItem == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(userUpdateDto, existingUserItem);
            _userRepository.Update(id, existingUserItem);

            if(!_userRepository.Save())
            {
                throw new Exception("Updating a user failed on save.");
            }

            return Ok(Mapper.Map<UserItemDto>(existingUserItem));

        }

        #endregion
        #region Helper Methods
        private dynamic ExpandSingleUserItem(UserItem UserItem)
        {
            var links = GetLinks(UserItem.Id);
            UserItemDto item = Mapper.Map<UserItemDto>(UserItem);

            var resourceToReturn = item.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
             new LinkDto(_urlHelper.Link(nameof(GetAllUsers), new
             {
                 pagecount = queryParameters.PageCount,
                 page = queryParameters.Page,
                 orderby = queryParameters.OrderBy
             }), "self", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllUsers), new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }), "first", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllUsers), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }), "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllUsers), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }), "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllUsers), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }), "previous", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> GetLinks(int id)
        {
            var links = new List<LinkDto>();

            links.Add(
              new LinkDto(_urlHelper.Link(nameof(GetSingleUser), new { id = id }),
              "self",
              "GET"));

            links.Add(
              new LinkDto(_urlHelper.Link(nameof(RemoveUser), new { id = id }),
              "delete_food",
              "DELETE"));

            links.Add(
              new LinkDto(_urlHelper.Link(nameof(AddUser), null),
              "create_food",
              "POST"));

            links.Add(
               new LinkDto(_urlHelper.Link(nameof(UpdateUser), new { id = id }),
               "update_food",
               "PUT"));

            return links;
        }


        #endregion
    }


    public class Users2Controller : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("2.0");
        }
    }
}