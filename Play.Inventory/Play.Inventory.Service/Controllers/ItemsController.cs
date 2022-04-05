using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _itemsRepo;

        public ItemsController(IRepository<InventoryItem> itemsRepo)
        {
            _itemsRepo = itemsRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            var items = (await _itemsRepo.GetAllAsync(item => item.UserId == userId)).Select(item => item.AsDto());
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> GrantItems(GrantItemsDto dto)
        {
            var inventoryItem = await _itemsRepo.GetAsync(item => item.UserId == dto.UserId && item.CatalogItemId == dto.CatalogItemId);

            if (inventoryItem is null)
            {
                inventoryItem = new InventoryItem
                {
                    UserId = dto.UserId,
                    CatalogItemId = dto.CatalogItemId,
                    Quantity = dto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };
                await _itemsRepo.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += dto.Quantity;
                inventoryItem.AcquiredDate = DateTimeOffset.UtcNow;
                await _itemsRepo.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
    }
}