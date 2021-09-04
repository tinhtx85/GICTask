using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GICTask.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PopulationController : ControllerBase
    {
        private BLLPopulation bLLPopulation;
        private readonly ILogger<PopulationController> _logger;
        public PopulationController(ILogger<PopulationController> logger, PopulationContext context)
        {
            _logger = logger;
            bLLPopulation = new BLLPopulation(context);
        }
        /// <summary>
        /// API Get state population
        /// </summary>
        /// <param name="state"> list state Id</param>
        /// <returns>Json population</returns>
        [HttpGet]
        public IActionResult Get(string state)
        {
            List<Population> lstPopulation = new List<Population>();
            List<int> statesNumbers = new List<int>();
            _logger.LogInformation("API endpoint called - " + Request.Path.ToString() + Request.QueryString);
            //Parse string and check state id available
            string lstState = ParseListState(state, ref statesNumbers);

            if (lstState != "success")
            {
                return NotFound(lstState);
            }
            else
            {
                foreach (int stateId in statesNumbers)
                {
                    Population itemPopulation = bLLPopulation.GetPopulation(stateId);
                    lstPopulation.Add(itemPopulation);
                }
            }
            return Ok(lstPopulation);
        }

        /// <summary>
        /// check and parse string list state with format 1,2,3
        /// </summary>
        /// <param name="states">list Id State</param>
        /// <param name="statesNumbers">Return list int</param>
        /// <returns> check status</returns>
        private string ParseListState(string states, ref List<int> statesNumbers)
        {
            try
            {
                statesNumbers = states.Split(',').Select(Int32.Parse).ToList();
                foreach (int state in statesNumbers)
                {
                    if ((state < 1) || (state > 51))
                        return "state id must from 1 to 51";
                }
            }
            catch
            {
                return "List state invalid";
            }
            return "success";
        }
    }
}
