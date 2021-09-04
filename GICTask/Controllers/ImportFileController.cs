using ExcelDataReader;
using GICTask.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GICTask.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImportFileController : ControllerBase
    {
        private readonly ILogger<ImportFileController> _logger;
        private BLLPopulation bLLPopulation;
        public ImportFileController(ILogger<ImportFileController> logger, PopulationContext context)
        {
            _logger = logger;
            bLLPopulation = new BLLPopulation(context);
        }

        /// <summary>
        /// Post File Excel.
        /// </summary>
        /// <returns>Status import DB</returns>
        [HttpPost]
        public async Task<ActionResult> OnPostImport()
        {
            _logger.LogInformation("API endpoint called - " + Request.Path.ToString() + Request.QueryString);
            string folderName = "Upload";
            string wwwPath = AppContext.BaseDirectory;
            string newPath = Path.Combine(wwwPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            IFormFile file = Request.Form.Files[0];
            if (file.Length > 0)
            {
                List<Actual> lstActual = new List<Actual>();
                List<Estimate> lstEstimate = new List<Estimate>();

                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet dsExcelRecords = reader.AsDataSet();

                        // Read Sheet Estimate.
                        DataTable dtEstimateRecords = dsExcelRecords.Tables[0];
                        for (int i = 1; i < dtEstimateRecords.Rows.Count; i++)
                        {
                            Estimate item = new Estimate
                            {
                                State = int.Parse(dtEstimateRecords.Rows[i][0].ToString()),
                                Districts = int.Parse(dtEstimateRecords.Rows[i][1].ToString()),
                                EstimatesPopulation = double.Parse(dtEstimateRecords.Rows[i][2].ToString()),
                                EstimatesHouseholds = double.Parse(dtEstimateRecords.Rows[i][3].ToString()),
                            };
                            lstEstimate.Add(item);
                        }

                        // Read Sheet Actual.
                        DataTable dtActualRecords = dsExcelRecords.Tables[1];
                        for (int i = 1; i < dtActualRecords.Rows.Count; i++)
                        {
                            Actual item = new Actual
                            {
                                State = Int32.Parse(dtActualRecords.Rows[i][0].ToString()),
                                ActualPopulation = double.Parse(dtActualRecords.Rows[i][1].ToString()),
                                ActualHouseholds = double.Parse(dtActualRecords.Rows[i][2].ToString()),
                            };
                            lstActual.Add(item);
                        }
                    }
                }
                string strReturn = await bLLPopulation.InitPopulations(lstActual, lstEstimate);
                return Ok(strReturn);
            }
            else
                return BadRequest("File lenght must > 0 !!!");
        }

    }
}
