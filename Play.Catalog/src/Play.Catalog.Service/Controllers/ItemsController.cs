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
            var item = new ItemDto(Guid.NewGuid(), dto.Name, dto.Description, dto.Price, DateTimeOffset.UtcNow);
            _items.Add(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
    }
}