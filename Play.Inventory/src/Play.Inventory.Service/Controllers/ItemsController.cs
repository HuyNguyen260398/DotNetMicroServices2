using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _inventoryRepo;
        private readonly IRepository<CatalogItem> _catalogRepo;

        public ItemsController(IRepository<InventoryItem> inventoryRepo, IRepository<CatalogItem> catalogRepo)
        {
            _inventoryRepo = inventoryRepo;
            _catalogRepo = catalogRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            var inventoryItemEntities = await _inventoryRepo.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
            var catalogItemEntities = await _catalogRepo.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<IActionResult> GrantItems(GrantItemsDto dto)
        {
            var inventoryItem = await _inventoryRepo.GetAsync(item => item.UserId == dto.UserId && item.CatalogItemId == dto.CatalogItemId);

            if (inventoryItem is null)
            {
                inventoryItem = new InventoryItem
                {
                    UserId = dto.UserId,
                    CatalogItemId = dto.CatalogItemId,
                    Quantity = dto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };
                await _inventoryRepo.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += dto.Quantity;
                inventoryItem.AcquiredDate = DateTimeOffset.UtcNow;
                await _inventoryRepo.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
    }
}