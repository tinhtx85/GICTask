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
        private readonly PopulationContext _context;
        private readonly ILogger<ImportFileController> _logger;
        private readonly IWebHostEnvironment _environment;
        public ImportFileController(ILogger<ImportFileController> logger, PopulationContext context, IWebHostEnvironment environment)
        {
            _logger = logger;
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        public async Task<ActionResult> OnPostImport()
        {
            string folderName = "Upload";
            string wwwPath = AppContext.BaseDirectory; //this._environment.WebRootPath;
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
                try
                {
                    _context.tblActuals.AddRange(lstActual);
                    await _context.SaveChangesAsync();

                    _context.tblEstimates.AddRange(lstEstimate);
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    return BadRequest("DB Error:"+ex.ToString());
                }
                
                return Ok("Success:"+ JsonConvert.SerializeObject(lstActual));
            }
            else
                return BadRequest("File lenght:" + file.Length);

        }
    }
}
