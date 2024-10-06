using CRM_Pipeline.DAL;
using CRM_Pipeline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM_Pipeline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeadsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeads()
        {
            var leadIdsInHistory = await _context.Lead_Stage_History
                .Select(lsh => lsh.Lead_Id)
                .ToListAsync();

            var leads = await _context.Leads
                .Where(x => x.IsActive == true && leadIdsInHistory.Contains(x.Id))
                .Select(lead => new
                {
                    Lead = lead,
                    Customer = _context.Customers.FirstOrDefault(c => c.Id == lead.CustomerId),
                    User = _context.Users.FirstOrDefault(u => u.Id == lead.UserId),
                    Product = _context.Products.FirstOrDefault(p => p.Id == lead.Product_Id),
                    Lead_Stag_History = _context.Lead_Stage_History.FirstOrDefault(p => p.Lead_Id == lead.Id)
                })
                .ToListAsync();

            if (!leads.Any())
            {
                return NotFound("No active leads found.");
            }

            return Ok(leads);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetLead(Guid id)
        {
            var lead = await _context.Leads
                .Where(l => l.Id == id && l.IsActive == true)
                .Select(l => new
                {
                    Lead = l,
                    Customer = _context.Customers.FirstOrDefault(c => c.Id == l.CustomerId),
                    User = _context.Users.FirstOrDefault(u => u.Id == l.UserId),
                    Product = _context.Products.FirstOrDefault(p => p.Id == l.Product_Id),
                    Lead_Stag_History = _context.Lead_Stage_History.FirstOrDefault(p => p.Lead_Id == l.Id)
                })
                .FirstOrDefaultAsync();

            if (lead == null)
                return NotFound("Lead not found or inactive.");

            return Ok(lead);
        }

        [HttpGet("GetLeadFullname/{fullname}")]
        public async Task<IActionResult> GetLeadFullname(string fullname)
        {
            if (string.IsNullOrWhiteSpace(fullname))
                return BadRequest("Full name cannot be empty.");

            var customers = await _context.Customers.ToListAsync();

            var matchingCustomers = customers
                .Where(c => c.Name.Contains(fullname, StringComparison.OrdinalIgnoreCase) ||
                            c.Surname.Contains(fullname, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!matchingCustomers.Any())
                return NotFound("No customers found.");

            var customerIds = matchingCustomers.Select(c => c.Id).ToList();
            var leads = await _context.Leads
                .Where(l => customerIds.Contains(l.CustomerId) && l.IsActive == true)
                .Select(l => new
                {
                    Lead = l,
                    Customer = _context.Customers.FirstOrDefault(c => c.Id == l.CustomerId),
                    User = _context.Users.FirstOrDefault(u => u.Id == l.UserId)
                })
                .ToListAsync();

            if (!leads.Any())
            {
                return NotFound("No leads found for the given customer.");
            }

            return Ok(leads);
        }


        [HttpGet("CustomersProducts")]
        public async Task<IActionResult> GetCusPro()
        {
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

            var data = new
            {
                Customers = customers,
                Products = products
            };

            return Ok(data); 
        }


        [HttpPost]
        public async Task<IActionResult> CreateLead([FromBody] LeadCreateDto leadDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //var fullName = leadDto.FullName.Split(' ');
            //var customer = new Customers
            //{
            //    Name = fullName[0],
            //    Surname = fullName.Length > 1 ? fullName[1] : "",
            //    Email = "",
            //    PhoneNumber = "",
            //    Company = ""
            //};

            //_context.Customers.Add(customer);
            // await _context.SaveChangesAsync();

            var lead = new Lead
            {
                CustomerId = leadDto.CustomerId,
                Product_Id = leadDto.ProductId,
                ExpectedRevenue = leadDto.ExpectedRevenue,
                UserId = leadDto.UserId,
                Stage_Id = leadDto.StageId
            };

            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();

            var leaddetail = new LeadStageHistory
            {
                Probability = leadDto.Probability,
                ExpectedClosingDate = leadDto.ExpectedClosingDate,
                Lead_Id = lead.Id,
                User_Id = lead.UserId
            };

            _context.Lead_Stage_History.Add(leaddetail);
            await _context.SaveChangesAsync();

            await UpdateTotalRevenueForStage(lead.Stage_Id);

            return CreatedAtAction(nameof(GetLead), new { id = lead.Id }, lead);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLead(Guid id)
        {
            var lead = await _context.Leads.FindAsync(id);
            if (lead == null)
                return NotFound("Lead not found.");

            lead.IsActive = false;
            var stage=await _context.Stages.FindAsync(lead.Stage_Id);
            if (stage != null)
            {
                stage.Total_Revenue -= lead.ExpectedRevenue;
            }
               await _context.SaveChangesAsync();
               return Ok(new { message = "Lead deleted." }); 
        }

        private async Task UpdateTotalRevenueForStage(Guid stageId)
        {
            var total_revenue = await _context.Leads.Where(x => x.Stage_Id == stageId).SumAsync(x => x.ExpectedRevenue);
            var stage = await _context.Stages.FindAsync(stageId);
            if (stage != null)
            {
                stage.Total_Revenue = (double)total_revenue;
                _context.Stages.Update(stage);
                await _context.SaveChangesAsync();
            }
        }

        [HttpPut("{leadId}")]
        public async Task<IActionResult> UpdateLead(Guid leadId, Guid stageId)
        {
            var lead = await _context.Leads.FindAsync(leadId);
            var stage = await _context.Stages.FindAsync(lead.Stage_Id);
            stage.Total_Revenue -= lead.ExpectedRevenue;
            _context.Stages.Update(stage);
            if (lead == null || lead.IsActive == false)
                return NotFound("Lead not found or inactive.");
            lead.Stage_Id = stageId;
           
            await _context.SaveChangesAsync();

            await UpdateTotalRevenueForStage(stageId);


            return Ok(lead); 
        }
        [HttpGet("SearchCustomers/{searchTerm}")]
        public async Task<IActionResult> SearchCustomers(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Customer cannot be empty.");

            var customers = await _context.Customers
                .Where(c => c.Name.ToLower().Contains(searchTerm.ToLower()) ||
                             c.Surname.ToLower().Contains(searchTerm.ToLower()))
                .Select(c => new
                {
                    c.Id,
                    c.Name
                })
                .ToListAsync();

            if (!customers.Any())
                return NotFound("No matching customers found.");

            return Ok(customers);
        }
        [HttpGet("SearchProducts/{searchTerm}")]
        public async Task<IActionResult> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Product cannot be empty.");

            var products = await _context.Products
                .Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()))
                .Select(p => new
                {
                    p.Id,
                    p.Name
                })
                .ToListAsync();

            if (!products.Any())
                return NotFound("No matching products found.");

            return Ok(products);
        }

    }
}
