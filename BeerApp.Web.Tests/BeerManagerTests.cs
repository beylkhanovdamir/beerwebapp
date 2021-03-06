﻿using System;
using System.Linq;
using BeerApp.Bll.Beers;
using BeerApp.Entities;
using BeerApp.Service;
using BeerApp.Service.Common;
using BeerApp.Service.Interfaces;
using Xunit;

namespace BeerApp.Bll.Tests
{
	/// <summary>
	/// integration tests
	/// </summary>
	public class BeerManagerTests
	{
		private readonly IBeerManager _beerManager;

		private BreweryDBSettings _apiSettings =>
			new BreweryDBSettings()
			{
				ApiSecretKey = "YourSecretKey",
				ApiUrl = "http://api.brewerydb.com/v2/"
			};

		private BreweryDBClient _http => new BreweryDBClient(HttpClientWrapper.GetInstance(_apiSettings.ApiUrl));

		public BeerManagerTests()
		{
			_beerManager = new BeerManager(new BeerService(_apiSettings, _http));
		}

		[Fact]
		public async void InvalidBreweryDbSettingsTest()
		{
			// act
			var fakeBeerService = new BeerService(new BreweryDBSettings()
			{
				ApiSecretKey = "FakeSecretKey",
				ApiUrl = "http://api.brewerydb.com/v2/"
			}, _http);

			var beerManager = new BeerManager(fakeBeerService);

			// act
			var result = await beerManager.GetBeers();

			// assert
			Assert.NotNull(result.ErrorMessage); // error message about API key
			Assert.Null(result.Data);
		}

		[Fact]
		public async void GetAllBeersTest()
		{
			// act
			var result = await _beerManager.GetBeers();

			// assert
			Assert.Null(result.ErrorMessage);
			Assert.True(result.Data.Any());
		}

		[Theory]
		[InlineData("Porter")]
		public async void GetBeersByQueryTest(string query)
		{
			// act
			var result = await _beerManager.GetBeers(query);

			// assert
			Assert.NotNull(result);
			Assert.True(result.Data.Any());
			Assert.True(result.Data.All(x=>x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase)!= -1));
		}

		[Theory]
		[InlineData(122)]
		public async void SearchBeersByStyleTest(int? styleId)
		{
			// act
			var result = await _beerManager.GetBeers(null, styleId);

			// assert
			Assert.True(result.Data.Any());
		}


		[Theory]
		[InlineData(999)]
		public async void SearchBeersByNotExistingStyleTest(int? styleId)
		{
			// act
			var result = await _beerManager.GetBeers(null, styleId);

			// assert
			Assert.Null(result.Data);
		}

		/// <summary>
		/// Brewery API doesn't provide a way to filter by query and other filters at a time, thereby, we can't use filter with query
		/// Fo example, 999 is not existing style id, and if we will filter by this style, then we have been get an empty result, (see <see cref="SearchBeersByNotExistingStyle"/>)
		/// but in this case, we will have search results, because if we have query, then beer style filter will be ignored
		/// </summary>
		/// <param name="query"></param>
		/// <param name="styleId"></param>
		[Theory]
		[InlineData("Porter", 999)]
		public async void SearchBeersByStyleAndQueryTest(string query, int? styleId)
		{
			// act
			var result = await _beerManager.GetBeers(query, styleId);

			// assert
			Assert.True(result.Data.Any());
		}


		[Fact]
		public async void GetBeersStylesTest()
		{
			// act
			var result = await _beerManager.GetBeerStyles();

			// assert
			Assert.True(result.Data.Any());
		}

		[Theory]
		[InlineData("RIV5hX", true)]
		[InlineData("NOTEXISTINGID", false)]
		public async void GetBeerByIdTest(string id, bool beerIsExists)
		{
			// act
			var result = await _beerManager.GetBeerById(id);

			// assert
			Assert.Equal(result.Data != null, beerIsExists);
		}
	}
}
