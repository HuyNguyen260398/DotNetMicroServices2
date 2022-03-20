using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> _items = new()
        {
            new ItemDto(Guid.NewGuid(), "Item 1", "Description 1", 10.00m, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Item 2", "Description 2", 10.00m, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Item 3", "Description 3", 10.00m, DateTimeOffset.UtcNow),
        };

        [HttpGet]
        public IEnumerable<ItemDto> Get() => _items;

        [HttpGet("{id}")]
        public ItemDto GetById(Guid id) => _items.Find(i => i.Id == id);

        [HttpPost]
        public ActionResult<ItemDto> Create(CreateItemDto dto)
        {
            var newItem = new ItemDto(Guid.NewGuid(), dto.Name, dto.Description, dto.Price, DateTimeOffset.UtcNow);
            _items.Add(newItem);
            return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, UpdateItemDto dto)
        {
            var existItem = _items.Find(i => i.Id == id);
            if (existItem == null)
                return NotFound();

            var updatedItem = existItem with
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            };

            var index = _items.FindIndex(existItem => existItem.Id == id);
            _items[index] = updatedItem;

            return NoContent();
        }
    }
}