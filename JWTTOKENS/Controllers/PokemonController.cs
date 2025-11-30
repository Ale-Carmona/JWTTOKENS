using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTTOKENS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PokemonController : Controller
    {
        private static readonly List<Models.PokemonModels> _pokemons = new()
        {
            new Models.PokemonModels { Id = 1, Name = "Bulbasaur", Type = "Grass/Poison", Level = 10, Description = "A small dinosaur with a plant bulb on its back." },
            new Models.PokemonModels { Id = 2, Name = "Charmander", Type = "Fire", Level = 15, Description = "A small fire lizard." },
            new Models.PokemonModels { Id = 3, Name = "Squirtle", Type = "Water", Level = 12, Description = "A small turtle that squirts water." }
        };

        #region--> GET: api/Pokemon
        [HttpGet]
        public IActionResult GetAllPokemons()
        {
            return Ok(_pokemons);
        }
        #endregion


        #region--> GET: api/Pokemon/{id}
        [HttpGet("{id}")]
        public IActionResult GetPokemonById(int id)
        {
            var pokemon = _pokemons.FirstOrDefault(p => p.Id == id);
            if (pokemon is null)
                return NotFound(new { message = $"Pokémon con Id {id} no encontrado." });

            return Ok(pokemon);
        }
        #endregion


        #region--> POST: api/Pokemon
        [HttpPost]
        public IActionResult CreatePokemon([FromBody] Models.PokemonModels newPokemon)
        {
            if (newPokemon is null)
                return BadRequest(new { message = "El cuerpo de la petición no puede estar vacío." });

            // Asignar Id automáticamente si no viene proporcionado
            if (newPokemon.Id == 0)
            {
                var nextId = _pokemons.Any() ? _pokemons.Max(p => p.Id) + 1 : 1;
                newPokemon.Id = nextId;
            }
            else
            {
                // Evitar duplicados de Id
                if (_pokemons.Any(p => p.Id == newPokemon.Id))
                    return Conflict(new { message = $"Ya existe un Pokémon con Id {newPokemon.Id}." });
            }
            _pokemons.Add(newPokemon);
            return CreatedAtAction(nameof(GetPokemonById), new { id = newPokemon.Id }, newPokemon);
        }
        #endregion


        #region--> PUT: api/Pokemon/{id}
        [HttpPut("{id}")]
        public IActionResult UpdatePokemon(int id, [FromBody] Models.PokemonModels updatedPokemon)
        {
            if (updatedPokemon is null)
                return BadRequest(new { message = "El cuerpo de la petición no puede estar vacío." });

            var existingPokemon = _pokemons.FirstOrDefault(p => p.Id == id);
            if (existingPokemon is null)
                return NotFound(new { message = $"Pokémon con Id {id} no encontrado." });

            // Actualizar campos
            existingPokemon.Name = updatedPokemon.Name;
            existingPokemon.Type = updatedPokemon.Type;
            existingPokemon.Level = updatedPokemon.Level;
            existingPokemon.Description = updatedPokemon.Description;

            return Ok(new { message = "Pokémon actualizado correctamente.", pokemon = existingPokemon });
        }
        #endregion


        #region--> DELETE: api/Pokemon/{id}
        [HttpDelete("{id}")]
        public IActionResult DeletePokemon(int id)
        {
            var pokemon = _pokemons.FirstOrDefault(p => p.Id == id);
            if (pokemon is null)
                return NotFound(new { message = $"Pokémon con Id {id} no encontrado." });

            _pokemons.Remove(pokemon);

            return Ok(new { message = $"Pokémon con Id {id} eliminado correctamente." });
        }
        #endregion

    }
}
