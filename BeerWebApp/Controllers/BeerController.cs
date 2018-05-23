﻿using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BeerApp.Bll.Beers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BeerApp.Web.Controllers
{
	[Route("api")]
	[EnableCors("CorsPolicy")]
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
			if (string.IsNullOrEmpty(result.ErrorMessage))
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
			if (string.IsNullOrEmpty(result.ErrorMessage))
			{
				return Ok(result.Data);
			}

			return BadRequest(result.ErrorMessage);
		}

		[HttpGet]
		[Route("[action]/{id}")]
		public async Task<IActionResult> Beer([Required]string id)
		{
			if (ModelState.IsValid)
			{
				var result = await _beerManager.GetBeerById(id);
				if (string.IsNullOrEmpty(result.ErrorMessage))
				{
					return Ok(result.Data);
				}

				return BadRequest(result.ErrorMessage);
			}

			return BadRequest("Beer Id is required");
		}
	}
}
