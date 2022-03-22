using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        // private static readonly List<ItemDto> _items = new()
        // {
        //     new ItemDto(Guid.NewGuid(), "Item 1", "Description 1", 10.00m, DateTimeOffset.UtcNow),
        //     new ItemDto(Guid.NewGuid(), "Item 2", "Description 2", 10.00m, DateTimeOffset.UtcNow),
        //     new ItemDto(Guid.NewGuid(), "Item 3", "Description 3", 10.00m, DateTimeOffset.UtcNow),
        // };

        private readonly ItemsRepo _itemsRepo = new();

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok((await _itemsRepo.GetAllAsync()).Select(item => item.AsDto()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var item = await _itemsRepo.GetAsync(id);

            if (item is null)
                return NotFound();

            return Ok(item.AsDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateItemDto dto)
        {
            var item = new Item
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _itemsRepo.CreateAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateItemDto dto)
        {
            var item = await _itemsRepo.GetAsync(id);

            if (item is null)
                return NotFound();

            item.Name = dto.Name;
            item.Description = dto.Description;
            item.Price = dto.Price;

            await _itemsRepo.UpdateAsync(item);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await _itemsRepo.GetAsync(id);

            if (item is null)
                return NotFound();

            await _itemsRepo.DeleteAsync(item.Id);

            return NoContent();
        }
    }
}