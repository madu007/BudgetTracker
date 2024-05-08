using AutoMapper;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Model;
using BudgetTracker.Domain.Model.DB;
using BudgetTracker.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet("All Categories")]
        public async Task<IActionResult> GetAll()
        {
            var catEntity = await _categoryService.GetAllAsync();
            return Ok(_mapper.Map<List<CategoryModel>>(catEntity));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var catModel =  await _categoryService.GetByIdAsync(id);
            if (catModel == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, 
                  new  Response { Status = "Error", Message = "The category was not found, Please try again", IsSuccess = false });
            }
            return Ok(_mapper.Map<CategoryModel>(catModel));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CategoryModel categoryModel)
        {
            
            var Category = await _categoryService.AddAsync(categoryModel);
            if (Category == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                new Response { IsSuccess = false, Status = "Error", Message = "Your Category is not created, Please try again." });
            }
            return Ok(StatusCode(StatusCodes.Status200OK,
                new Response { IsSuccess = true, Message = "You created a new Category successfully", Status = "Success" }));
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CategoryModel categoryModel)
        {
            var catViewModel = _mapper.Map<Category>(categoryModel);
            catViewModel = await _categoryService.UpdateAsync(id, catViewModel);
            if (catViewModel == null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, new Response { Status = "Error", Message = "Unable to update please try again!", IsSuccess = false });
            }
            return StatusCode(StatusCodes.Status200OK, new Response { IsSuccess = true, Status = "Success", Message = "Your category is update successfully" });
        }

        [HttpDelete("Delete{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var catViewModel = await _categoryService.DeleteAsync(id);
            if (catViewModel == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", IsSuccess = false, Message = "Bad Request!" });
            }
            return StatusCode(StatusCodes.Status200OK,
                new Response { IsSuccess = true, Status = "Success", Message = "Category deleted successfully" });
        }
    }
}
