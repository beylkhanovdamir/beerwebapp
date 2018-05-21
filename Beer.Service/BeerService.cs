﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BeerApp.Entities;
using BeerApp.Service.Common;
using BeerApp.Service.Interfaces;
using BeerApp.Service.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BeerApp.Service
{
    public class BeerService : IBeerService
    {
	    private readonly IOptions<BreweryDBSettings> _settings;

	    public BeerService(IOptions<BreweryDBSettings> settings)
	    {
		    _settings = settings;
	    }

	    public async Task<ResponseModel<IEnumerable<Beer>>> GetAllBeers()
	    {
		    var response = await HttpClientWrapper.GetInstance(_settings.Value.ApiUrl).GetAsync($"beers?key={_settings.Value.ApiSecretKey}");
		    string json;
		    using (var content = response.Content)
		    {
			    json = await content.ReadAsStringAsync();
		    }
		    return JsonConvert.DeserializeObject<ResponseModel<IEnumerable<Beer>>>(json);
	    }

		
	    public async Task<ResponseModel<Beer>> GetBeerById(string id)
	    {
		    var response = await HttpClientWrapper.GetInstance(_settings.Value.ApiUrl).GetAsync($"beer/{id}/?key={_settings.Value.ApiSecretKey}");
		    string json;
		    using (var content = response.Content)
		    {
			    json = await content.ReadAsStringAsync();
		    }
		    return JsonConvert.DeserializeObject<ResponseModel<Beer>>(json);
	    }
    }
}
