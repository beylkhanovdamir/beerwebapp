using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BeerApp.Bll.Beers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BeerApp.Web.Controllers
{
	// [Route("api/[controller]")]
	[Route("api")]
	[EnableCors("CorsPolicy")]
	[ApiController]
	public class BeerController : Controller
	{
		private readonly IBeerManager _beerManager;

		public BeerController(IBeerManager beerManager)
		{
			_beerManager = beerManager;
		}

		[HttpGet]
		[Route("[action]")]
		[Route("[action]/{query}")]
		[Route("[action]/{styleId}")]
		public async Task<IActionResult> Beers(string query, int? styleId)
		{
			var result = await _beerManager.GetBeers(query, styleId);
			if (result.StatusCode == HttpStatusCode.OK)
			{
				return Ok(result.Data);
			}

			return BadRequest(result.ErrorMessage);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> BeerStyles()
		{
			var result = await _beerManager.GetBeerStyles();
			if (result.StatusCode == HttpStatusCode.OK)
			{
				return Ok(result.Data.OrderBy(x => x.Name));
			}

			return BadRequest(result.ErrorMessage);
		}

		[HttpGet]
		[Route("[action]/{id}")]
		public async Task<IActionResult> Beer([Required]string id)
		{
			var result = await _beerManager.GetBeerById(id);
			if (result.StatusCode == HttpStatusCode.OK)
			{
				return Ok(result.Data);
			}

			return BadRequest(result.ErrorMessage);
		}
	}
}
