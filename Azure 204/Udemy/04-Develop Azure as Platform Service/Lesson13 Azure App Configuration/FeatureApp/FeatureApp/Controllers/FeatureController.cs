using FeatureApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace FeatureApp.Controllers
{
    public class FeatureController : Controller
    {
        private readonly IFeatureManager _featureManager;
        public FeatureController(IFeatureManager featureManager) 
        {
            _featureManager = featureManager;
        }

        [FeatureGate(FeatureFlag.Staging)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
