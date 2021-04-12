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
        public GraphContainer Generate(int seed = -1, int maxDepth = 4, int maxBranches = 3, int skipOdds = 5)
        {
            return GraphContainer.Generate(new GenerateParameters
            {
                Seed = seed,
                MaxDepth = maxDepth,
                MaxBranches = maxBranches,
                SkipOdds = skipOdds
            });
        }

        [HttpGet, Route("/GenerateXML")]
        public string GenerateXML(int seed = 50, int maxDepth = 2, int maxBranches = 4, int skipOdds = 8,
                                  bool doSort = true, bool doSwap = true, bool doCompressX = true, bool doCompressY = true)
        {
            return GraphContainer.GenerateXML(new GenerateParameters { 
                Seed = seed,
                MaxDepth = maxDepth,
                MaxBranches = maxBranches,
                SkipOdds = skipOdds,
                DoSort = doSort,
                DoSwap = doSwap,
                DoCompressX = doCompressX,
                DoCompressY = doCompressY
            });
        }

        [HttpPost, Route("/GenerateXML")]
        public string GenerateXML(GenerateParameters parameters)
        {
            return GraphContainer.GenerateXML(parameters);
        }

        [HttpGet, Route("/DigExample")]
        public string DigExample() => GraphContainer.DigExample();
    }
}
