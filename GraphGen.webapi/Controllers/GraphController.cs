using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PuzzleGraphGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphGen.webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GraphController : ControllerBase
    {
        private readonly ILogger<GraphController> _logger;

        public GraphController(ILogger<GraphController> logger)
        {
            _logger = logger;
        }

        [HttpGet, Route("/Generate")]
        public GraphContainer Generate(int seed = -1, int maxDepth = 4, int maxBranches = 3)
        {
            return GraphContainer.Generate(seed, maxDepth, maxBranches);
        }

        [HttpGet, Route("/GenerateXML")]
        public string GenerateXML(int seed = 50, int maxDepth = 2, int maxBranches = 4)
        {
            return GraphContainer.GenerateXML(seed, maxDepth, maxBranches);
        }
    }
}
