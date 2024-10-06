using CRM_Pipeline.DAL;
using CRM_Pipeline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CRM_Pipeline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Lead_Stage_HistoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Lead_Stage_HistoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeads()
        {
            var leaddetails = await _context.Lead_Stage_History.ToListAsync();
            if (!leaddetails.Any())
                return NotFound("No active leads found.");

            return Ok(leaddetails); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLead(Guid id)
        {
            var lead = await _context.Leads.FindAsync(id);

            if (lead == null || lead.IsActive == false)
                return NotFound("Lead not found or inactive.");

            var customer = await _context.Customers.FindAsync(lead.CustomerId);
            var user = await _context.Users.FindAsync(lead.UserId);

            var leadStageHistory = await _context.Lead_Stage_History
            .Where(lsh => lsh.Lead_Id == lead.Id)
            .Select(lsh => new LeadStageHistoryDto
            {
                Id = lsh.Id,
                Probability = lsh.Probability,
                ExpectedClosingDate = lsh.ExpectedClosingDate ?? DateTime.MinValue,
                User_Id = lsh.User_Id,
                Extra_Information = string.IsNullOrEmpty(lsh.Extra_Information) ? "No extra information" : lsh.Extra_Information,
                Internal_Notes = string.IsNullOrEmpty(lsh.Internal_Notes) ? "No internal notes" : lsh.Internal_Notes,
                Changed_At = lsh.Changed_At
            })
            .ToListAsync();

            if (!leadStageHistory.Any())
            {
                leadStageHistory = new List<LeadStageHistoryDto>
                {
                    new LeadStageHistoryDto
                    {
                        Id = Guid.NewGuid(),
                        Probability = 0.0,
                        ExpectedClosingDate = DateTime.MinValue,
                        User_Id = 0,
                        Extra_Information = "No extra information",
                        Internal_Notes = "No internal notes",
                        Changed_At = DateTime.Now
                    }
                };
            }
            var customers = await _context.Customers
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToListAsync();

            var products = await _context.Products
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToListAsync();


            return Ok(new
            {
                Customers = customers,
                Products = products,
                Lead = lead,
                Customer = customer,
                LeadStageHistory = leadStageHistory,
                User = user
            }); 
        }

        [HttpPut]
        public async Task<IActionResult> UpdateLead([FromBody] LeadStageHistoryDto updatedLeadStageHistory)
        {
            var lead = await _context.Leads.FindAsync(updatedLeadStageHistory.Id);
            if (lead == null)
                return NotFound("Lead not found.");

            var customer = await _context.Customers.FindAsync(lead.CustomerId);
            if (customer == null)
                return NotFound("Customer not found.");

            var product = await _context.Products.FindAsync(lead.Product_Id);
            if (product == null)
                return NotFound("Customer not found.");

            var leadStageHistory = await _context.Lead_Stage_History
                .FirstOrDefaultAsync(lsh => lsh.Lead_Id == lead.Id);

            if (leadStageHistory == null)
            {
                leadStageHistory = new LeadStageHistory
                {
                    Lead_Id = lead.Id,
                    Changed_At = DateTime.Now
                };
                _context.Lead_Stage_History.Add(leadStageHistory);
            }

            leadStageHistory.Probability = updatedLeadStageHistory.Probability;

            //leadStageHistory.ExpectedClosingDate =
            //    !string.IsNullOrEmpty(updatedLeadStageHistory.ExpectedClosingDate) &&
            //    updatedLeadStageHistory.ExpectedClosingDate != "No date available"
            //        ? (DateTime?)DateTime.Parse(updatedLeadStageHistory.ExpectedClosingDate)
            //        : leadStageHistory.ExpectedClosingDate;

            leadStageHistory.ExpectedClosingDate =  updatedLeadStageHistory.ExpectedClosingDate!=null
                   ? updatedLeadStageHistory.ExpectedClosingDate
                   : leadStageHistory.ExpectedClosingDate;

            leadStageHistory.User_Id = updatedLeadStageHistory.User_Id;

            leadStageHistory.Extra_Information = string.IsNullOrEmpty(updatedLeadStageHistory.Extra_Information)
                ? leadStageHistory.Extra_Information
                : updatedLeadStageHistory.Extra_Information;

            leadStageHistory.Internal_Notes = string.IsNullOrEmpty(updatedLeadStageHistory.Internal_Notes)
                ? leadStageHistory.Internal_Notes
                : updatedLeadStageHistory.Internal_Notes;

            leadStageHistory.Changed_At = DateTime.Now;

            lead.ExpectedRevenue = updatedLeadStageHistory.ExpectedRevenue.HasValue
                ? updatedLeadStageHistory.ExpectedRevenue.Value
                : lead.ExpectedRevenue;

            lead.Product_Id = updatedLeadStageHistory.ProductId ==null ? lead.Product_Id : updatedLeadStageHistory.ProductId;

            customer.Name = string.IsNullOrEmpty(updatedLeadStageHistory.Name) ? customer.Name : updatedLeadStageHistory.Name;
            customer.Surname = string.IsNullOrEmpty(updatedLeadStageHistory.Surname) ? customer.Surname : updatedLeadStageHistory.Surname;
            customer.Email = string.IsNullOrEmpty(updatedLeadStageHistory.Email) ? customer.Email : updatedLeadStageHistory.Email;
            customer.PhoneNumber = string.IsNullOrEmpty(updatedLeadStageHistory.Phone_Number) ? customer.PhoneNumber : updatedLeadStageHistory.Phone_Number;
            customer.Company = string.IsNullOrEmpty(updatedLeadStageHistory.Company) ? customer.Company : updatedLeadStageHistory.Company;
            customer.Department = string.IsNullOrEmpty(updatedLeadStageHistory.Department) ? customer.Department : updatedLeadStageHistory.Department;
            customer.Position = string.IsNullOrEmpty(updatedLeadStageHistory.Position) ? customer.Position : updatedLeadStageHistory.Position;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Lead = lead,
                Customer = customer,
                LeadStageHistory = leadStageHistory
            }); 
        }

        [HttpDelete("DeleteLeadStageHistory/{id}")]
        public async Task<IActionResult> DeleteLeadStageHistory(Guid id)
        {
            var leadStageHistory = await _context.Lead_Stage_History.FindAsync(id);

            if (leadStageHistory == null)
            {
                return NotFound($"Lead_Stage_History with Id = {id} not found.");
            }

            _context.Lead_Stage_History.Remove(leadStageHistory);

            await _context.SaveChangesAsync();

            return Ok($"Lead_Stage_History with Id = {id} has been deleted.");
        }
    }
}
