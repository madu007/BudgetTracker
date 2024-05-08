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
    
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [HttpGet("GetAllTransaction")]
        public async Task<IActionResult> GetAllTransaction()
        {
            var transaction = await _transactionService.GatAllTransactionAsync();
            return Ok(_mapper.Map<List<TransactionModel>>(transaction));
        }

        [HttpGet("TransactionId")]
        public async Task<IActionResult> GetTransactionById([FromRoute]int transactionId)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                new Response { IsSuccess = false, Status = "Error", Message = "The Transaction was not found, Please try again." });
            }
            return Ok(transaction);
        }

        [HttpPost("CreateTransaction")]
        public async Task<IActionResult> AddTransaction([FromBody]TransactionModel transaction)
        {
            var trans = await _transactionService.AddTransactionAsync(transaction);
            if (transaction == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                new Response { IsSuccess = true, Status = "Error", Message = "Your Transaction is not created, Please try again." });
            }
            return StatusCode(StatusCodes.Status200OK,
                new Response { IsSuccess = true, Status = "Success", Message = "You created a new Transaction successFully." });
        }

        [HttpPut("UpdateTransaction")]
        public async Task<IActionResult> UpdateTransaction([FromRoute]int transactionId,[FromBody] TransactionModel transactionModel)
        {
            var transaction = _mapper.Map<Transaction>(transactionModel);
            transaction = await _transactionService.UpdateTransactionAsync(transactionId, transaction);
            if (transaction == null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, new Response { Status = "Error", Message = "Unable to update please try again!", IsSuccess = false });
            }
            return StatusCode(StatusCodes.Status406NotAcceptable, new Response { Status = "Success", Message = "Your transaction is Updated successfully.", IsSuccess = true });
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteTransaction([FromRoute]int transactionId)
        {
            var transaction = await _transactionService.DeleteTransactionAsync(transactionId);
            if (transaction == null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, new Response { Status = "Error", Message = "Bad Request!", IsSuccess = false });
            }
            return StatusCode(StatusCodes.Status406NotAcceptable, new Response { Status = "Success", Message = "You have delete the transaction successfully.", IsSuccess = true });

        }
    }
}
